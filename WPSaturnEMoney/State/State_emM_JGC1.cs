using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPSaturnEMoney.State
{
    class State_emM_JGC1 : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emM_JGC1)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emM_JGC1;
                Session.MainViewModel.LoadScreen_emM_JGC1();
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
