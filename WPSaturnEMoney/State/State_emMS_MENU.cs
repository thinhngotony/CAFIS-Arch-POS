using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Common;

namespace WPSaturnEMoney.State
{
    class State_emMS_MENU : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emMS_MENU)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emMS_MENU;
                Session.MainViewModel.LoadScreen_emMS_MENU();

                if (Session.PreFunc_emMS_MENU == "login")
                {
                    // Print receipt (if any)
                    // Utilities.PrintReceiptOperation();
                }
                Session.PreFunc_emMS_MENU = "";
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
