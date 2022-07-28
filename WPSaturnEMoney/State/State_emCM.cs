using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;

namespace WPSaturnEMoney.State
{
    class State_emCM : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emCM)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emCM;
                Session.MainViewModel.LoadScreen_emCM();

                if (Session.PreState_emCM == "emTM" && Session.PreState_emTM == "emP")
                {
                    // Utilities.PlaySound("emcm1.wav");
                }
                else if (Session.PreState_emCM == "emTM" && Session.PreState_emTM == "emZ")
                {
                    // Utilities.PlaySound("emcm2.wav");
                }
            }

            // Display message for customer_error_wait (ms)
            Session.TimerCount = (int)GlobalData.BasicConfig.customer_error_wait > 0 ? (int)GlobalData.BasicConfig.customer_error_wait : 0;
            while (Session.TimerCount >= 100)
            {
                await Task.Delay(100);
                Session.TimerCount -= 100;
            }

            if ((Session.PreState_emCM == "emTM" && Session.PreState_emTM == "emP") ||
                Session.PreState_emCM == "emS" || Session.PreState_emCM == "emP")
            {
                GlobalData.Data_ToPosDat.service = "EMONEY";
                GlobalData.Data_ToPosDat.sequence = GlobalData.Data_FromPosDat.sequence;
                GlobalData.Data_ToPosDat.last_operator = GlobalData.LastOperator;
                GlobalData.Data_ToPosDat.result = "0"; // canceled
                GlobalData.Data_ToPosDat.SettledAmount = "0";
                GlobalData.Data_ToPosDat.CurrentService = "";
                GlobalData.Data_ToPosDat.statementID = "";

                await WinAPI.EndTransaction();
                Session.PreState_emTM = "";
                Session.PreState_emCM = "";
            }
            else if (Session.PreState_emCM == "emTM" && Session.PreState_emTM == "emZ")
            {
                Session.ScreenState.NextState = StateMachine.State.emPayment;
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
