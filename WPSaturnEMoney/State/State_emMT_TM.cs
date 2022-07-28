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
    class State_emMT_TM : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emMT_TM)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emMT_TM;
                Session.MainViewModel.LoadScreen_emMT_TM();
            }

            if (Session.PreState_emMT_TM == "emMT_RV")
            {
                var readValueResult = await PaymentAPI.EMoney_ReadValue(GlobalData.ServiceName);
                if (readValueResult.Item1 == Utilities.ErrorType.Success)
                {
                    // if error type is ProcessingNotCompleted, go to emMT_ZRESJ
                    // otherwise, go to emMT_ZRES (also use emMT_ZRESJ but display another button)
                    Session.ScreenState.NextState = StateMachine.State.emMT_ZRESJ;
                }
                else
                {
                    GlobalData.ScreenSubErrorMsg = readValueResult.Item2;
                    Session.ScreenState.NextState = StateMachine.State.emMT_EMSG;
                    Session.PreFunc_emMT_EMSG = "read_value";
                }
            }
            else // Session.PreState_emMT_TM == "emMT_HV"
            {
                var cardHistoryResult = await PaymentAPI.EMoney_CardHistory(GlobalData.ServiceName);
                if (cardHistoryResult.Item1 == Utilities.ErrorType.Success)
                {
                    // if error type is ProcessingNotCompleted, go to emMT_HRESJ
                    // otherwise, go to emMT_HRES (also use emMT_HRESJ but display another button)
                    Session.ScreenState.NextState = StateMachine.State.emMT_HRESJ;
                }
                else
                {
                    GlobalData.ScreenSubErrorMsg = cardHistoryResult.Item2;
                    Session.ScreenState.NextState = StateMachine.State.emMT_EMSG;
                    Session.PreFunc_emMT_EMSG = "card_history";
                }
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
