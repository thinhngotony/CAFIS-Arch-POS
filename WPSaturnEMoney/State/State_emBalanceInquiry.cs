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
    class State_emBalanceInquiry : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emBalanceInquiry)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emBalanceInquiry;

                string msgCode = "";
                bool isBtnOrderReadValue = true;
                if (GlobalData.BasicConfig.btn_order_ReadValue.Length == 0)
                {
                    isBtnOrderReadValue = false;
                    Utilities.Log.Error("▲ Config.json >> btn_order_ReadValue: list of services is empty.");
                }
                else
                {
                    int cnt = 0;
                    foreach (var service in GlobalData.BasicConfig.btn_order_ReadValue)
                    {
                        string brandName = Utilities.GetBrandName(service);
                        if (GlobalData.ServiceNameCorrect.Contains(service) && !string.IsNullOrEmpty(brandName))
                        {
                            cnt++;
                            break;
                        }
                    }
                    if (cnt == 0)
                    {
                        isBtnOrderReadValue = false;
                        Utilities.Log.Error("▲ Config.json >> btn_order_ReadValue: there are no services that match the services of system and brand_code in ServiceId.json.");
                    }
                }

                if (!isBtnOrderReadValue)
                {
                    msgCode = "F0001";
                    FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                    {
                        MessageMaintenance = msgMode.msg2,
                    };
                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties
                    {
                        MessageMaintenance = msgMode.msg3,
                    };
                    GlobalData.MsgCode = msgCode;
                    Session.ScreenState.NextState = StateMachine.State.emErrorToMaintenance;
                }
                else
                {
                    msgCode = "F0002";
                    GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties();
                    GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                    Session.MainViewModel.LoadScreen_emBalanceInquiry(msgCode);

                    // Utilities.PlaySound("emz.wav");

                    Session.TimerCount = (int)GlobalData.BasicConfig.settlement_choice_timeout * 1000;
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
                            Session.ScreenState.NextState = StateMachine.State.emPayment;
                        }
                        await Task.Delay(100, cancellationToken); // 100ms is for Task.Delay() to be more precise
                        Session.TimerCount -= 100;
                    }
                }
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
