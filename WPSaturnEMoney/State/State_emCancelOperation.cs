using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;

namespace WPSaturnEMoney.State
{
    class State_emCancelOperation : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emCancelOperation)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emCancelOperation;

                /*string msgCode = "";
                if (Session.ScreenState.PreviousStates[0] == StateMachine.State.emPayment)
                {
                    if (GlobalData.TransactionErrorType == Utilities.ErrorType.Timeout)
                    {
                        msgCode = "E0015";
                    }
                    else if (GlobalData.TransactionErrorType == Utilities.ErrorType.ProcessingNotCompleted)
                    {
                        msgCode = "E0002";
                    }
                }
                else if (Session.ScreenState.PreviousStates[0] == StateMachine.State.emBalanceInquiry)
                {
                    if (GlobalData.TransactionErrorType == Utilities.ErrorType.Timeout)
                    {
                        msgCode = "F0014";
                    }
                    else if (GlobalData.TransactionErrorType == Utilities.ErrorType.ProcessingNotCompleted)
                    {
                        msgCode = "F0004";
                    }
                }
                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties();
                GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                Session.MainViewModel.LoadScreen_emMessage(msgCode);

                Utilities.Log.Info("Cancel request is sent!");
                FileStruct.SaturnAPIResponse result = await SaturnAPI.CancelRequest();

                if (result == null)
                {
                    if (Session.ScreenState.PreviousStates[0] == StateMachine.State.emPayment)
                        msgCode = "E0004";
                    else if (Session.ScreenState.PreviousStates[0] == StateMachine.State.emBalanceInquiry)
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
                }
                else if (result.controlInfo.procResult != "0" && result.controlInfo.errorCode != "TFWL0002")
                {
                    if (Session.ScreenState.PreviousStates[0] == StateMachine.State.emPayment)
                        msgCode = "E0005";
                    else if (Session.ScreenState.PreviousStates[0] == StateMachine.State.emBalanceInquiry)
                        msgCode = "F0007";
                    FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                    {
                        MessageMaintenance = msgMode.msg2.Replace("[エラーコード詳細1]", $"{result.controlInfo.errorCodeDetail1}")
                                                         .Replace("[エラーコード詳細2]", $"{result.controlInfo.errorCodeDetail2}"),
                    };
                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties
                    {
                        MessageMaintenance = msgMode.msg3,
                    };
                    Utilities.Log.Error("Failed to abort operation, response is abnormal! procResult: " + result.controlInfo.procResult
                        + ", errorCodeDetail1: " + result.controlInfo.errorCodeDetail1 + ", errorCodeDetail2: " + result.controlInfo.errorCodeDetail2 + "!");
                    GlobalData.MsgCode = msgCode;
                    Session.ScreenState.NextState = StateMachine.State.emErrorToMaintenance;
                }
                else if (result.controlInfo.procResult == "9" && result.controlInfo.errorCode == "TFWL0002")
                {
                    Utilities.Log.Info("Abort operation successful!");
                    GlobalData.TransactionErrorType = Utilities.ErrorType.ProcessingNotCompleted;
                    if (Session.ScreenState.PreviousStates[0] == StateMachine.State.emPayment)
                        Session.ScreenState.NextState = StateMachine.State.emPaymentResult;
                    else if (Session.ScreenState.PreviousStates[0] == StateMachine.State.emBalanceInquiry)
                        Session.ScreenState.NextState = StateMachine.State.emBalanceInquiryResult;
                }
                else if (result.controlInfo.procResult == "0") // (result.bizInfo.posWaitingStatus != 1)
                {
                    if (Session.ScreenState.PreviousStates[0] == StateMachine.State.emPayment)
                        msgCode = "E0006";
                    else if (Session.ScreenState.PreviousStates[0] == StateMachine.State.emBalanceInquiry)
                        msgCode = "F0008";
                    FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                    {
                        MessageMaintenance = msgMode.msg2.Replace("[Arch状態]", $"{result.bizInfo.posWaitingStatus}"),
                    };
                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties
                    {
                        MessageMaintenance = msgMode.msg3,
                    };
                    Utilities.Log.Error("Failed to abort operation, Arch status is not Idle! posWaitingStatus: " + result.bizInfo.posWaitingStatus + "!");
                    GlobalData.MsgCode = msgCode;
                    Session.ScreenState.NextState = StateMachine.State.emErrorToMaintenance;
                }*/
            }
            return Session.ScreenState.GoToNextState(this);
        }
    }
}
