using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;
using WPSaturnEMoney.ViewModels;

namespace WPSaturnEMoney.State
{
    class State_emStandBy : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(100, cancellationToken);
                bool isHealthCheckOK = false;
                bool isBrandNameOK = false;
                bool is_btn_order_SubtractValue_OK = false;
                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties();
                GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                if (Session.ScreenState.CurrentState != StateMachine.State.emStandBy)
                {
                    Session.ScreenState.CurrentState = StateMachine.State.emStandBy;
                    GlobalData.TransactionErrorType = Utilities.ErrorType.Success;
                    string msgCode = "D0001";
                    Session.MainViewModel.LoadScreen_emMessage(msgCode);

                    // End of standby maintenance (after clicking "終了" on emMaintenanceMenu screen)
                    if (Session.MaintenanceMode == "Maintenance")
                    {
                        Session.MaintenanceMode = "";
                        GlobalData.Data_ToPosDat.service = GlobalData.Data_FromPosDat.service;
                        GlobalData.Data_ToPosDat.sequence = GlobalData.Data_FromPosDat.sequence;
                        GlobalData.Data_ToPosDat.last_operator = GlobalData.LastOperator;
                        GlobalData.Data_ToPosDat.result = "0";
                        GlobalData.Data_ToPosDat.SettledAmount = "";
                        GlobalData.Data_ToPosDat.CurrentService = "";
                        GlobalData.Data_ToPosDat.statementID = "";

                        await WinAPI.EndTransaction();
                        return Session.ScreenState.GoToNextState(this);
                    }

                    // Wait for start transaction trigger
                    int timeout = 5000; // ms
                    bool startTransaction = false;
                    while (timeout >= 100)
                    {
                        if (Session.IsTransactionStart)
                        {
                            startTransaction = true;
                            Session.IsTransactionStart = false;
                            break;
                        }
                        await Task.Delay(100);
                        timeout -= 100;
                    }

                    if (startTransaction)
                    {
                        Utilities.Log.Info("Health Check is started!");
                        FileStruct.SaturnAPIResponse result = await SaturnAPI.HealthCheck();
                        if (result == null)
                        {
                            if (GlobalData.Data_FromPosDat.location == "0")
                            {
                                msgCode = "A0003";
                                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                {
                                    BtnVisibility = "Visible",
                                    BtnContent = "終了",
                                    BtnCommand = new Command(ErrorEnd),
                                };
                                GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                            }
                            else
                            {
                                msgCode = "D0004";
                                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties();
                                GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                            }
                            Utilities.Log.Error("Health Check is done, timeout when waiting to receive the response!");
                        }
                        else if (result.controlInfo.procResult != "0")
                        {
                            if (GlobalData.Data_FromPosDat.location == "0")
                            {
                                msgCode = "A0004";
                                FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                {
                                    Message = msgMode.msg1
                                                    .Replace("[エラーコード詳細1]", $"{result.controlInfo.errorCodeDetail1}")
                                                    .Replace("[エラーコード詳細2]", $"{result.controlInfo.errorCodeDetail2}"),
                                    BtnVisibility = "Visible",
                                    BtnContent = "終了",
                                    BtnCommand = new Command(ErrorEnd),
                                };
                                GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                            }
                            else
                            {
                                msgCode = "D0002";
                                FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                {
                                    MessageMaintenance = msgMode.msg2
                                                                .Replace("[エラーコード詳細1]", $"{result.controlInfo.errorCodeDetail1}")
                                                                .Replace("[エラーコード詳細2]", $"{result.controlInfo.errorCodeDetail2}")
                                };
                                GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                            }
                            Utilities.Log.Error("Health Check is done, response is abnormal, procResult = " + result.controlInfo.procResult + "!");
                        }
                        else if (result.bizInfo.posWaitingStatus != "1")
                        {
                            if (GlobalData.Data_FromPosDat.location == "0")
                            {
                                msgCode = "A0005";
                                FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                {
                                    Message = msgMode.msg1.Replace("[Arch状態]", $"{result.bizInfo.posWaitingStatus}"),
                                    BtnVisibility = "Visible",
                                    BtnContent = "終了",
                                    BtnCommand = new Command(ErrorEnd),
                                };
                                GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                            }
                            else
                            {
                                msgCode = "D0003";
                                FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                {
                                    MessageMaintenance = msgMode.msg2.Replace("[Arch状態]", $"{result.bizInfo.posWaitingStatus}"),
                                };
                                GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                            }
                            Utilities.Log.Error("Health Check is done, Arch status is not Idle, posWaitingStatus = " + result.bizInfo.posWaitingStatus + "!");
                        }
                        else
                        {
                            isHealthCheckOK = true;
                            Utilities.Log.Info("Health Check is done and no error found!");
                            isBrandNameOK = true;
                            Utilities.Log.Info("Start checking brand_name in Config.json file!");
                            if (GlobalData.BasicConfig.brand_name.Length == 0)
                            {
                                isBrandNameOK = false;
                                Utilities.Log.Error("▲ Config.json >> brand_name: list of services is empty.");
                            }
                            else
                            {
                                int cnt = 0;
                                foreach (var service in GlobalData.BasicConfig.brand_name)
                                {
                                    string brandName = Utilities.GetBrandName(service);
                                    if (GlobalData.ServiceNameCorrect.Contains(service) && !string.IsNullOrEmpty(brandName))
                                    {
                                        cnt++;
                                        break;
                                    }
                                    /*if (!GlobalData.ServiceNameCorrect.Contains(key))
                                    {
                                        isBrandNameOK = false;
                                        Utilities.Log.Error("▲ Config.json >> brand_name: " + key + " is not correct.");
                                        break;
                                    }
                                    if (string.IsNullOrEmpty(GlobalData.BasicConfig.brand_name[key]))
                                    {
                                        isBrandNameOK = false;
                                        Utilities.Log.Error("▲ Config.json >> brand_name: value of " + key + " is null or empty.");
                                        break;
                                    }*/
                                    if (cnt == 0) Utilities.Log.Error("▲ Config.json >> brand_name: there are no services that match the services of system and brand_code in ServiceId.json.");
                                }
                            }
                            if (!isBrandNameOK)
                            {
                                if (GlobalData.Data_FromPosDat.location == "0")
                                {
                                    msgCode = "A0001";
                                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                    {
                                        BtnVisibility = "Visible",
                                        BtnContent = "終了",
                                        BtnCommand = new Command(ErrorEnd),
                                    };
                                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                                }
                                else
                                {
                                    msgCode = "D0005";
                                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties();
                                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                                }
                            }
                            else
                            {
                                Utilities.Log.Info("Done checking brand_name in Config.json file and no error found!");
                                is_btn_order_SubtractValue_OK = true;
                                Utilities.Log.Info("Start checking btn_order_SubtractValue in Config.json file!");
                                if (GlobalData.BasicConfig.btn_order_SubtractValue.Length == 0)
                                {
                                    is_btn_order_SubtractValue_OK = false;
                                    Utilities.Log.Error("▲ Config.json >> btn_order_SubtractValue: list of services is empty.");
                                }
                                else
                                {
                                    int cnt = 0;
                                    foreach (var service in GlobalData.BasicConfig.btn_order_SubtractValue)
                                    {
                                        string brandName = Utilities.GetBrandName(service);
                                        if (GlobalData.ServiceNameCorrect.Contains(service) && !string.IsNullOrEmpty(brandName))
                                        {
                                            cnt++;
                                            break;
                                        }
                                        /*if (!GlobalData.ServiceNameCorrect.Contains(service))
                                        {
                                            is_btn_order_SubtractValue_OK = false;
                                            Utilities.Log.Error("▲ Config.json >> btn_order_SubtractValue: " + service + " is not correct.");
                                            break;
                                        }*/
                                        if (cnt == 0) Utilities.Log.Error("▲ Config.json >> btn_order_SubtractValue: there are no services that match the services of system and brand_code in ServiceId.json.");
                                    }
                                }
                                if (!is_btn_order_SubtractValue_OK)
                                {
                                    if (GlobalData.Data_FromPosDat.location == "0")
                                    {

                                        msgCode = "A0002";
                                        GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                        {
                                            BtnVisibility = "Visible",
                                            BtnContent = "終了",
                                            BtnCommand = new Command(ErrorEnd),
                                        };
                                        GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                                    }
                                    else
                                    {
                                        msgCode = "D0006";
                                        GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties();
                                        GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                                    }
                                }
                                else
                                {
                                    Utilities.Log.Info("Done checking btn_order_SubtractValue in Config.json file and no error found!");
                                }
                            }
                        }

                        if (!isHealthCheckOK || !isBrandNameOK || !is_btn_order_SubtractValue_OK)
                        {
                            if (GlobalData.Data_FromPosDat.location == "1")
                            {
                                GlobalData.MsgCode = msgCode;
                                Session.ScreenState.NextState = StateMachine.State.emErrorToMaintenance;
                            }
                            else
                                Session.MainViewModel.LoadScreen_emMessage(msgCode);
                        }
                        else if (GlobalData.Data_FromPosDat.location == "1")
                        {
                            // customer operation
                            GlobalData.LastOperator = "1";
                            Utilities.Log.Info("■ Start transaction (payment)");

                            /*// Clear old printer error
                            if (GlobalData.ApplicationError == "AE7" || GlobalData.ApplicationError == "AE9" || GlobalData.ApplicationError == "AE10")
                            {
                                GlobalData.ApplicationError = "";
                            }

                            // Connect printer
                            PrintAPI.GetPrinter(GlobalData.PrinterData.device_name);
                            PrintAPI.OpenConnection();

                            // Check printer status
                            bool isPrinterEnabled = PrintAPI.Printer != null && PrintAPI.Printer.DeviceEnabled;
                            bool isPrinterReady = false;
                            if (isPrinterEnabled) isPrinterReady = !PrintAPI.Printer.CoverOpen && !PrintAPI.Printer.RecEmpty;

                            if (isPrinterEnabled && isPrinterReady)
                            {
                                if (!readPrinterOK) // GlobalData.ApplicationError == "AE1" || FileStruct.IsBtnOrderConfigError("SubtractValue").Item1
                                {
                                    Session.PreState_emCM = "emS";
                                    Session.ScreenState.NextState = StateMachine.State.emCM;
                                    if (FileStruct.IsBtnOrderConfigError("SubtractValue").Item1)
                                    {
                                        GlobalData.ScreenErrorMsg = FileStruct.IsBtnOrderConfigError("SubtractValue").Item2;
                                        GlobalData.BtnOrderConfigErrors.Clear();
                                    }
                                    else
                                    {
                                        GlobalData.ScreenErrorMsg = GlobalData.ApplicationErrorMsg;
                                    }
                                    GlobalData.ApplicationErrorMsg = "";
                                    GlobalData.ApplicationError = "";
                                }
                                else*/
                            {
                                Session.ScreenState.NextState = StateMachine.State.emPayment;
                            }
                            /*}
                            else
                            {
                                Session.MaintenanceMode = "";
                                Session.ScreenState.NextState = StateMachine.State.emErrorToMaintenance;
                                if (isPrinterEnabled && PrintAPI.Printer.CoverOpen)
                                {
                                    GlobalData.ApplicationError = "AE9";
                                    GlobalData.ApplicationErrorMsg = Utilities.GetErrorTypeAndMsg("application", GlobalData.ApplicationError).Item2;
                                    Utilities.Log.Error("▲ Printer Cover is open");
                                }
                                else if (isPrinterEnabled && PrintAPI.Printer.RecEmpty)
                                {
                                    GlobalData.ApplicationError = "AE10";
                                    GlobalData.ApplicationErrorMsg = Utilities.GetErrorTypeAndMsg("application", GlobalData.ApplicationError).Item2;
                                    Utilities.Log.Error("▲ Printer runs out of receipt paper");
                                }
                                else
                                {
                                    GlobalData.ApplicationError = "AE7";
                                    GlobalData.ApplicationErrorMsg = Utilities.GetErrorTypeAndMsg("application", GlobalData.ApplicationError).Item2;
                                }
                                Utilities.Log.Info("Load screen emMT");
                            }*/
                        }
                        else if (GlobalData.Data_FromPosDat.location == "0")
                        {
                            // clerk operation at standby maintenance
                            GlobalData.LastOperator = "0";
                            Utilities.Log.Info("■ Start transaction (maintenance)");

                            /*// Clear old printer error
                            if (GlobalData.ApplicationError == "AE7" || GlobalData.ApplicationError == "AE9" || GlobalData.ApplicationError == "AE10")
                            {
                                GlobalData.ApplicationError = "";
                            }

                            // Connect printer
                            PrintAPI.GetPrinter(GlobalData.PrinterData.device_name);
                            PrintAPI.OpenConnection();
                            bool isPrinterEnabled = PrintAPI.Printer != null && PrintAPI.Printer.DeviceEnabled;
                            bool isPrinterReady = false;
                            if (isPrinterEnabled) isPrinterReady = !PrintAPI.Printer.CoverOpen && !PrintAPI.Printer.RecEmpty;

                            if (isPrinterEnabled && isPrinterReady && readPrinterOK)*/
                            {
                                Session.ScreenState.NextState = StateMachine.State.emMaintenanceMenu;
                                Session.MaintenanceMode = "Maintenance";
                            }
                            /*else
                            {
                                if (!readPrinterOK) // (GlobalData.ApplicationError == "AE1")
                                {
                                    GlobalData.ApplicationErrorMsg = Utilities.GetErrorTypeAndMsg("application", "AE1").Item2;
                                }
                                else if (isPrinterEnabled && PrintAPI.Printer.CoverOpen)
                                {
                                    GlobalData.ApplicationError = "AE9";
                                    GlobalData.ApplicationErrorMsg = Utilities.GetErrorTypeAndMsg("application", GlobalData.ApplicationError).Item2;
                                    Utilities.Log.Error("▲ Printer Cover is open");
                                }
                                else if (isPrinterEnabled && PrintAPI.Printer.RecEmpty)
                                {
                                    GlobalData.ApplicationError = "AE10";
                                    GlobalData.ApplicationErrorMsg = Utilities.GetErrorTypeAndMsg("application", GlobalData.ApplicationError).Item2;
                                    Utilities.Log.Error("▲ Printer runs out of receipt paper");
                                }
                                else
                                {
                                    GlobalData.ApplicationError = "AE7";
                                    GlobalData.ApplicationErrorMsg = Utilities.GetErrorTypeAndMsg("application", GlobalData.ApplicationError).Item2;
                                }
                                Session.ScreenState.NextState = StateMachine.State.emMAorMI_EMSG;
                            }*/
                        }
                    }
                    else
                    {
                        Session.ScreenState.NextState = StateMachine.State.emIdle;
                        WinAPI.HideWindow();
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.Log.Error(ex.ToString());
            }
            return Session.ScreenState.GoToNextState(this);
        }

        private async void ErrorEnd()
        {
            GlobalData.Data_ToPosDat.service = GlobalData.Data_FromPosDat.service;
            GlobalData.Data_ToPosDat.sequence = GlobalData.Data_FromPosDat.sequence;
            GlobalData.Data_ToPosDat.last_operator = GlobalData.LastOperator;
            GlobalData.Data_ToPosDat.result = "0";
            GlobalData.Data_ToPosDat.SettledAmount = "";
            GlobalData.Data_ToPosDat.CurrentService = "";
            GlobalData.Data_ToPosDat.statementID = "";

            await WinAPI.EndTransaction();
        }
    }
}
