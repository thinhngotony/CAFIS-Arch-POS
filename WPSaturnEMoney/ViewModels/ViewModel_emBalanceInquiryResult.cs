using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPSaturnEMoney.State;

namespace WPSaturnEMoney.ViewModels
{
    class ViewModel_emBalanceInquiryResult : BaseViewModel
    {
        public string TitleColor { get; set; }
        public string ButtonBorderColor { get; set; }
        public string NormalButtonColor { get; set; }
        public string NormalButtonLabelColor { get; set; }
        public string BodyTextColor { get; set; }
        public string NumberColor { get; set; }
        public string BorderLineColor { get; set; }
        public string BtnVisibility { get; set; } = "Hidden";
        public ICommand GoToScreen_emBalanceInquiry_Command { get; }
        public string Btn2ndVisibility { get; set; } = "Collapsed";
        public string Btn2ndContent { get; set; } = "";
        public ICommand Btn2ndCommand { get; set; } = new Command(Common.WinAPI.HideWindow);
        public string MessageVisibility { get; set; } = "Collapsed";
        public string Message { get; set; } = "";
        public string TotalPayment { get; set; }
        public string BrandName { get; set; }
        public string Balance { get; set; }
        public ViewModel_emBalanceInquiryResult()
        {
            GoToScreen_emBalanceInquiry_Command = new Command(GoToScreen_emBalanceInquiry);
        }

        public void GoToScreen_emBalanceInquiry()
        {
            Common.Utilities.Log.Info("Press button [電子マネー選択に戻る]");
            Session.ScreenState.NextState = StateMachine.State.emBalanceInquiry;
            Session.TimerCount = 0;
        }

        public override void UpdateView()
        {
        }
    }
}
