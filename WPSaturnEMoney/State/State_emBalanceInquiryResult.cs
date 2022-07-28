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
    class State_emBalanceInquiryResult : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emBalanceInquiryResult)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emBalanceInquiryResult;
                if (GlobalData.TransactionErrorType == Utilities.ErrorType.ProcessingNotCompleted)
                {
                    string msgCode = "F0005";
                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                    {
                        BtnVisibility = "Visible",
                        BtnContent = "電子マネー選択に戻る",
                        BtnCommand = new ViewModels.Command(ReturnToBalanceInquiry),
                    };
                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                    Session.MainViewModel.LoadScreen_emMessage(msgCode);

                    Session.TimerCount = Session.TimerCountRemaining;
                    while (Session.TimerCount >= 100)
                    {
                        await Task.Delay(100);
                        Session.TimerCount -= 100;
                    }
                    Session.ScreenState.NextState = StateMachine.State.emBalanceInquiry;
                }
                // if successful then Hide after timeout  
                else if (GlobalData.TransactionErrorType == Utilities.ErrorType.Success)
                {
                    // Utilities.PlaySound("emzf.wav");

                    decimal.TryParse(GlobalData.SaturnAPIResponse.bizInfo.cardBalance, out GlobalData.Balance);

                    Session.MainViewModel.LoadScreen_emBalanceInquiryResult(GlobalData.MsgCode);
                }

                Session.TimerCount = Session.TimerCountRemaining;
                while (Session.TimerCount > 0)
                {
                    if (Session.TimerCount <= 100)
                    {
                        // Customer timeout occurred
                        /*Utilities.Log.Error("Timeout customer operation!");
                        GlobalData.Data_ToPosDat.service = "EMONEY";
                        GlobalData.Data_ToPosDat.sequence = GlobalData.Data_FromPosDat.sequence;
                        GlobalData.Data_ToPosDat.last_operator = GlobalData.LastOperator;
                        GlobalData.Data_ToPosDat.result = "9"; // consumer operation timeout
                        GlobalData.Data_ToPosDat.SettledAmount = "0";
                        GlobalData.Data_ToPosDat.CurrentService = "";
                        GlobalData.Data_ToPosDat.statementID = "";
                        Session.TimerCount = 0;

                        await WinAPI.EndTransaction();*/
                        Session.ScreenState.NextState = StateMachine.State.emBalanceInquiry;
                    }
                    await Task.Delay(100, cancellationToken); // 100ms is for Task.Delay() to be more precise
                    Session.TimerCount -= 100;
                }
            }

            return Session.ScreenState.GoToNextState(this);
        }

        private void ReturnToBalanceInquiry()
        {
            Session.ScreenState.NextState = StateMachine.State.emBalanceInquiry;
            Session.TimerCount = 0;
        }
    }
}
