using Microsoft.PointOfService;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;

namespace WPSaturnEMoney.State
{
    class State_emLoginToMaintenance : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emLoginToMaintenance)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emLoginToMaintenance;

                if (GlobalData.BasicConfig.maintenance_staff_check_flag == "1")
                {
                    Session.MainViewModel.LoadScreen_emLoginToMaintenance();
                }
            }

            if (Session.IsIDCorrect || GlobalData.BasicConfig.maintenance_staff_check_flag == "0")
            {
                string msgCode = "";
                if (GlobalData.TransactionErrorType == Utilities.ErrorType.PrintReceipt)
                {
                    if (Session.ScreenState.PreviousStates[0] == StateMachine.State.emBalanceInquiry)
                    {
                        var printReceiptData = Utilities.PrepareBalanceReceiptData(GlobalData.SaturnAPIResponse);
                        if (printReceiptData.Count > 0)
                        {
                            // Connect to the printer
                            PrintAPI.GetPrinter(GlobalData.PrinterData.device_name);
                            PrintAPI.OpenConnection();

                            // Check printer status
                            bool isPrinterEnabled = PrintAPI.Printer != null && PrintAPI.Printer.DeviceEnabled;
                            bool isPrinterReady = true;
                            if (!isPrinterEnabled)
                            {
                                msgCode = "B0013";
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
                                        msgCode = "B0012";
                                        Utilities.Log.Error("▲ Printer Cover is open!");
                                    }
                                    else if (ex.ErrorCode == ErrorCode.Extended && ex.ErrorCodeExtended == 203)
                                    {
                                        msgCode = "B0011";
                                        Utilities.Log.Error("▲ Printer runs out of receipt paper!");
                                    }
                                    else
                                    {
                                        msgCode = "B0013";
                                        Utilities.Log.Error("▲ Printer is not available!");
                                    }
                                    GlobalData.MPErrorCode = ex.ErrorCode;
                                    GlobalData.MPErrorCodeExtended = ex.ErrorCodeExtended;
                                    isPrinterReady = false;
                                }
                            }
                            if (!isPrinterEnabled || !isPrinterReady)
                            {
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
                            }
                            else
                            {
                                // Save to journal file
                                if (!FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.JournalPath, ref GlobalData.JournalData))
                                {
                                    GlobalData.JournalData = new System.Collections.Generic.List<FileStruct.RowData>();
                                }
                                GlobalData.JournalData.AddRange(printReceiptData);
                                FileStruct.CsFileWrite(GlobalData.AppPath, GlobalData.JournalPath, GlobalData.JournalData);
                                Utilities.Log.Info("Save printed receipt data to journal file!");
                                Session.ScreenState.NextState = StateMachine.State.emBalanceInquiry;
                            }
                        }
                    }
                }
                else
                {
                    Session.MaintenanceMode = "MaintenanceFromError";
                    Session.ScreenState.NextState = StateMachine.State.emMaintenanceMenuFromError;
                    Session.PreFunc_emMS_MENU = "login";
                }
                Session.IsIDCorrect = false;
            }
            
            return Session.ScreenState.GoToNextState(this);
        }
    }
}
