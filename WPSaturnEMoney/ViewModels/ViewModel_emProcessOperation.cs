using System.Collections.Generic;

namespace WPSaturnEMoney.ViewModels
{
    class ViewModel_emProcessOperation : BaseViewModel
    {
        public string BodyTextColor { get; set; }
        public List<string> ListMessage { get; set; }
        public string BtnVisibility { get; set; } = "Hidden";
        public string BtnContent { get; set; } = "";
        public System.Windows.Input.ICommand BtnCommand { get; set; } = new Command(Common.WinAPI.HideWindow);
        public string ButtonBorderColor { get; set; }
        public string BackButtonColor { get; set; }
        public string BackButtonLabelColor { get; set; }
        public override void UpdateView()
        {
        }
    }
}
