using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Models;

namespace WPSaturnEMoney.State
{
    class State_emES_MENU : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emES_MENU)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emES_MENU;

                FileStruct.MsgMode msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "J0001");
                GlobalData.ViewModelProperties.Message = msgMode.msg2;
                GlobalData.CustomerViewModelProperties.Message = msgMode.msg3;

                Session.MainViewModel.LoadScreen_emES_MENU();
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
