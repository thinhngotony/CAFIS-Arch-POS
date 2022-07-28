using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Common;

namespace WPSaturnEMoney.State
{
    class State_emM_SMSG : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emM_SMSG)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emM_SMSG;
                Session.MainViewModel.LoadScreen_emM_SMSG();

                if (Session.PreFunc_emM_SMSG == "reprint_receipt" ||
                    Session.PreFunc_emM_SMSG == "daily_total")
                {
                    // Utilities.PrintReceiptOperation();
                }
                else if (Session.PreFunc_emM_SMSG == "reprint_conv_receipt")
                {
                    // Print receipt information in ConvertedReceipt.dat
                    // Utilities.ReprintConvertedReceipt();
                }
                Session.PreFunc_emM_SMSG = "";
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
