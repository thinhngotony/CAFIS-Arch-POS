using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPSaturnEMoney.State
{
    class State_emMT_JGC2 : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emMT_JGC2)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emMT_JGC2;
                Session.MainViewModel.LoadScreen_emMT_JGC2();
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
