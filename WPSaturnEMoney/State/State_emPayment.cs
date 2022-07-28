using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;

namespace WPSaturnEMoney.State
{
    class State_emPayment : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emPayment)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emPayment;
                using (StreamWriter writer = new StreamWriter(GlobalData.AppPath + GlobalData.LastSettlementBrandCodePath, false))
                {
                    writer.Write(string.Empty);
                }
                Utilities.Log.Info("Cleared LastSettlementBrandCode.dat file.");
                using (StreamWriter writer = new StreamWriter(GlobalData.AppPath + GlobalData.LastSettlementSeqNoPath, false))
                {
                    writer.Write(string.Empty);
                }
                Utilities.Log.Info("Cleared LastSettlementSeqNo.dat file.");

                decimal.TryParse(GlobalData.Data_FromPosDat.amount, out GlobalData.TotalPayment);
                Utilities.Log.Info("Reset Payment Request Amount to The amount of the bill from the POS cash register!");

                string msgCode = "D0007";
                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties();
                GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                Session.MainViewModel.LoadScreen_emPayment(msgCode);

                // Utilities.PlaySound("emp.wav");

                Session.TimerCount = (int)GlobalData.BasicConfig.settlement_choice_timeout * 1000;
                while (Session.TimerCount > 0)
                {
                    if (Session.TimerCount <= 100)
                    {
                        //// Customer timeout occurred
                        //Utilities.Log.Error("Timeout customer operation!");
                        //GlobalData.Data_ToPosDat.service = "EMONEY";
                        //GlobalData.Data_ToPosDat.sequence = GlobalData.Data_FromPosDat.sequence;
                        //GlobalData.Data_ToPosDat.last_operator = GlobalData.LastOperator;

                        //// TODO: change Data_ToPosDat.result to "0" 
                        //GlobalData.Data_ToPosDat.result = "9"; // consumer operation timeout
                        //GlobalData.Data_ToPosDat.SettledAmount = "0";
                        //GlobalData.Data_ToPosDat.CurrentService = "";
                        //GlobalData.Data_ToPosDat.statementID = "";
                        //Session.TimerCount = 0;
                        //await WinAPI.EndTransaction();

                        GlobalData.TransactionErrorType = Utilities.ErrorType.Timeout;
                        Session.ScreenState.NextState = StateMachine.State.emPaymentResult;
                    }
                    await Task.Delay(100, cancellationToken); // 100ms is for Task.Delay() to be more precise
                    Session.TimerCount -= 100;
                }
            }
            return Session.ScreenState.GoToNextState(this);
        }
    }
}
