using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPSaturnEMoney.State;

namespace WPSaturnEMoney.ViewModels
{
    class ViewModel_emMT_EMSG : BaseViewModel
    {
        public string BodyTextColor { get; set; }
        public string ButtonBorderColor { get; set; }
        public string NormalButtonColor { get; set; }
        public string NormalButtonLabelColor { get; set; }
        public string ErrorMessage { get; set; }
        public ICommand GoToScreen_emMT_MENU_Command { get; }
        public ViewModel_emMT_EMSG()
        {
            GoToScreen_emMT_MENU_Command = new Command(GoToScreen_emMT_MENU);
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
