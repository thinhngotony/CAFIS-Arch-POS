using System.Windows.Input;
using WPSaturnEMoney.Common;

namespace WPSaturnEMoney.ViewModels
{
    class ViewModel_emErrorToMaintenance : BaseViewModel
    {
        public string BodyTextColor { get; set; }
        public string Message { get; set; }
        public string BtnVisibility { get; set; } = "Hidden";
        public string ButtonBorderColor { get; set; } = "#254474";
        public string NormalButtonColor { get; set; } = "#EDEDED";
        public string NormalButtonLabelColor { get; set; } = "#254474";
        public string BtnContent { get; set; } = "";
        public ICommand BtnCommand { get; set; } = new Command(WinAPI.HideWindow);
        public override void UpdateView()
        {
        }
    }
}
