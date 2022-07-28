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
    public class StateMachine
    {
        private IState _state;

        public State NextState { get; set; }
        public State CurrentState { get; set; }
        public List<State> PreviousStates { get; } = new List<State>();

        public Task AsyncStart(IState initialState, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => Start(initialState, cancellationToken), cancellationToken);
        }

        public async Task Start(IState initialState, CancellationToken cancellationToken = default)
        {
            _state = initialState;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    _state = await _state.Execute(cancellationToken);
                    await Task.Delay(10, cancellationToken);
                }
                catch (Exception ex)
                {
                    Session.ScreenState.NextState = State.emUnexpectedError;
                    Session.MaintenanceMode = "";
                    Utilities.Log.Error("▲▲ StateMachine throw exception: " + ex.ToString());
                    _state = GoToNextState(_state);
                }
            } while (_state != null);
        }

        public void SetPreState(State state)
        {
            if (PreviousStates.Count >= 3)
            {
                PreviousStates.RemoveRange(2, PreviousStates.Count - 2);
            }
            PreviousStates.Insert(0, state);
        }

        public IState GoToNextState(IState currentState)
        {
            switch (NextState)
            {
                case State.emInit:
                    return new State_emInit();
                case State.emIdle:
                    return new State_emIdle();
                case State.emStandBy:
                    return new State_emStandBy();
                case State.emPayment:
                    return new State_emPayment();
                case State.emPaymentResult: 
                    return new State_emPaymentResult();
                case State.emProcessOperation:
                    return new State_emProcessOperation();
                case State.emCancelOperation:
                    return new State_emCancelOperation();
                case State.emF:
                    return new State_emF();
                case State.emMAorMI_EMSG:
                    return new State_emMAorMI_EMSG();
                case State.emCM:
                    return new State_emCM();
                case State.emUnexpectedError:
                    return new State_emUnexpectedError();
                case State.emErrorToMaintenance:
                    return new State_emErrorToMaintenance();
                case State.emLoginToMaintenance:
                    return new State_emLoginToMaintenance();
                case State.emMT_MSG:
                    return new State_emMT_MSG();
                case State.emMS_MENU:
                    return new State_emMS_MENU();
                case State.emES_MENU:
                    return new State_emES_MENU();
                case State.emMSorMT_TA:
                    return new State_emMSorMT_TA();
                case State.emBalanceInquiry:
                    return new State_emBalanceInquiry();
                case State.emBalanceInquiryResult:
                    return new State_emBalanceInquiryResult();
                case State.emMaintenanceMenuFromError:
                    return new State_emMaintenanceMenuFromError();
                case State.emMT_RV:
                    return new State_emMT_RV();
                case State.emMT_HV:
                    return new State_emMT_HV();
                case State.emMT_TM:
                    return new State_emMT_TM();
                case State.emMT_ZRESJ:
                    return new State_emMT_ZRESJ();
                case State.emMT_HRESJ:
                    return new State_emMT_HRESJ();
                case State.emMT_EMSG:
                    return new State_emMT_EMSG();
                case State.emMT_SMSG:
                    return new State_emMT_SMSG();
                case State.emMT_ZJG:
                    return new State_emMT_ZJG();
                case State.emMT_HJG:
                    return new State_emMT_HJG();
                case State.emMT_JGC1:
                    return new State_emMT_JGC1();
                case State.emMT_JGC2:
                    return new State_emMT_JGC2();
                case State.emM_TA:
                    return new State_emM_TA();
                case State.emM_SMSG:
                    return new State_emM_SMSG();
                case State.emM_EMSG:
                    return new State_emM_EMSG();
                case State.emM_CH:
                    return new State_emM_CH();
                case State.emM_HRES:
                    return new State_emM_HRES();
                case State.emMT_F:
                    return new State_emMT_F();
                case State.emMaintenanceMenu:
                    return new State_emMaintenanceMenu();
                case State.emM_RV:
                    return new State_emM_RV();
                case State.emM_TM:
                    return new State_emM_TM();
                case State.emM_ZRES:
                    return new State_emM_ZRES();
                case State.emM_ZRESJ:
                    return new State_emM_ZRESJ();
                case State.emM_HRESJ:
                    return new State_emM_HRESJ();
                case State.emM_ZJG:
                    return new State_emM_ZJG();
                case State.emM_HJG:
                    return new State_emM_HJG();
                case State.emM_JGC1:
                    return new State_emM_JGC1();
                case State.emM_JGC2:
                    return new State_emM_JGC2();
                case State.emM_TTLC:
                    return new State_emM_TTLC();
                case State.emM_F:
                    return new State_emM_F();
                default:
                    return currentState;
            }
        }

        public enum State
        {
            InitApp,
            emInit,
            emIdle,
            emStandBy,
            emPayment,
            emProcessOperation,
            emCancelOperation,
            emPaymentResult, 
            emF,
            emMAorMI_EMSG,
            emCM,
            emUnexpectedError,
            emErrorToMaintenance,
            emLoginToMaintenance,
            emMT_MSG,
            emMSorMT_TA,
            emMS_MENU,
            emES_MENU,
            emBalanceInquiry,
            emBalanceInquiryResult,
            emMaintenanceMenuFromError,
            emMT_RV,
            emMT_HV,
            emMT_TM,
            emMT_ZRESJ,
            emMT_HRESJ,
            emMT_EMSG,
            emMT_SMSG,
            emMT_ZJG,
            emMT_HJG,
            emMT_JGC1,
            emMT_JGC2,
            emMT_F,
            emM_TA,
            emM_SMSG,
            emM_EMSG,
            emM_CH,
            emMaintenanceMenu,
            emM_RV,
            emM_TM,
            emM_ZRES,
            emM_ZRESJ,
            emM_ZJG,
            emM_HRES,
            emM_HRESJ,
            emM_HJG,
            emM_JGC1,
            emM_JGC2,
            emM_TTLC,
            emM_F
        }
    }
}
