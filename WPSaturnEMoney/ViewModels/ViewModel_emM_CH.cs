using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPSaturnEMoney.State;

namespace WPSaturnEMoney.ViewModels
{
    class ViewModel_emM_CH : BaseViewModel
    {
        public string TitleColor { get; set; }
        public string ButtonBorderColor { get; set; }
        public string BrandButtonColor { get; set; }
        public string BrandButtonLabelColor { get; set; }
        public string BackButtonColor { get; set; }
        public string BackButtonLabelColor { get; set; }
        public string[] BtnContent { get; set; }
        public string[] BtnVisibility { get; set; }
        public ICommand GoToScreen_emM_TM_Command { get; }
        public ICommand GoToScreen_emM_MENU_Command { get; }

        public ViewModel_emM_CH()
        {
            GoToScreen_emM_TM_Command = new Command(GoToScreen_emM_TM);
            GoToScreen_emM_MENU_Command = new Command(GoToScreen_emM_MENU);
        }

        public void GoToScreen_emM_TM()
        {
            Session.PreState_emM_TM = "emM_CH";
            Session.ScreenState.NextState = StateMachine.State.emM_TM;
            Session.TimerCount = 0;
        }
        public void GoToScreen_emM_MENU()
        {
            Common.Utilities.Log.Info("Press button [戻る]");
            Session.ScreenState.NextState = StateMachine.State.emMaintenanceMenu;
            Session.TimerCount = 0;
        }

        public override void UpdateView()
        {
        }
    }
}
