using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Common;

namespace WPSaturnEMoney.State
{
    class State_emMT_SMSG : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emMT_SMSG)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emMT_SMSG;
                Session.MainViewModel.LoadScreen_emMT_SMSG();

                if (Session.PreFunc_emMT_SMSG == "reprint_receipt")
                {
                    // Utilities.PrintReceiptOperation();
                }
                else if (Session.PreFunc_emMT_SMSG == "reprint_conv_receipt")
                {
                    // Print receipt information in ConvertedReceipt.dat
                    // Utilities.ReprintConvertedReceipt();
                }
                Session.PreFunc_emMT_SMSG = "";
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
