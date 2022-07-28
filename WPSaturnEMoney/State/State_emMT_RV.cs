using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPSaturnEMoney.State
{
    class State_emMT_RV : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emMT_RV)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emMT_RV;
                Session.MainViewModel.LoadScreen_emMT_RV();
            }
            
            return Session.ScreenState.GoToNextState(this);
        }
    }
}
