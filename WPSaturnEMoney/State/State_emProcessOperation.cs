using Microsoft.PointOfService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;
using WPSaturnEMoney.ViewModels;

namespace WPSaturnEMoney.State
{
    class State_emProcessOperation : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emProcessOperation)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emProcessOperation;

                GlobalData.SettlementSeqNo = Utilities.GetSettlementSequenceNumber();
                string[] spearator = { "\t" };
                string msgCode = "";
                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties();
                GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();

                bool playSoundOnce = GlobalData.BasicConfig.sound_repeat_while_cat == 0;
                if (Session.ScreenState.PreviousStates[0] == StateMachine.State.emPayment)
                {
                    using (StreamWriter writer = new StreamWriter(GlobalData.AppPath + GlobalData.LastSettlementBrandCodePath, false))
                    {
                        writer.Write(GlobalData.ServiceName);
                    }
                    using (StreamWriter writer = new StreamWriter(GlobalData.AppPath + GlobalData.LastSettlementSeqNoPath, false))
                    {
                        writer.Write(GlobalData.SettlementSeqNo);
                    }

                    // Utilities.PlaySound("emtm1.wav", isPlayOnce: playSoundOnce);

                    msgCode = "E0001";
                    GlobalData.ViewModelProperties.BtnVisibility = "Visible";
                    GlobalData.ViewModelProperties.BtnContent = "決済\n中止";
                    GlobalData.ViewModelProperties.BtnCommand = new Command(CancelFromPayment);
                    Session.MainViewModel.LoadScreen_emProcessOperation(msgCode);

                    FileStruct.ServiceId service = GlobalData.ServiceIdConfig.Find(m => m.brand_code == GlobalData.ServiceName);
                    SaturnAPI.PaymentRequest(GlobalData.BasicConfig.operation_mode, service.service_id);
                    await Task.Delay(500);

                    msgCode = "E0007";
                    Session.MainViewModel.LoadScreen_emProcessOperation(msgCode);

                    GlobalData.CurrentServiceID = ((int)SaturnAPI.SaturnRequestService.StatusNotification).ToString();
                    Session.TimerCount = (int)GlobalData.BasicConfig.settlement_request_timeout * 1000;

                    bool isError = false;
                    bool isFromCancelState = false;
                    while (Session.TimerCount > 0)
                    {
                        if (Session.TimerCount <= 100)
                        {
                            if (GlobalData.TransactionErrorType != Utilities.ErrorType.Timeout)
                            {
                                msgCode = "E0015";
                                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties();
                                GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                                Session.MainViewModel.LoadScreen_emMessage(msgCode);

                                Utilities.Log.Info("Cancel request is sent!");
                                SaturnAPI.CancelRequest();
                                Session.TimerCount = (int)GlobalData.BasicConfig.settlement_cancel_timeout * 1000;
                                GlobalData.TransactionErrorType = Utilities.ErrorType.Timeout;
                            }
                            else
                            {
                                msgCode = "E0004";
                                FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                {
                                    MessageMaintenance = msgMode.msg2,
                                };
                                GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties
                                {
                                    MessageMaintenance = msgMode.msg3,
                                };
                                Utilities.Log.Error("Failed to abort operation!");
                                GlobalData.MsgCode = msgCode;
                                Session.ScreenState.NextState = StateMachine.State.emErrorToMaintenance;
                                GlobalData.SaturnAPIResponse = null;
                            }
                        }
                        if (GlobalData.SaturnAPIResponse != null)
                        {
                            if (SaturnAPI.CurrentServiceID == ((int)SaturnAPI.SaturnRequestService.Cancel).ToString())
                            {
                                if (GlobalData.SaturnAPIResponse.controlInfo.procResult != "0" && GlobalData.SaturnAPIResponse.controlInfo.errorCode != "TFWL0002")
                                {
                                    msgCode = "E0005";
                                    FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                    {
                                        MessageMaintenance = msgMode.msg2.Replace("[エラーコード詳細1]", $"{GlobalData.SaturnAPIResponse.controlInfo.errorCodeDetail1}")
                                                                         .Replace("[エラーコード詳細2]", $"{GlobalData.SaturnAPIResponse.controlInfo.errorCodeDetail2}"),
                                    };
                                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties
                                    {
                                        MessageMaintenance = msgMode.msg3,
                                    };
                                    Utilities.Log.Error("Failed to abort operation, response is abnormal! procResult: " + GlobalData.SaturnAPIResponse.controlInfo.procResult
                                        + ", errorCodeDetail1: " + GlobalData.SaturnAPIResponse.controlInfo.errorCodeDetail1
                                        + ", errorCodeDetail2: " + GlobalData.SaturnAPIResponse.controlInfo.errorCodeDetail2 + "!");
                                    GlobalData.MsgCode = msgCode;
                                    Session.ScreenState.NextState = StateMachine.State.emErrorToMaintenance;
                                    Session.TimerCount = 0;
                                }
                                else if (GlobalData.SaturnAPIResponse.controlInfo.procResult == "9" && GlobalData.SaturnAPIResponse.controlInfo.errorCode == "TFWL0002")
                                {
                                    Utilities.Log.Info("Abort operation successful!");
                                    GlobalData.TransactionErrorType = Utilities.ErrorType.ProcessingNotCompleted;
                                    Session.ScreenState.NextState = StateMachine.State.emPaymentResult;
                                    Session.TimerCountRemaining = Session.TimerCount;
                                    Session.TimerCount = 0;
                                }
                                else if (GlobalData.SaturnAPIResponse.controlInfo.procResult == "0" && GlobalData.SaturnAPIResponse.bizInfo.posWaitingStatus != "2")
                                {
                                    msgCode = "E0006";
                                    FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                    {
                                        MessageMaintenance = msgMode.msg2.Replace("[Arch状態]", $"{GlobalData.SaturnAPIResponse.bizInfo.posWaitingStatus}"),
                                    };
                                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties
                                    {
                                        MessageMaintenance = msgMode.msg3,
                                    };
                                    Utilities.Log.Error("Failed to abort operation, Arch status is not in operating state, posWaitingStatus: " + GlobalData.SaturnAPIResponse.bizInfo.posWaitingStatus + "!");
                                    GlobalData.MsgCode = msgCode;
                                    Session.ScreenState.NextState = StateMachine.State.emErrorToMaintenance;
                                    Session.TimerCount = 0;
                                }
                                else // if (GlobalData.SaturnAPIResponse.controlInfo.procResult == "0" && GlobalData.SaturnAPIResponse.bizInfo.posWaitingStatus == "2")
                                {
                                    msgCode = "E0020";
                                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties();
                                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                                    Utilities.Log.Error("Failed to abort operation, Arch status is in operating state, posWaitingStatus: " + GlobalData.SaturnAPIResponse.bizInfo.posWaitingStatus + "!");
                                    Session.MainViewModel.LoadScreen_emMessage(msgCode);
                                    SaturnAPI.CurrentServiceID = service.service_id;
                                    GlobalData.SaturnAPIResponse = null;
                                    isFromCancelState = true;
                                }
                            }
                            else if (GlobalData.SaturnAPIResponse.controlInfo.service == ((int)SaturnAPI.SaturnRequestService.StatusNotification).ToString())
                            {
                                if (isFromCancelState)
                                {
                                    msgCode = "E0022";
                                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties();
                                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                                    Session.MainViewModel.LoadScreen_emMessage(msgCode);
                                    await Task.Delay((int)GlobalData.BasicConfig.settlement_close_time_01 * 1000);
                                    isFromCancelState = false;

                                    msgCode = "E0007";
                                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties();
                                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                                    GlobalData.ViewModelProperties.BtnVisibility = "Visible";
                                    GlobalData.ViewModelProperties.BtnContent = "決済\n中止";
                                    GlobalData.ViewModelProperties.BtnCommand = new Command(CancelFromPayment);
                                    Session.MainViewModel.LoadScreen_emProcessOperation(msgCode);
                                }
                                FileStruct.MsgDetail msg = GlobalData.MsgConditionConfig.Find(m => m.msg_code == GlobalData.SaturnAPIResponse.bizInfo.kbn);
                                if (GlobalData.CurrentStatusNotificationCode != GlobalData.SaturnAPIResponse.bizInfo.kbn)
                                {
                                    GlobalData.CurrentStatusNotificationCode = GlobalData.SaturnAPIResponse.bizInfo.kbn;
                                    if (msg != null && GlobalData.SaturnAPIResponse.bizInfo.kbn != "C1")
                                    {
                                        if (GlobalData.IsPopupNotificationOpen == false)
                                        {
                                            GlobalData.IsPopupNotificationOpen = true;
                                            Application.Current.Dispatcher.Invoke(() =>
                                            {
                                                GlobalData.mainWindow.PopupNotification.IsOpen = GlobalData.IsPopupNotificationOpen;
                                            }, DispatcherPriority.ContextIdle);
                                        }
                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            GlobalData.mainWindow.MessageNotification.Text = msg.msg;
                                        }, DispatcherPriority.ContextIdle);
                                    }
                                    else
                                    {
                                        if (GlobalData.IsPopupNotificationOpen == true)
                                        {
                                            GlobalData.IsPopupNotificationOpen = false;
                                            Application.Current.Dispatcher.Invoke(() =>
                                            {
                                                GlobalData.mainWindow.PopupNotification.IsOpen = GlobalData.IsPopupNotificationOpen;
                                                GlobalData.mainWindow.MessageNotification.Text = "";
                                            }, DispatcherPriority.ContextIdle);
                                        }
                                    }
                                }
                            }
                            else if (GlobalData.SaturnAPIResponse.controlInfo.service == ((int)SaturnAPI.SaturnRequestService.HealthCheck).ToString())
                            {
                                if (GlobalData.SaturnAPIResponse.controlInfo.procResult != "0")
                                {
                                    msgCode = "E0009";
                                    FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                    {
                                        MessageMaintenance = msgMode.msg2
                                                                    .Replace("[エラーコード詳細1]", $"{GlobalData.SaturnAPIResponse.controlInfo.errorCodeDetail1}")
                                                                    .Replace("[エラーコード詳細2]", $"{GlobalData.SaturnAPIResponse.controlInfo.errorCodeDetail2}"),
                                    };
                                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties
                                    {
                                        MessageMaintenance = msgMode.msg3,
                                    };
                                }
                                else if (GlobalData.SaturnAPIResponse.bizInfo.posWaitingStatus != "1")
                                {
                                    msgCode = "E0010";
                                    FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                    {
                                        MessageMaintenance = msgMode.msg2.Replace("[Arch状態]", $"{GlobalData.SaturnAPIResponse.bizInfo.posWaitingStatus}"),
                                    };
                                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties
                                    {
                                        MessageMaintenance = msgMode.msg3,
                                    };
                                }
                                GlobalData.MsgCode = msgCode;
                                Session.ScreenState.NextState = StateMachine.State.emErrorToMaintenance;
                                Session.TimerCount = 0;
                            }
                            else if (GlobalData.SaturnAPIResponse.controlInfo.service == SaturnAPI.CurrentServiceID)
                            {
                                if (isFromCancelState)
                                {
                                    msgCode = "E0021";
                                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties();
                                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                                    Session.MainViewModel.LoadScreen_emMessage(msgCode);
                                    await Task.Delay((int)GlobalData.BasicConfig.settlement_close_time_01 * 1000);
                                    isFromCancelState = false;
                                }
                                if (GlobalData.SaturnAPIResponse.controlInfo.procResult == "0")
                                {
                                    var (printReceiptData, saveReceiptData) = Utilities.PreparePaymentReceiptData(GlobalData.SaturnAPIResponse);
                                    string printDataPath = GlobalData.PosPath + @"\" + GlobalData.ServiceName + "_" + GlobalData.SaturnAPIResponse.bizInfo.slipNo + ".dat";
                                    if (printReceiptData.Count != 0)
                                    {
                                        if (GlobalData.BasicConfig.selling_receipt_print_control_setting == "0" && GlobalData.SaturnAPIResponse.bizInfo.tradeStatus != "02")
                                        {
                                            if (!FileStruct.CsFileRead(GlobalData.AppPath, printDataPath, ref GlobalData.SalesReceiptData))
                                            {
                                                GlobalData.SalesReceiptData = new List<FileStruct.RowData>();
                                            }
                                            GlobalData.SalesReceiptData.AddRange(saveReceiptData);
                                            FileStruct.CsFileWrite(GlobalData.AppPath, printDataPath, GlobalData.SalesReceiptData);
                                            Utilities.Log.Info("Save sales receipt data to file!");
                                        }
                                        else
                                        {
                                            // Connect to the printer
                                            PrintAPI.GetPrinter(GlobalData.PrinterData.device_name);
                                            PrintAPI.OpenConnection();

                                            // Check printer status
                                            bool isPrinterEnabled = PrintAPI.Printer != null && PrintAPI.Printer.DeviceEnabled;
                                            bool isPrinterReady = true;
                                            if (!isPrinterEnabled)
                                            {
                                                msgCode = "B0003";
                                                Utilities.Log.Error("▲ Printer is not available!");
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    Utilities.PrintReceipt(printReceiptData);
                                                }
                                                catch (PosControlException ex)
                                                {
                                                    Utilities.Log.Error($"▲ Print error occurred! PosControlException ErrorCode: {ex.ErrorCode}, ErrorCodeExtended: {ex.ErrorCodeExtended}");
                                                    if (ex.ErrorCode == ErrorCode.Extended && ex.ErrorCodeExtended == 201)
                                                    {
                                                        msgCode = "B0002";
                                                        Utilities.Log.Error("▲ Printer Cover is open!");
                                                    }
                                                    else if (ex.ErrorCode == ErrorCode.Extended && ex.ErrorCodeExtended == 203)
                                                    {
                                                        msgCode = "B0001";
                                                        Utilities.Log.Error("▲ Printer runs out of receipt paper!");
                                                    }
                                                    else
                                                    {
                                                        msgCode = "B0003";
                                                        Utilities.Log.Error("▲ Printer is not available!");
                                                    }
                                                    GlobalData.MPErrorCode = ex.ErrorCode;
                                                    GlobalData.MPErrorCodeExtended = ex.ErrorCodeExtended;
                                                    isPrinterReady = false;
                                                }
                                            }
                                            if (!isPrinterEnabled || !isPrinterReady)
                                            {
                                                isError = true;
                                                FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                                {
                                                    MessageMaintenance = msgMode.msg2,
                                                };
                                                GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties
                                                {
                                                    MessageMaintenance = msgMode.msg3,
                                                };
                                                GlobalData.MsgCode = msgCode;
                                                Session.ScreenState.NextState = StateMachine.State.emErrorToMaintenance;
                                                Session.TimerCount = 0;
                                            }
                                        }
                                    }
                                    if (saveReceiptData.Count != 0 && !isError)
                                    {
                                        if (!FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.JournalPath, ref GlobalData.JournalData))
                                        {
                                            GlobalData.JournalData = new List<FileStruct.RowData>();
                                        }
                                        GlobalData.JournalData.AddRange(saveReceiptData);
                                        FileStruct.CsFileWrite(GlobalData.AppPath, GlobalData.JournalPath, GlobalData.JournalData);
                                        Utilities.Log.Info("Save printed receipt data to journal file!");
                                    }
                                    if (!isError && (GlobalData.SaturnAPIResponse.bizInfo.tradeStatus == "01" || string.IsNullOrEmpty(GlobalData.SaturnAPIResponse.bizInfo.tradeStatus)))
                                    {
                                        var sumary = GlobalData.DailyTotalData.brand.Find(b => b.brand_code == GlobalData.ServiceName);
                                        if (sumary != null)
                                        {
                                            decimal.TryParse(GlobalData.SaturnAPIResponse.bizInfo.tradeAmount, out decimal tradeAmount);
                                            sumary.payment_count++;
                                            sumary.payment_amount += tradeAmount;
                                        }
                                        FileStruct.CsFileWrite(GlobalData.AppPath, GlobalData.DailyTotalPath, GlobalData.DailyTotalData);
                                        if (File.Exists(GlobalData.AppPath + GlobalData.LastSettlementBrandCodePath))
                                        {
                                            File.Delete(GlobalData.AppPath + GlobalData.LastSettlementBrandCodePath);
                                            Utilities.Log.Info("Delete " + GlobalData.LastSettlementBrandCodePath);
                                        }
                                        if (File.Exists(GlobalData.AppPath + GlobalData.LastSettlementSeqNoPath))
                                        {
                                            File.Delete(GlobalData.AppPath + GlobalData.LastSettlementSeqNoPath);
                                            Utilities.Log.Info("Delete " + GlobalData.LastSettlementSeqNoPath);
                                        }
                                        GlobalData.TransactionErrorType = Utilities.ErrorType.Success;
                                        Session.ScreenState.NextState = StateMachine.State.emPaymentResult;
                                        Session.TimerCount = 0;
                                    }
                                    else if (!isError && GlobalData.SaturnAPIResponse.bizInfo.tradeStatus == "02")
                                    {
                                        msgCode = "E0011";
                                        FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                        GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                        {
                                            MessageMaintenance = msgMode.msg2,
                                        };
                                        GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties
                                        {
                                            MessageMaintenance = msgMode.msg3,
                                        };
                                        GlobalData.MsgCode = msgCode;
                                        Session.ScreenState.NextState = StateMachine.State.emErrorToMaintenance;
                                        Session.TimerCount = 0;
                                    }
                                }
                                switch (GlobalData.ServiceName)
                                {
                                    case "Suica":
                                        if (GlobalData.SaturnAPIResponse.controlInfo.procResult == "1")
                                        {
                                            msgCode = "E0011";
                                            FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                            GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                            {
                                                MessageMaintenance = msgMode.msg2,
                                            };
                                            GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties
                                            {
                                                MessageMaintenance = msgMode.msg3,
                                            };
                                            GlobalData.MsgCode = msgCode;
                                            Session.ScreenState.NextState = StateMachine.State.emErrorToMaintenance;
                                            Session.TimerCount = 0;
                                        }
                                        else if (GlobalData.SaturnAPIResponse.controlInfo.procResult == "6")
                                        {
                                            msgCode = "E0012";
                                            GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                            {
                                                BtnVisibility = "Visible",
                                                BtnContent = "支払選択に戻る",
                                                BtnCommand = new Command(BackToPayment),
                                            };
                                            GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                                            Session.MainViewModel.LoadScreen_emMessage(msgCode);
                                            Session.TimerCount = 0;
                                        }
                                        else if (GlobalData.SaturnAPIResponse.controlInfo.procResult == "9")
                                        {
                                            string errorCodeDetail1 = GlobalData.SaturnAPIResponse.controlInfo.errorCodeDetail1;
                                            FileStruct.Msg_Brand msg_Brand = FileStruct.Find(GlobalData.MsgBrandConfig, b => b.brand_code == GlobalData.ServiceName, GlobalData.ServiceName);
                                            FileStruct.MsgDetail msg = FileStruct.Find(msg_Brand.message, m => m.msg_code == errorCodeDetail1);
                                            if (errorCodeDetail1 == "TKOC0586")
                                            {
                                                msgCode = "E0019";
                                                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                                {
                                                    BtnVisibility = "Visible",
                                                    BtnContent = "支払選択に戻る",
                                                    BtnCommand = new Command(BackToPayment),
                                                    Btn2ndVisibility = "Visible",
                                                    Btn2ndContent = "残高照会に進む",
                                                    Btn2ndCommand = new Command(GoToBalanceInquiryOperation),
                                                };
                                                GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                                                Session.MainViewModel.LoadScreen_emMessage(msgCode);
                                                Session.TimerCount = 0;
                                            }
                                            else if (msg.type == "0" || msg.type == "3")
                                            {
                                                msgCode = "E0013";
                                                FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                                {
                                                    Message = msgMode.msg1.Replace("[エラーメッセージ]", msg.msg),
                                                    BtnVisibility = "Visible",
                                                    BtnContent = "支払選択に戻る",
                                                    BtnCommand = new Command(BackToPayment),
                                                };
                                                GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties
                                                {
                                                    Message = msgMode.msg3.Replace("[エラーメッセージ]", msg.msg),
                                                };
                                                Session.MainViewModel.LoadScreen_emMessage(msgCode);
                                                Session.TimerCount = 0;
                                            }
                                            else
                                            {
                                                msgCode = "E0014";
                                                FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                                {
                                                    MessageMaintenance = msgMode.msg2.Replace("[エラーコード]", errorCodeDetail1),
                                                };
                                                GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties
                                                {
                                                    MessageMaintenance = msgMode.msg3,
                                                };
                                                GlobalData.MsgCode = msgCode;
                                                Session.ScreenState.NextState = StateMachine.State.emErrorToMaintenance;
                                                Session.TimerCount = 0;
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        else
                        {
                            GlobalData.CurrentStatusNotificationCode = "";
                            if (GlobalData.IsPopupNotificationOpen == true)
                            {
                                GlobalData.IsPopupNotificationOpen = false;
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    GlobalData.mainWindow.PopupNotification.IsOpen = GlobalData.IsPopupNotificationOpen;
                                    GlobalData.mainWindow.MessageNotification.Text = "";
                                }, DispatcherPriority.ContextIdle);
                            }
                        }
                        await Task.Delay(100, cancellationToken); // 100ms is for Task.Delay() to be more precise
                        Session.TimerCount -= 100;
                    }
                }
                else if (Session.ScreenState.PreviousStates[0] == StateMachine.State.emBalanceInquiry)
                {
                    // Utilities.PlaySound("emtm2.wav", isPlayOnce: playSoundOnce);
                    msgCode = "F0003";
                    GlobalData.ViewModelProperties.BtnVisibility = "Visible";
                    GlobalData.ViewModelProperties.BtnContent = "残高照会\n中止";
                    GlobalData.ViewModelProperties.BtnCommand = new Command(CancelFromBalanceInquiry);
                    Session.MainViewModel.LoadScreen_emProcessOperation(msgCode);

                    FileStruct.ServiceId service = GlobalData.ServiceIdConfig.Find(m => m.brand_code == GlobalData.ServiceName);
                    SaturnAPI.BalanceInquiryRequest(GlobalData.BasicConfig.operation_mode, service.service_id);
                    await Task.Delay(500);

                    msgCode = "F0009";
                    Session.MainViewModel.LoadScreen_emProcessOperation(msgCode);

                    GlobalData.CurrentServiceID = ((int)SaturnAPI.SaturnRequestService.StatusNotification).ToString();
                    Session.TimerCount = (int)GlobalData.BasicConfig.settlement_request_timeout * 1000;

                    bool isFromCancelState = false;
                    while (Session.TimerCount > 0)
                    {
                        if (Session.TimerCount <= 100)
                        {
                            if (GlobalData.TransactionErrorType != Utilities.ErrorType.Timeout)
                            {
                                msgCode = "F0014";
                                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties();
                                GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                                Session.MainViewModel.LoadScreen_emMessage(msgCode);

                                Utilities.Log.Info("Cancel request is sent!");
                                SaturnAPI.CancelRequest();
                                Session.TimerCount = (int)GlobalData.BasicConfig.settlement_cancel_timeout * 1000;
                                GlobalData.TransactionErrorType = Utilities.ErrorType.Timeout;
                            }
                            else
                            {
                                msgCode = "F0006";
                                FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                {
                                    MessageMaintenance = msgMode.msg2,
                                };
                                GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties
                                {
                                    MessageMaintenance = msgMode.msg3,
                                };
                                Utilities.Log.Error("Failed to abort operation!");
                                GlobalData.MsgCode = msgCode;
                                Session.ScreenState.NextState = StateMachine.State.emErrorToMaintenance;
                                GlobalData.SaturnAPIResponse = null;
                            }
                        }
                        if (GlobalData.SaturnAPIResponse != null)
                        {
                            if (SaturnAPI.CurrentServiceID == ((int)SaturnAPI.SaturnRequestService.Cancel).ToString())
                            {
                                if (GlobalData.SaturnAPIResponse.controlInfo.procResult != "0" && GlobalData.SaturnAPIResponse.controlInfo.errorCode != "TFWL0002")
                                {
                                    msgCode = "F0007";
                                    FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                    {
                                        MessageMaintenance = msgMode.msg2.Replace("[エラーコード詳細1]", $"{GlobalData.SaturnAPIResponse.controlInfo.errorCodeDetail1}")
                                                                         .Replace("[エラーコード詳細2]", $"{GlobalData.SaturnAPIResponse.controlInfo.errorCodeDetail2}"),
                                    };
                                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties
                                    {
                                        MessageMaintenance = msgMode.msg3,
                                    };
                                    Utilities.Log.Error("Failed to abort operation, response is abnormal! procResult: " + GlobalData.SaturnAPIResponse.controlInfo.procResult
                                        + ", errorCodeDetail1: " + GlobalData.SaturnAPIResponse.controlInfo.errorCodeDetail1
                                        + ", errorCodeDetail2: " + GlobalData.SaturnAPIResponse.controlInfo.errorCodeDetail2 + "!");
                                    GlobalData.MsgCode = msgCode;
                                    Session.ScreenState.NextState = StateMachine.State.emErrorToMaintenance;
                                    Session.TimerCount = 0;
                                }
                                else if (GlobalData.SaturnAPIResponse.controlInfo.procResult == "9" && GlobalData.SaturnAPIResponse.controlInfo.errorCode == "TFWL0002")
                                {
                                    Utilities.Log.Info("Abort operation successful!");
                                    GlobalData.TransactionErrorType = Utilities.ErrorType.ProcessingNotCompleted;
                                    Session.ScreenState.NextState = StateMachine.State.emBalanceInquiryResult;
                                    Session.TimerCountRemaining = Session.TimerCount;
                                    Session.TimerCount = 0;
                                }
                                else if (GlobalData.SaturnAPIResponse.controlInfo.procResult == "0" && GlobalData.SaturnAPIResponse.bizInfo.posWaitingStatus != "2")
                                {
                                    msgCode = "F0008";
                                    FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                    {
                                        MessageMaintenance = msgMode.msg2.Replace("[Arch状態]", $"{GlobalData.SaturnAPIResponse.bizInfo.posWaitingStatus}"),
                                    };
                                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties
                                    {
                                        MessageMaintenance = msgMode.msg3,
                                    };
                                    Utilities.Log.Error("Failed to abort operation, Arch status is not in operating state, posWaitingStatus: " + GlobalData.SaturnAPIResponse.bizInfo.posWaitingStatus + "!");
                                    GlobalData.MsgCode = msgCode;
                                    Session.ScreenState.NextState = StateMachine.State.emErrorToMaintenance;
                                    Session.TimerCount = 0;
                                }
                                else // if (GlobalData.SaturnAPIResponse.controlInfo.procResult == "0" && GlobalData.SaturnAPIResponse.bizInfo.posWaitingStatus == "2")
                                {
                                    msgCode = "F0021";
                                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties();
                                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                                    Utilities.Log.Error("Failed to abort operation, Arch status is in operating state, posWaitingStatus: " + GlobalData.SaturnAPIResponse.bizInfo.posWaitingStatus + "!");
                                    Session.MainViewModel.LoadScreen_emMessage(msgCode);
                                    SaturnAPI.CurrentServiceID = service.service_id;
                                    GlobalData.SaturnAPIResponse = null;
                                    isFromCancelState = true;
                                }
                            }
                            else if (GlobalData.SaturnAPIResponse.controlInfo.service == ((int)SaturnAPI.SaturnRequestService.StatusNotification).ToString())
                            {
                                if (isFromCancelState)
                                {
                                    msgCode = "F0023";
                                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties();
                                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                                    Session.MainViewModel.LoadScreen_emMessage(msgCode);
                                    await Task.Delay((int)GlobalData.BasicConfig.settlement_close_time_01 * 1000);
                                    isFromCancelState = false;

                                    msgCode = "F0009";
                                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties();
                                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                                    GlobalData.ViewModelProperties.BtnVisibility = "Visible";
                                    GlobalData.ViewModelProperties.BtnContent = "残高照会\n中止";
                                    GlobalData.ViewModelProperties.BtnCommand = new Command(CancelFromBalanceInquiry);
                                    Session.MainViewModel.LoadScreen_emProcessOperation(msgCode);
                                }
                                FileStruct.MsgDetail msg = GlobalData.MsgConditionConfig.Find(m => m.msg_code == GlobalData.SaturnAPIResponse.bizInfo.kbn);
                                if (GlobalData.CurrentStatusNotificationCode != GlobalData.SaturnAPIResponse.bizInfo.kbn)
                                {
                                    GlobalData.CurrentStatusNotificationCode = GlobalData.SaturnAPIResponse.bizInfo.kbn;
                                    if (msg != null && GlobalData.SaturnAPIResponse.bizInfo.kbn != "C1")
                                    {
                                        if (GlobalData.IsPopupNotificationOpen == false)
                                        {
                                            GlobalData.IsPopupNotificationOpen = true;
                                            Application.Current.Dispatcher.Invoke(() =>
                                            {
                                                GlobalData.mainWindow.PopupNotification.IsOpen = GlobalData.IsPopupNotificationOpen;
                                            }, DispatcherPriority.ContextIdle);
                                        }
                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            GlobalData.mainWindow.MessageNotification.Text = msg.msg;
                                        }, DispatcherPriority.ContextIdle);
                                    }
                                    else
                                    {
                                        if (GlobalData.IsPopupNotificationOpen == true)
                                        {
                                            GlobalData.IsPopupNotificationOpen = false;
                                            Application.Current.Dispatcher.Invoke(() =>
                                            {
                                                GlobalData.mainWindow.PopupNotification.IsOpen = GlobalData.IsPopupNotificationOpen;
                                                GlobalData.mainWindow.MessageNotification.Text = "";
                                            }, DispatcherPriority.ContextIdle);
                                        }
                                    }
                                }
                            }
                            else if (GlobalData.SaturnAPIResponse.controlInfo.service == ((int)SaturnAPI.SaturnRequestService.HealthCheck).ToString())
                            {
                                if (GlobalData.SaturnAPIResponse.controlInfo.procResult != "0")
                                {
                                    msgCode = "F0011";
                                    FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                    {
                                        MessageMaintenance = msgMode.msg2
                                                                    .Replace("[エラーコード詳細1]", $"{GlobalData.SaturnAPIResponse.controlInfo.errorCodeDetail1}")
                                                                    .Replace("[エラーコード詳細2]", $"{GlobalData.SaturnAPIResponse.controlInfo.errorCodeDetail2}"),
                                    };
                                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties
                                    {
                                        MessageMaintenance = msgMode.msg3,
                                    };
                                }
                                else if (GlobalData.SaturnAPIResponse.bizInfo.posWaitingStatus != "1")
                                {
                                    msgCode = "F0012";
                                    FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                    {
                                        MessageMaintenance = msgMode.msg2.Replace("[Arch状態]", $"{GlobalData.SaturnAPIResponse.bizInfo.posWaitingStatus}"),
                                    };
                                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties
                                    {
                                        MessageMaintenance = msgMode.msg3,
                                    };
                                }
                                GlobalData.MsgCode = msgCode;
                                Session.ScreenState.NextState = StateMachine.State.emErrorToMaintenance;
                                Session.TimerCount = 0;
                            }
                            else if (GlobalData.SaturnAPIResponse.controlInfo.service == SaturnAPI.CurrentServiceID)
                            {
                                if (isFromCancelState)
                                {
                                    msgCode = "F0022";
                                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties();
                                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                                    Session.MainViewModel.LoadScreen_emMessage(msgCode);
                                    await Task.Delay((int)GlobalData.BasicConfig.settlement_close_time_01 * 1000);
                                    isFromCancelState = false;
                                }
                                switch (GlobalData.ServiceName)
                                {
                                    case "Suica":
                                        if (GlobalData.SaturnAPIResponse.controlInfo.procResult == "0")
                                        {
                                            GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                            {
                                                BtnVisibility = "Visible",
                                                Btn2ndVisibility = "Visible",
                                                Btn2ndCommand = new Command(BackToPayment),
                                            };
                                            GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                                            decimal.TryParse(GlobalData.SaturnAPIResponse.bizInfo.cardBalance, out decimal cardBalance);
                                            decimal.TryParse(GlobalData.Data_FromPosDat.amount, out decimal paymentAmount);
                                            if (cardBalance >= paymentAmount)
                                            {
                                                msgCode = "F0016";
                                                GlobalData.ViewModelProperties.Btn2ndContent = "お支払いに進む";
                                                GlobalData.ViewModelProperties.Btn2ndVisibility = "Visible";
                                                GlobalData.ViewModelProperties.Btn2ndCommand = new Command(GoToPaymentOperation);
                                            }
                                            else if (cardBalance == 0)
                                            {
                                                msgCode = "F0017";
                                                GlobalData.ViewModelProperties.Btn2ndVisibility = "Hidden";
                                                GlobalData.ViewModelProperties.Btn2ndVisibility = "Collapsed";
                                            }
                                            else if (cardBalance < paymentAmount)
                                            {
                                                msgCode = "F0018";
                                                GlobalData.ViewModelProperties.Btn2ndContent = "残高分を使う";
                                                GlobalData.ViewModelProperties.Btn2ndVisibility = "Visible";
                                                GlobalData.ViewModelProperties.Btn2ndCommand = new Command(GoToPaymentOperationWithInsufficientBalance);
                                            }
                                            GlobalData.ViewModelProperties.MessageVisibility = "Visible";

                                            GlobalData.TransactionErrorType = Utilities.ErrorType.Success;
                                            GlobalData.MsgCode = msgCode;
                                            Session.ScreenState.NextState = StateMachine.State.emBalanceInquiryResult;
                                            Session.TimerCountRemaining = Session.TimerCount;
                                            Session.TimerCount = 0;
                                        }
                                        else if (GlobalData.SaturnAPIResponse.controlInfo.procResult == "1")
                                        {
                                            msgCode = "F0013";
                                            FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                            GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                            {
                                                BtnVisibility = "Visible",
                                                BtnContent = "電子マネー選択に戻る",
                                                BtnCommand = new Command(BackToPayment),
                                            };
                                            GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                                            GlobalData.TransactionErrorType = Utilities.ErrorType.PrintReceipt;
                                            Session.ScreenState.SetPreState(StateMachine.State.emBalanceInquiry);
                                            GlobalData.MsgCode = msgCode;
                                            Session.ScreenState.NextState = StateMachine.State.emErrorToMaintenance;
                                            Session.TimerCount = 0;
                                        }
                                        else if (GlobalData.SaturnAPIResponse.controlInfo.procResult == "9")
                                        {
                                            string errorCodeDetail1 = GlobalData.SaturnAPIResponse.controlInfo.errorCodeDetail1;
                                            FileStruct.Msg_Brand msg_Brand = FileStruct.Find(GlobalData.MsgBrandConfig, b => b.brand_code == GlobalData.ServiceName, GlobalData.ServiceName);
                                            FileStruct.MsgDetail msg = FileStruct.Find(msg_Brand.message, m => m.msg_code == errorCodeDetail1);
                                            if (msg.type == "0" || msg.type == "3")
                                            {
                                                msgCode = "F0015";
                                                FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                                {
                                                    Message = msgMode.msg1.Replace("[エラーメッセージ]", msg.msg),
                                                    BtnVisibility = "Visible",
                                                    BtnContent = "支払選択に戻る",
                                                    BtnCommand = new Command(BackToPayment),
                                                };
                                                GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties
                                                {
                                                    Message = msgMode.msg3.Replace("[エラーメッセージ]", msg.msg),
                                                };
                                                Session.MainViewModel.LoadScreen_emMessage(msgCode);
                                                Session.TimerCount = 0;
                                            }
                                            else
                                            {
                                                msgCode = "F0019";
                                                FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                                {
                                                    BtnVisibility = "Visible",
                                                    BtnContent = "電子マネー選択に戻る",
                                                    BtnCommand = new Command(GoToBalanceInquirySelection)
                                                };
                                                GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                                                GlobalData.TransactionErrorType = Utilities.ErrorType.PrintReceipt;
                                                Session.ScreenState.SetPreState(StateMachine.State.emBalanceInquiry);
                                                GlobalData.MsgCode = msgCode;
                                                Session.ScreenState.NextState = StateMachine.State.emErrorToMaintenance;
                                                Session.TimerCount = 0;
                                            }
                                        }
                                        break;
                                    default:
                                        if (GlobalData.SaturnAPIResponse.controlInfo.procResult == "0")
                                        {
                                            Session.ScreenState.NextState = StateMachine.State.emBalanceInquiryResult;
                                            Session.TimerCountRemaining = Session.TimerCount;
                                            Session.TimerCount = 0;
                                        }
                                        break;
                                }
                            }
                        }
                        else
                        {
                            GlobalData.CurrentStatusNotificationCode = "";
                            if (GlobalData.IsPopupNotificationOpen == true)
                            {
                                GlobalData.IsPopupNotificationOpen = false;
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    GlobalData.mainWindow.PopupNotification.IsOpen = GlobalData.IsPopupNotificationOpen;
                                    GlobalData.mainWindow.MessageNotification.Text = "";
                                }, DispatcherPriority.ContextIdle);
                            }
                        }
                        await Task.Delay(100, cancellationToken); // 100ms is for Task.Delay() to be more precise
                        Session.TimerCount -= 100;
                    }
                }
                GlobalData.CurrentStatusNotificationCode = "";
                if (GlobalData.IsPopupNotificationOpen == true)
                {
                    GlobalData.IsPopupNotificationOpen = false;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GlobalData.mainWindow.PopupNotification.IsOpen = GlobalData.IsPopupNotificationOpen;
                        GlobalData.mainWindow.MessageNotification.Text = "";
                    }, DispatcherPriority.ContextIdle);
                }
            }

            return Session.ScreenState.GoToNextState(this);
        }

        public void CancelFromPayment()
        {
            string msgCode = "E0002";
            GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties();
            GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
            Session.MainViewModel.LoadScreen_emMessage(msgCode);

            Utilities.Log.Info("Cancel request is sent!");
            SaturnAPI.CancelRequest();
            Session.TimerCount = (int)GlobalData.BasicConfig.settlement_cancel_timeout * 1000;
            GlobalData.TransactionErrorType = Utilities.ErrorType.Timeout;
        }
        public void CancelFromBalanceInquiry()
        {
            string msgCode = "F0004";
            GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties();
            GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
            Session.MainViewModel.LoadScreen_emMessage(msgCode);

            Utilities.Log.Info("Cancel request is sent!");
            SaturnAPI.CancelRequest();
            Session.TimerCount = (int)GlobalData.BasicConfig.settlement_cancel_timeout * 1000;
            GlobalData.TransactionErrorType = Utilities.ErrorType.Timeout;
        }
        public void BackToPayment()
        {
            Session.ScreenState.NextState = StateMachine.State.emPayment;
            Session.TimerCount = 0;
        }
        public void GoToBalanceInquiryOperation()
        {
            Session.ScreenState.SetPreState(StateMachine.State.emBalanceInquiry);
            Session.ScreenState.CurrentState = StateMachine.State.emBalanceInquiry;
            Session.TimerCount = 0;
        }
        public void GoToBalanceInquirySelection()
        {
            Session.ScreenState.SetPreState(StateMachine.State.emBalanceInquiry);
            Session.ScreenState.NextState = StateMachine.State.emBalanceInquiry;
            Session.TimerCount = 0;
        }
        public void GoToPaymentOperation()
        {
            Session.ScreenState.SetPreState(StateMachine.State.emPayment);
            Session.ScreenState.NextState = StateMachine.State.emProcessOperation;
            Session.TimerCount = 0;
        }
        public void GoToPaymentOperationWithInsufficientBalance()
        {
            decimal.TryParse(GlobalData.SaturnAPIResponse.bizInfo.cardBalance, out decimal cardBalance);
            GlobalData.TotalPayment = cardBalance;
            Session.ScreenState.SetPreState(StateMachine.State.emPayment);
            Session.ScreenState.NextState = StateMachine.State.emProcessOperation;
            Session.TimerCount = 0;
        }
    }
}
