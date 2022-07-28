using System.Windows.Input;
using WPSaturnEMoney.State;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;

namespace WPSaturnEMoney.ViewModels
{
    public class ViewModel_emPayment : BaseViewModel
    {
        public string TitleColor { get; set; }
        public string ButtonBorderColor { get; set; }
        public string BrandButtonColor { get; set; }
        public string BrandButtonLabelColor { get; set; }
        public string NormalButtonColor { get; set; }
        public string NormalButtonLabelColor { get; set; }
        public string BackButtonColor { get; set; }
        public string BackButtonLabelColor { get; set; }
        public ICommand GoToScreen_emTM_Command { get; }
        public ICommand GoToScreen_emIdle_Command { get; set; }
        public ICommand GoToScreen_emBalanceInquiry_Command { get; }
        public string TotalPayment { get; set; }

        public ViewModel_emPayment()
        {
            GoToScreen_emTM_Command = new Command(GoToScreen_emTM);
            GoToScreen_emIdle_Command = new Command(GoToScreen_emIdle);
            GoToScreen_emBalanceInquiry_Command = new Command(GoToScreen_emBalanceInquiry);
        }

        public void GoToScreen_emTM()
        {
            //Session.ScreenState.SetPreState(StateMachine.State.emPayment);
            //Session.ScreenState.NextState = StateMachine.State.emTM;
            //Session.TimerCount = 0;
        }
        public async void GoToScreen_emIdle()
        {
            Utilities.Log.Info("Press button [戻る]");

            GlobalData.Data_ToPosDat.service = GlobalData.Data_FromPosDat.service;
            GlobalData.Data_ToPosDat.sequence = GlobalData.Data_FromPosDat.sequence;
            GlobalData.Data_ToPosDat.last_operator = GlobalData.LastOperator;
            GlobalData.Data_ToPosDat.result = "0"; // canceled
            GlobalData.Data_ToPosDat.SettledAmount = "";
            GlobalData.Data_ToPosDat.CurrentService = "";
            GlobalData.Data_ToPosDat.statementID = "";

            await WinAPI.EndTransaction();

            Session.TimerCount = 0;
        }
        public void GoToScreen_emBalanceInquiry()
        {
            Utilities.Log.Info("Press button [電子マネー残高照会]");

            /*if (FileStruct.IsBtnOrderConfigError("ReadValue").Item1)
            {
                Session.PreState_emCM = "emP";
                Session.ScreenState.NextState = StateMachine.State.emCM;
                GlobalData.ScreenErrorMsg = FileStruct.IsBtnOrderConfigError("ReadValue").Item2;
                GlobalData.BtnOrderConfigErrors.Clear();
                GlobalData.ApplicationErrorMsg = "";
                GlobalData.ApplicationError = "";
            }
            else*/
            {
                Session.ScreenState.NextState = StateMachine.State.emBalanceInquiry;
            }
            Session.TimerCount = 0;
        }

        public override void UpdateView()
        {
        }
    }
}
