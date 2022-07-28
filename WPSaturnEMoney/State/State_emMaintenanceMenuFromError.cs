using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Models;

namespace WPSaturnEMoney.State
{
    class State_emMaintenanceMenuFromError : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emMaintenanceMenuFromError)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emMaintenanceMenuFromError;
                /*if (GlobalData.IsPrintError)
                {
                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties();
                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                    FileStruct.MsgMode msgMode = new FileStruct.MsgMode();
                    if (Session.ScreenState.PreviousStates[0] == StateMachine.State.emPayment)
                    {
                        if (GlobalData.MPErrorCode == ErrorCode.Extended && GlobalData.MPErrorCodeExtended == 201)
                        {
                            msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == "B0002", "B0002");
                        }
                        else if (GlobalData.MPErrorCode == ErrorCode.Extended && GlobalData.MPErrorCodeExtended == 203)
                        {
                            msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == "B0001", "B0001");
                        }
                        else
                        {
                            msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == "B0003", "B0003");
                        }
                    }
                    else if (Session.ScreenState.PreviousStates[0] == StateMachine.State.emBalanceInquiry)
                    {
                        if (GlobalData.MPErrorCode == ErrorCode.Extended && GlobalData.MPErrorCodeExtended == 201)
                        {
                            msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == "B0012", "B0012");
                        }
                        else if (GlobalData.MPErrorCode == ErrorCode.Extended && GlobalData.MPErrorCodeExtended == 203)
                        {
                            msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == "B0011", "B0011");
                        }
                        else
                        {
                            msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == "B0013", "B0013");
                        }
                    }
                    GlobalData.ViewModelProperties.HeaderTitle = msgMode.header_title2;
                    GlobalData.ViewModelProperties.Message = msgMode.msg2;
                    GlobalData.CustomerViewModelProperties.HeaderTitle = msgMode.header_title3;
                    GlobalData.CustomerViewModelProperties.Message = msgMode.msg3;

                    GlobalData.IsPrintError = false;
                }*/
                Session.MainViewModel.LoadScreen_emMaintenanceMenuFromError(GlobalData.MsgCode);
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
