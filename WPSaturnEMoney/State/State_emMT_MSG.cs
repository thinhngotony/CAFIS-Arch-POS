﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Common;

namespace WPSaturnEMoney.State
{
    class State_emMT_MSG : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emMT_MSG)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emMT_MSG;
                Session.MainViewModel.LoadScreen_emMT_MSG();

                // Print receipt (if any)
                // Utilities.PrintReceiptOperation();
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
