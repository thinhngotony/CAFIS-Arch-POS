using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Common;

namespace WPSaturnEMoney.State
{
    class State_emMT_EMSG : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emMT_EMSG)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emMT_EMSG;
                Session.MainViewModel.LoadScreen_emMT_EMSG();

                if (Session.PreFunc_emMT_EMSG == "reprint_receipt" ||
                    Session.PreFunc_emMT_EMSG == "read_value" ||
                    Session.PreFunc_emMT_EMSG == "card_history")
                {
                    // Print receipt (if any)
                    // Utilities.PrintReceiptOperation();
                }
                Session.PreFunc_emMT_EMSG = "";
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
