using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;

namespace WPSaturnEMoney.State
{
    class State_emErrorToMaintenance : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emErrorToMaintenance)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emErrorToMaintenance;
                Session.MainViewModel.LoadScreen_emErrorToMaintenance(GlobalData.MsgCode);

                if (Session.ScreenState.PreviousStates[0] == StateMachine.State.emPayment)
                {
                    // Utilities.PlaySound("emmt1.wav");
                }
                else // Session.PreState_emTM == "emZ"
                {
                    // Utilities.PlaySound("emmt2.wav");
                }

                /*// Get error message
                string msg;
                if (GlobalData.ApplicationError.Length > 0) msg = GlobalData.ApplicationErrorMsg;
                else msg = GlobalData.ScreenErrorMsg;

                // Get error code from error message (the first [...] in the message)
                if (msg is null) msg = ""; // avoid null exception at regex.Match()
                Regex regex = new Regex(@"\[(.*?)\]");
                Match match = regex.Match(msg);
                string errorCode = match.Value.Trim('[', ']');

                // Clerk call notification to the cashier app
                WinAPI.ClerkCallNotification(errorCode);*/
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
