using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;
using Microsoft.PointOfService;

namespace WPSaturnEMoney.State
{
    class State_emM_EMSG : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emM_EMSG)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emM_EMSG;

                Session.MainViewModel.LoadScreen_emM_EMSG();

                if (Session.PreFunc_emM_EMSG == "read_value" ||
                    Session.PreFunc_emM_EMSG == "card_history" ||
                    Session.PreFunc_emM_EMSG == "reprint_receipt" ||
                    Session.PreFunc_emM_EMSG == "daily_total")
                {
                    // Utilities.PrintReceiptOperation();
                }
                Session.PreFunc_emM_EMSG = "";
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
