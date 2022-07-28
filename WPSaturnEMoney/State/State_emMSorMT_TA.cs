using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;

namespace WPSaturnEMoney.State
{
    class State_emMSorMT_TA : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emMSorMT_TA)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emMSorMT_TA;
                Session.MainViewModel.LoadScreen_emMSorMT_TA();
            }

            if (Session.MaintenanceMode == "MS")
            {
                var connectionResult = await PaymentAPI.CheckConnection();
                if (connectionResult.Item1 == Utilities.ErrorType.Success)
                {
                    GlobalData.ScreenErrorMsg = "完了しました。";
                }
                else
                {
                    GlobalData.ScreenErrorMsg = connectionResult.Item2;
                }

                Session.ScreenState.NextState = StateMachine.State.emMS_MENU;
                Session.PreFunc_emMS_MENU = "check_connection";
                Session.ScreenState.NextState = StateMachine.State.emES_MENU;
                Session.PreFunc_emES_MENU = "check_connection";
            }
            else if (Session.ClickedButton_emMT_MENU == "check_connection")
            {
                var connectionResult = await PaymentAPI.CheckConnection();
                if (connectionResult.Item1 == Utilities.ErrorType.Success)
                {
                    Session.ScreenState.NextState = StateMachine.State.emMT_SMSG;
                    Session.PreFunc_emMT_SMSG = "check_connection";
                }
                else
                {
                    GlobalData.ScreenSubErrorMsg = connectionResult.Item2;
                    Session.ScreenState.NextState = StateMachine.State.emMT_EMSG;
                    Session.PreFunc_emMT_EMSG = "check_connection";
                }
            }
            else // Session.ClickedButton_emMT_MENU = "reprint_receipt"
            {
                if (System.IO.File.Exists(GlobalData.AppPath + GlobalData.ConvertedReceiptPath))
                {
                    // Will print receipt information in ConvertedReceipt.dat
                    Session.ScreenState.NextState = StateMachine.State.emMT_SMSG;
                    Session.PreFunc_emMT_SMSG = "reprint_conv_receipt";
                }
                else
                {
                    var printRetryResult = await PaymentAPI.PrintRetry();
                    Utilities.Log.Info("Previous SubtractValue SequenceNumber=" + GlobalData.PreSequenceNumberSV);

                    if (printRetryResult.Item1 == Utilities.ErrorType.Success)
                    {
                        // Acquire sequence number and compare with the sequence number of the previous SubtractValue request,
                        // and if they are the same, the transaction is considered "successful"
                        if (GlobalData.SequenceNumberPR == GlobalData.PreSequenceNumberSV)
                        {
                            Session.ScreenState.NextState = StateMachine.State.emMT_F;
                            Session.IsPrintRetryPreState_emMT_F = true;
                        }
                        else
                        {
                            Session.ScreenState.NextState = StateMachine.State.emMT_SMSG;
                            Session.PreFunc_emMT_SMSG = "reprint_receipt";
                        }
                    }
                    else
                    {
                        if (GlobalData.SequenceNumberPR == GlobalData.PreSequenceNumberSV)
                        {
                            GlobalData.TransactionErrorType = printRetryResult.Item1;
                            GlobalData.ScreenErrorMsg = printRetryResult.Item2;
                        }

                        GlobalData.ScreenSubErrorMsg = printRetryResult.Item2;
                        Session.ScreenState.NextState = StateMachine.State.emMT_EMSG;
                        Session.PreFunc_emMT_EMSG = "reprint_receipt";
                    }
                }
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
