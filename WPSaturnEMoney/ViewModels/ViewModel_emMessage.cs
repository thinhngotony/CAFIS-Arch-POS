namespace WPSaturnEMoney.ViewModels
{
    class ViewModel_emMessage : BaseViewModel
    {
        public string BodyTextColor { get; set; } = "#254474";
        public string Message { get; set; } = "";
        public string ButtonBorderColor { get; set; } = "#254474";
        public string NormalButtonColor { get; set; } = "#EDEDED";
        public string NormalButtonLabelColor { get; set; } = "#254474";
        public string BtnContent { get; set; } = "";
        public string BtnVisibility { get; set; } = "Hidden";
        public System.Windows.Input.ICommand BtnCommand { get; set; } = new Command(Common.WinAPI.HideWindow);
        public string Btn2ndContent { get; set; } = "";
        public string Btn2ndVisibility { get; set; } = "Collapsed";
        public System.Windows.Input.ICommand Btn2ndCommand { get; set; } = new Command(Common.WinAPI.HideWindow);
        public override void UpdateView()
        {
        }
    }
}
