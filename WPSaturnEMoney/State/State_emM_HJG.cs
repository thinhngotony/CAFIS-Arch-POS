using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPSaturnEMoney.State
{
    class State_emM_HJG : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emM_HJG)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emM_HJG;
                Session.MainViewModel.LoadScreen_emM_HJG();
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
