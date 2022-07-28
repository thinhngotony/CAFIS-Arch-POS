using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Models;
using WPSaturnEMoney.Common;

namespace WPSaturnEMoney.State
{
    class State_emIdle : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emIdle)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emIdle;

                string msgCode = "D0001";
                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                {
                    BtnVisibility = "Visible",
                    BtnContent = "OK",
                    BtnCommand = new ViewModels.Command(WinAPI.HideWindow)
                };
                GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                Session.MainViewModel.LoadScreen_emMessage(msgCode);
            }

            /*if (GlobalData.ApplicationError == "AE8")
            {
                Session.ScreenState.NextState = StateMachine.State.emMAorMI_EMSG;
            }*/

            if (Session.IsStatusOK)
            {
                // FileStruct.SettingReadAndCheck();
                Session.ScreenState.NextState = StateMachine.State.emStandBy;
                Session.IsStatusOK = false;
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
