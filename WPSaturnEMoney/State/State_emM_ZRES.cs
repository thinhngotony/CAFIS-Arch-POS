using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Common;

namespace WPSaturnEMoney.State
{
    class State_emM_ZRES : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emM_ZRES)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emM_ZRES;
                Session.MainViewModel.LoadScreen_emM_ZRES();

                // Utilities.PrintReceiptOperation();
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
