using WPSaturnEMoney.State;

namespace WPSaturnEMoney.ViewModels
{
    class ViewModel_emBalanceInquiry : BaseViewModel
    {
        public string TitleColor { get; set; }
        public string ButtonBorderColor { get; set; }
        public string BrandButtonColor { get; set; }
        public string BrandButtonLabelColor { get; set; }
        public string BackButtonColor { get; set; }
        public string BackButtonLabelColor { get; set; }
        public string[] BtnContent { get; set; }
        public string[] BtnVisibility { get; set; }
        // public ICommand GoToScreen_emTM_Command { get; }
        public System.Windows.Input.ICommand GoToScreen_emPayment_Command { get; }
        public string TotalPayment { get; set; }

        public ViewModel_emBalanceInquiry()
        {
            // GoToScreen_emTM_Command = new Command(GoToScreen_emTM);
            GoToScreen_emPayment_Command = new Command(GoToScreen_emPayment);
        }

        /*public void GoToScreen_emTM()
        {
            Session.ScreenState.SetPreState(StateMachine.State.emBalanceInquiry);
            Session.ScreenState.NextState = StateMachine.State.emPaymentOperation;
            Session.TimerCount = 0;
        }*/
        public void GoToScreen_emPayment()
        {
            Common.Utilities.Log.Info("Press button [戻る]");
            Session.ScreenState.NextState = StateMachine.State.emPayment;
            Session.TimerCount = 0;
        }

        public override void UpdateView()
        {
        }
    }
}
