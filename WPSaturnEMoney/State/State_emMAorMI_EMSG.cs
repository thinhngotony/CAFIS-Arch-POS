using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPSaturnEMoney.State
{
    class State_emMAorMI_EMSG : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            StateMachine.State preState = Session.ScreenState.CurrentState;
            if (preState != StateMachine.State.emMAorMI_EMSG)
            {
                Session.MainViewModel.LoadScreen_emMAorMI_EMSG();
                Session.ScreenState.CurrentState = StateMachine.State.emMAorMI_EMSG;

                if (preState == StateMachine.State.emIdle)
                {
                    await Task.Delay(700);
                    Common.WinAPI.BringWindowTop();
                }
            }
            
            return Session.ScreenState.GoToNextState(this);
        }
    }
}
