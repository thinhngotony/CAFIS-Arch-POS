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
    class State_emM_F : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emM_F)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emM_F;
                Session.MainViewModel.LoadScreen_emM_F();

                // Get PreSettledAmountSV from UnprocessedReceipt before PrintConvertedReceipt
                // because the data will be cleared after printed (PrintRetry response datas have no SettledAmount property)
                string keyLabel;
                switch (GlobalData.PreCurrentServiceSV)
                {
                    case "Suica":
                        keyLabel = "交通系支払"; break;
                    case "Edy":
                        keyLabel = "Edy支払"; break;
                    case "nanaco":
                        keyLabel = "nanaco支払"; break;
                    case "WAON":
                        keyLabel = "WAON支払"; break;
                    default:
                        keyLabel = ""; break;
                }
                if (keyLabel == "")
                {
                    GlobalData.PreSettledAmountSV = 0;
                    Utilities.Log.Error($"Unable to get amount from unprocessed receipt -> PreSettledAmountSV: 0 !!!");
                }
                else
                {
                    foreach (var row in PaymentAPI.UnprocessedReceipt)
                    {
                        if (row.Label == keyLabel)
                        {
                            string amount = string.Join("", row.Value.Where(char.IsDigit));
                            GlobalData.PreSettledAmountSV = decimal.Parse(amount);
                            Utilities.Log.Error($"Get amount from unprocessed receipt -> PreSettledAmountSV: {GlobalData.PreSettledAmountSV}");
                            break;
                        }

                        if (row.Equals(PaymentAPI.UnprocessedReceipt.Last()))
                        {
                            GlobalData.PreSettledAmountSV = 0;
                            Utilities.Log.Error($"Not found amount from unprocessed receipt -> PreSettledAmountSV: 0 !!!");
                        }
                    }
                }

                // Create and print new receipt from unfinished progressing data (ConvReceipt)
                Utilities.Log.Info($"Get ConvertReceipt, PreCurrentServiceSV: {GlobalData.PreCurrentServiceSV}");
                var receiptData = Utilities.ConvertReceipt();
                // Utilities.PrintConvertedReceipt(receiptData);
                if (GlobalData.IsConvertReceiptError)
                {
                    Utilities.Log.Error("▲ Convert Receipt Error!");
                }

                Utilities.UpdateTotalInfo(GlobalData.PreCurrentServiceSV, GlobalData.PreSettledAmountSV, Utilities.TotalRecordType.NASS);

                Session.ScreenState.NextState = StateMachine.State.emMaintenanceMenu;
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
