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
    class State_emM_TM : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emM_TM)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emM_TM;
                Session.MainViewModel.LoadScreen_emM_TM();
            }

            if (Session.PreState_emM_TM == "emM_RV")
            {
                var readValueResult = await PaymentAPI.EMoney_ReadValue(GlobalData.ServiceName);
                if (readValueResult.Item1 == Utilities.ErrorType.Success)
                {
                    // If Processing Not Completed error is got from Reprint Receipt, go to the judgement screen
                    if (Session.IsProcNotCmpltAtStandby && GlobalData.ServiceName == GlobalData.PreCurrentServiceSV)
                    {
                        Session.ScreenState.NextState = StateMachine.State.emM_ZRESJ;
                        Session.IsProcNotCmpltAtStandby = false;
                    }
                    else
                    {
                        Session.ScreenState.NextState = StateMachine.State.emM_ZRES;
                    }
                }
                else
                {
                    GlobalData.ScreenMErrorMsg = readValueResult.Item2;
                    Session.ScreenState.NextState = StateMachine.State.emM_EMSG;
                    Session.PreFunc_emM_EMSG = "read_value";
                }
            }
            else // Session.PreState_emM_TM == "emM_CH"
            {
                var cardHistoryResult = await PaymentAPI.EMoney_CardHistory(GlobalData.ServiceName);
                if (cardHistoryResult.Item1 == Utilities.ErrorType.Success)
                {
                    // If Processing Not Completed error is got from Reprint Receipt, go to the judgement screen
                    if (Session.IsProcNotCmpltAtStandby && GlobalData.ServiceName == GlobalData.PreCurrentServiceSV)
                    {
                        Session.ScreenState.NextState = StateMachine.State.emM_HRESJ;
                        Session.IsProcNotCmpltAtStandby = false;
                    }
                    else
                    {
                        Session.ScreenState.NextState = StateMachine.State.emM_HRES;
                    }
                }
                else
                {
                    GlobalData.ScreenMErrorMsg = cardHistoryResult.Item2;
                    Session.ScreenState.NextState = StateMachine.State.emM_EMSG;
                    Session.PreFunc_emM_EMSG = "card_history";
                }
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
