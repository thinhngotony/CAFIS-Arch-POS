using System.Windows.Input;
using WPSaturnEMoney.Common;

namespace WPSaturnEMoney.ViewModels
{
    class ViewModel_emPaymentResult : BaseViewModel
    {
        public string BodyTextColor { get; set; }
        public System.Collections.Generic.List<string> ListMessage { get; set; }
        public string PaymentRequestAmount { get; set; }
        public string PaymentSettlementAmount { get; set; }
        public string RemainingAmount { get; set; }
        public string BtnVisibility { get; set; }
        public string ButtonBorderColor { get; set; }
        public string NormalButtonColor { get; set; }
        public string NormalButtonLabelColor { get; set; }
        public string BtnContent { get; set; }
        public ICommand BtnCommand { get; set; }

        public ViewModel_emPaymentResult()
        {
            BtnCommand = new Command(ReturnToPaymentTerminalScreen);  
        }
        private void ReturnToPaymentTerminalScreen()
        {
            Utilities.Log.Info($"Press button [{BtnContent}]");
        }
        public override void UpdateView()
        {
        }
    }
}
