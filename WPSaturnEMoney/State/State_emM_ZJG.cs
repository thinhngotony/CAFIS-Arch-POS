using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPSaturnEMoney.State
{
    class State_emM_ZJG : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emM_ZJG)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emM_ZJG;
                Session.MainViewModel.LoadScreen_emM_ZJG();
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
