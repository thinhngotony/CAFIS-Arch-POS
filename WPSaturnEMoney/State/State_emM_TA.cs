using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;
using System.IO;
using WPSaturnEMoney.ViewModels;

namespace WPSaturnEMoney.State
{
    class State_emM_TA : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emM_TA)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emM_TA;

                FileStruct.MsgMode msgMode;
                if (Session.PreState_emTM != "emP") //Error While Not Selected Emoney
                {
                    msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "J0002");
                    //GlobalData.ViewModelProperties.ListHeaderTitle = msgMode.header_title2;
                    GlobalData.ViewModelProperties.Message = msgMode.msg2;
                    //GlobalData.CustomerViewModelProperties.ListHeaderTitle = msgMode.header_title3;
                    GlobalData.CustomerViewModelProperties.Message = msgMode.msg3;
                    Session.MainViewModel.LoadScreen_emM_EMSG();
                }
                else //Error While Selected Emoney
                {
                    if (!File.Exists(GlobalData.AppPath + GlobalData.LastTransactionPath))
                    {
                        msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "J0003");
                        //GlobalData.ViewModelProperties.ListHeaderTitle = msgMode.header_title2;
                        GlobalData.ViewModelProperties.Message = msgMode.msg2;
                        //GlobalData.CustomerViewModelProperties.ListHeaderTitle = msgMode.header_title3;
                        GlobalData.CustomerViewModelProperties.Message = msgMode.msg3;
                        Session.MainViewModel.LoadScreen_emM_EMSG();
                    }
                    else
                    {
                        msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "J0004");
                        //GlobalData.ViewModelProperties.ListHeaderTitle = msgMode.header_title2;
                        GlobalData.ViewModelProperties.Message = msgMode.msg2;
                        //GlobalData.CustomerViewModelProperties.ListHeaderTitle = msgMode.header_title3;
                        GlobalData.CustomerViewModelProperties.Message = msgMode.msg3;
                        Session.MainViewModel.LoadScreen_emM_TA();

                        var LastTransactionResult = new FileStruct.SaturnAPIResponse();
                        FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.LastTransactionPath, ref LastTransactionResult);

                        // TODO: print

                    }
                }


                if (Session.ClickedButton_emM_MENU == "check_connection")
                {
                    var connectionResult = await PaymentAPI.CheckConnection();
                    if (connectionResult.Item1 == Utilities.ErrorType.Success)

                        GlobalData.ViewModelProperties.ButtonBorderColor = GlobalData.BasicConfig.color.maintenance.btn_line;
                    GlobalData.ViewModelProperties.NormalButtonColor = GlobalData.BasicConfig.color.maintenance.normal_btn;
                    GlobalData.ViewModelProperties.NormalButtonLabelColor = GlobalData.BasicConfig.color.maintenance.normal_btn_label;
                    GlobalData.ViewModelProperties.SignalBarColor = GlobalData.BasicConfig.color.common.signal_caution;
                    GlobalData.ViewModelProperties.BtnCommand = new Command(ReturnToMaintenance);
                    GlobalData.ViewModelProperties.BtnVisibility = "Visible";
                    GlobalData.ViewModelProperties.BtnContent = "戻る";

                    if (Session.ClickedButton_emM_MENU == "check_connection")                                               // 7.2 + 8.2
                    {
                        msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "G0001");
                        GlobalData.ScreenProgressMsg = msgMode.msg2;
                        Session.MainViewModel.LoadScreen_emM_TA();

                        FileStruct.SaturnAPIResponse result = await SaturnAPI.HealthCheck().ConfigureAwait(false);

                        if (result == null)
                        {
                            msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "G0003");
                        }
                        else if (result.controlInfo.procResult != "0")
                        {
                            msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "G0004");
                        }
                        else if (result.bizInfo.posWaitingStatus != "1")
                        {
                            msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "G0005");
                        }
                        else
                        {
                            msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "G0002");
                            GlobalData.ViewModelProperties.SignalBarColor = GlobalData.BasicConfig.color.common.signal_complete;
                        }
                    }
                    else if (Session.ClickedButton_emM_MENU == "balance_inquiry")                                           // 7.4 + 8.3
                    {
                        Session.MainViewModel.LoadScreen_emM_TA();
                        var obj = new FileStruct.SaturnAPIRequest();

                        FileStruct.SaturnAPIResponse result = await SaturnAPI.sendRequest(obj).ConfigureAwait(false);

                        // TODO: ...
                    }
                    else if (Session.ClickedButton_emM_MENU == "reprint_receipt")                                           // 8.4                            
                    {
                        var LastTransactionResult = new FileStruct.SaturnAPIResponse();
                        var Reprint_service_ID = "";

                        if (Reprint_service_ID == null)
                        {
                            msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "N0001");
                        }
                        else if (!FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.LastTransactionPath, ref LastTransactionResult))
                        {
                            msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "N0002");
                        }
                        else
                        {
                            msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "N0003");
                            GlobalData.ScreenProgressMsg = msgMode.msg2;
                            Session.MainViewModel.LoadScreen_emM_TA();

                            //TODO: Get data then Print Last Minute Transaction

                            msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "N0004");
                            GlobalData.ViewModelProperties.SignalBarColor = GlobalData.BasicConfig.color.common.signal_complete;
                        }
                    }
                    else if (Session.ClickedButton_emM_MENU == "transaction_inquiry")                                       // 7.3 + 8.5
                    {
                        // TODO: エラー発生が支払操作時で、最終決済シーケンス番号が存在し、電子マネーが選択済か？
                        if (false)
                        {
                            msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "H0001");
                        }
                        else
                        {
                            msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "H0002");
                            GlobalData.ScreenProgressMsg = msgMode.msg2;
                            Session.MainViewModel.LoadScreen_emM_TA();

                            // TODO: Change curSeqNo and curService to correct references
                            var LastTransactionResult = new FileStruct.SaturnAPIResponse();
                            var curSeqNo = "0001";
                            var curService = "4100001";

                            if (!FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.LastTransactionPath, ref LastTransactionResult)
                                || LastTransactionResult.bizInfo.seqNo != curSeqNo
                                || LastTransactionResult.controlInfo.service != curService
                            )
                            {
                                Utilities.Log.Info("Requesting lastest transaction result...");

                                var obj = new FileStruct.SaturnAPIRequest();

                                // TODO: Set obj.service to curService reference
                                obj.service = "0";
                                obj.biz = "9";
                                obj.printMode = "0";

                                FileStruct.SaturnAPIResponse result = await SaturnAPI.sendRequest(obj).ConfigureAwait(false);

                                if (result == null)
                                {
                                    msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "H0006");
                                }
                                else if (result.controlInfo.procResult != "0")
                                {
                                    msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "H0005");
                                }
                                else if (result.bizInfo.bizInfo == null) // 応答電文(Response telegram) bình thường (Kết quả xử lsy=0), nhưng có gắn data đối tượng chưa?
                                {
                                    msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "H0004");
                                }
                                else
                                {
                                    // TODO: Health check

                                    // Rewrite new LastTransactionResult
                                    LastTransactionResult.bizInfo = result.bizInfo.bizInfo;
                                    LastTransactionResult.controlInfo = result.bizInfo.controlInfo;
                                    FileStruct.CsFileWrite(GlobalData.AppPath, GlobalData.LastTransactionPath, LastTransactionResult);

                                    // TODO: Print Last Minute Transaction

                                    msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "H0003");
                                    GlobalData.ViewModelProperties.SignalBarColor = GlobalData.BasicConfig.color.common.signal_complete;
                                }
                            }
                            else
                            {
                                //TODO: Print Last Minute Transaction

                                msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "H0003");
                                GlobalData.ViewModelProperties.SignalBarColor = GlobalData.BasicConfig.color.common.signal_complete;
                            }
                        }
                    }
                    else if (Session.ClickedButton_emM_MENU == "daily_print")
                    {
                        // Total data file exists
                        if (false)
                        {
                            msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "O0011");
                        }
                        else
                        {
                            msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "O0002");
                            GlobalData.ScreenProgressMsg = msgMode.msg2;
                            Session.MainViewModel.LoadScreen_emM_TA();

                            var obj = new FileStruct.SaturnAPIRequest();
                            obj.service = "1000000";
                            obj.biz = "0";

                            FileStruct.SaturnAPIResponse result = await SaturnAPI.sendRequest(obj).ConfigureAwait(false);

                            if (result == null)
                            {
                                msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "O0005");

                            }
                            else if (result.controlInfo.procResult != "0")
                            {
                                msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "O0006");

                            }
                            else if (result.bizInfo.transactionArray == null || result.bizInfo.transactionArray.Count == 0)
                            {
                                msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "O0007");
                            }
                            else
                            {
                                // TODO: Save total data file

                                // TODO: Print 日計 receipt

                                // TODO: Delete total data file

                                msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "O0010");
                                GlobalData.ViewModelProperties.SignalBarColor = GlobalData.BasicConfig.color.common.signal_complete;
                            }
                        }
                    }
                    else // Session.ClickedButton_emM_MENU == "daily_reprint"
                    {
                        // Total data file exists
                        if (false)
                        {
                            // TODO: Get total data
                        }
                        else
                        {
                            msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "P0002");
                            GlobalData.ScreenProgressMsg = msgMode.msg2;
                            Session.MainViewModel.LoadScreen_emM_TA();

                            var obj = new FileStruct.SaturnAPIRequest();
                            obj.service = "4000000";

                            FileStruct.SaturnAPIResponse result = await SaturnAPI.sendRequest(obj).ConfigureAwait(false);

                            if (result == null)
                            {
                                msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "P0003");
                            }
                            else if (result.controlInfo.procResult != "0")
                            {
                                msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "P0004");

                            }
                            else if (result.bizInfo.transactionArray == null || result.bizInfo.transactionArray.Count == 0)
                            {
                                msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "P0005");

                            }
                        }
                        // TODO: Receipt shaping

                        // TODO: Print 日計 receipt

                        // TODO: Delete total data file

                        msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "P0008");
                        GlobalData.ViewModelProperties.SignalBarColor = GlobalData.BasicConfig.color.common.signal_complete;
                    }

                    if (msgMode.msg4 == null)
                    {
                        GlobalData.CustomerViewModelProperties.Message = msgMode.msg3;
                        GlobalData.CustomerViewModelProperties.SignalBarColor = GlobalData.BasicConfig.color.common.signal_calling;
                    }
                    else
                    {
                        GlobalData.CustomerViewModelProperties.Message = msgMode.msg4;
                        //GlobalData.CustomerViewModelProperties.SignalBarColor = GlobalData.BasicConfig.color.common.signal_normal;
                    }

                }

                // GlobalData.ViewModelProperties.ListHeaderTitle = msgMode.header_title2;
                GlobalData.ViewModelProperties.Message = msgMode.msg2;
                Session.MainViewModel.LoadScreen_emMessage();
            }

            return Session.ScreenState.GoToNextState(this);
        }
        private void ReturnToMaintenance()
        {
            Session.ScreenState.NextState = StateMachine.State.emMaintenanceMenu;
            Session.TimerCount = 0;
        }
    }
}
