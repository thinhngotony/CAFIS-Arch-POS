using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPSaturnEMoney.State;

namespace WPSaturnEMoney.ViewModels
{
    class ViewModel_emMT_HV : BaseViewModel
    {
        public string TitleColor { get; set; }
        public string ButtonBorderColor { get; set; }
        public string BrandButtonColor { get; set; }
        public string BrandButtonLabelColor { get; set; }
        public string BackButtonColor { get; set; }
        public string BackButtonLabelColor { get; set; }
        public string[] BtnContent { get; set; }
        public string[] BtnVisibility { get; set; }
        public ICommand GoToScreen_emMT_TM_Command { get; }
        public ICommand GoToScreen_emMT_MENU_Command { get; }

        public ViewModel_emMT_HV()
        {
            GoToScreen_emMT_TM_Command = new Command(GoToScreen_emMT_TM);
            GoToScreen_emMT_MENU_Command = new Command(GoToScreen_emMT_MENU);
        }

        public void GoToScreen_emMT_TM()
        {
            Session.PreState_emMT_TM = "emMT_HV";
            Session.ScreenState.NextState = StateMachine.State.emMT_TM;
            Session.TimerCount = 0;
        }
        public void GoToScreen_emMT_MENU()
        {
            Common.Utilities.Log.Info("Press button [戻る]");
            Session.ScreenState.NextState = StateMachine.State.emMaintenanceMenuFromError;
            Session.TimerCount = 0;
        }

        public override void UpdateView()
        {
        }
    }
}
