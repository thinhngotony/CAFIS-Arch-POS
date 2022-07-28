using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPSaturnEMoney.State;

namespace WPSaturnEMoney.ViewModels
{
    class ViewModel_emM_ZRES : BaseViewModel
    {
        public string TitleColor { get; set; }
        public string ButtonBorderColor { get; set; }
        public string NormalButtonColor { get; set; }
        public string NormalButtonLabelColor { get; set; }
        public string BodyTextColor { get; set; }
        public string NumberColor { get; set; }
        public string BorderLineColor { get; set; }
        public string BrandName { get; set; }
        public string Balance { get; set; }
        public ICommand GoToScreen_emM_MENU_Command { get; }
        public ViewModel_emM_ZRES()
        {
            GoToScreen_emM_MENU_Command = new Command(GoToScreen_emM_MENU);
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
