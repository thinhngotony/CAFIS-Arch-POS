using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPSaturnEMoney.State;

namespace WPSaturnEMoney.ViewModels
{
    class ViewModel_emMT_HRESJ : BaseViewModel
    {
        public string TitleColor { get; set; }
        public string BodyTextColor { get; set; }
        public string ButtonBorderColor { get; set; }
        public string NormalButtonColor { get; set; }
        public string NormalButtonLabelColor { get; set; }
        public string BtnOKVisibility { get; set; }
        public string BtnReturnVisibility { get; set; }
        public ICommand GoToScreen_emMT_HJG_Command { get; }
        public ICommand GoToScreen_emMT_MENU_Command { get; }
        public ViewModel_emMT_HRESJ()
        {
            GoToScreen_emMT_HJG_Command = new Command(GoToScreen_emMT_HJG);
            GoToScreen_emMT_MENU_Command = new Command(GoToScreen_emMT_MENU);
        }

        public void GoToScreen_emMT_HJG()
        {
            Session.ScreenState.NextState = StateMachine.State.emMT_HJG;
            Session.TimerCount = 0;
        }
        public void GoToScreen_emMT_MENU()
        {
            Session.ScreenState.NextState = StateMachine.State.emMaintenanceMenuFromError;
            Session.TimerCount = 0;
        }

        public override void UpdateView()
        {
        }
    }
}
