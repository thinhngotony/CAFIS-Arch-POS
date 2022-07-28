using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Models;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.ViewModels;

namespace WPSaturnEMoney.State
{
    class State_emPaymentResult : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);

            if (Session.ScreenState.CurrentState != StateMachine.State.emPaymentResult)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emPaymentResult;
                string msgCode = "";
                // If Timeout then Hide after Timeout 
                if (GlobalData.TransactionErrorType == Utilities.ErrorType.Timeout)
                {
                    msgCode = "D0008";
                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties();
                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                    Session.MainViewModel.LoadScreen_emMessage(msgCode);

                    await Task.Delay((int)GlobalData.BasicConfig.settlement_close_time * 1000);

                    // Customer timeout occurred
                    Utilities.Log.Error("Timeout settlement choice!");
                    GlobalData.Data_ToPosDat.service = GlobalData.Data_FromPosDat.service;
                    GlobalData.Data_ToPosDat.sequence = GlobalData.Data_FromPosDat.sequence;
                    GlobalData.Data_ToPosDat.last_operator = GlobalData.LastOperator;

                    GlobalData.Data_ToPosDat.result = "0";
                    GlobalData.Data_ToPosDat.SettledAmount = "";
                    GlobalData.Data_ToPosDat.CurrentService = "";
                    GlobalData.Data_ToPosDat.statementID = "";
                    Session.TimerCount = 0;
                    await WinAPI.EndTransaction();
                }
                // if discontinue then return to emP (similar to cCM)  
                else if (GlobalData.TransactionErrorType == Utilities.ErrorType.ProcessingNotCompleted)
                {
                    msgCode = "E0003";
                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                    {
                        BtnVisibility = "Visible",
                        BtnContent = "支払選択に戻る",
                        BtnCommand = new Command(ReturnToPayment),
                    };
                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                    Session.MainViewModel.LoadScreen_emMessage(msgCode);
                    
                    Session.TimerCount = Session.TimerCountRemaining;
                    while (Session.TimerCount >= 100)
                    {
                        await Task.Delay(100);
                        Session.TimerCount -= 100;
                    }
                    Session.ScreenState.NextState = StateMachine.State.emPayment;
                }
                // if successful then Hide after timeout  
                else if (GlobalData.TransactionErrorType == Utilities.ErrorType.Success)
                {
                    string[] spearator = { "\t" };
                    if (GlobalData.Data_FromPosDat.amount != GlobalData.SaturnAPIResponse.bizInfo.tradeAmount)
                    {
                        msgCode = "E0018";
                        GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                        {
                            BtnVisibility = "Visible",
                            BtnContent = "終了",
                            BtnCommand = new Command(PaymentSuccess),
                        };
                        GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties
                        {
                            BtnVisibility = "Visible",
                            BtnContent = "終了",
                            BtnCommand = new Command(PaymentSuccess),
                        };
                        Session.MainViewModel.LoadScreen_emPaymentResult(msgCode);

                        Session.TimerCount = (int)GlobalData.BasicConfig.settlement_close_time_02 * 1000;
                        while (Session.TimerCount > 0)
                        {
                            if (Session.TimerCount <= 100)
                            {
                                Utilities.Log.Info("Payment successfully with insufficient amount!");
                                GlobalData.Data_ToPosDat.service = GlobalData.Data_FromPosDat.service;
                                GlobalData.Data_ToPosDat.sequence = GlobalData.Data_FromPosDat.sequence;
                                GlobalData.Data_ToPosDat.last_operator = GlobalData.LastOperator;
                                GlobalData.Data_ToPosDat.result = "1"; // payment successful
                                GlobalData.Data_ToPosDat.SettledAmount = GlobalData.SaturnAPIResponse.bizInfo.tradeAmount.ToString();
                                GlobalData.Data_ToPosDat.CurrentService = GlobalData.ServiceName;
                                GlobalData.Data_ToPosDat.statementID = GlobalData.statementID;

                                await WinAPI.EndTransaction();
                            }
                            await Task.Delay(100, cancellationToken); // 100ms is for Task.Delay() to be more precise
                            Session.TimerCount -= 100;
                        }
                    }
                    else
                    {
                        msgCode = "E0008";
                        GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                        {
                            BtnVisibility = "Visible",
                            BtnContent = "終了",
                            BtnCommand = new Command(PaymentSuccess),
                        };
                        GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties
                        {
                            BtnVisibility = "Visible",
                            BtnContent = "終了",
                            BtnCommand = new Command(PaymentSuccess),
                        };
                        Session.MainViewModel.LoadScreen_emPaymentResult(msgCode);

                        Session.TimerCount = (int)GlobalData.BasicConfig.settlement_close_time_02 * 1000;
                        while (Session.TimerCount > 0)
                        {
                            if (Session.TimerCount <= 100)
                            {
                                Utilities.Log.Info("Payment successfully!");
                                GlobalData.Data_ToPosDat.service = GlobalData.Data_FromPosDat.service;
                                GlobalData.Data_ToPosDat.sequence = GlobalData.Data_FromPosDat.sequence;
                                GlobalData.Data_ToPosDat.last_operator = GlobalData.LastOperator;
                                GlobalData.Data_ToPosDat.result = "1"; // payment successful
                                GlobalData.Data_ToPosDat.SettledAmount = GlobalData.SaturnAPIResponse.bizInfo.tradeAmount.ToString();
                                GlobalData.Data_ToPosDat.CurrentService = GlobalData.ServiceName;
                                GlobalData.Data_ToPosDat.statementID = GlobalData.statementID;

                                await WinAPI.EndTransaction();
                            }
                            await Task.Delay(100, cancellationToken); // 100ms is for Task.Delay() to be more precise
                            Session.TimerCount -= 100;
                        }
                    }
                }
            }

            return Session.ScreenState.GoToNextState(this);
        }

        private void ReturnToPayment()
        {
            Session.ScreenState.NextState = StateMachine.State.emPayment;
            Session.TimerCount = 0;
        }

        private async void PaymentSuccess()
        {
            Utilities.Log.Info("Payment successfully!");
            GlobalData.Data_ToPosDat.service = GlobalData.Data_FromPosDat.service;
            GlobalData.Data_ToPosDat.sequence = GlobalData.Data_FromPosDat.sequence;
            GlobalData.Data_ToPosDat.last_operator = GlobalData.LastOperator;
            GlobalData.Data_ToPosDat.result = "1"; // payment successful
            GlobalData.Data_ToPosDat.SettledAmount = GlobalData.SaturnAPIResponse.bizInfo.tradeAmount.ToString();
            GlobalData.Data_ToPosDat.CurrentService = GlobalData.ServiceName;
            GlobalData.Data_ToPosDat.statementID = GlobalData.statementID;

            await WinAPI.EndTransaction();
            Session.TimerCount = 0;
        }
    }
}
