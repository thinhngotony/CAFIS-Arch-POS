using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPSaturnEMoney.State;

namespace WPSaturnEMoney.ViewModels
{
    class ViewModel_emM_HRESJ : BaseViewModel
    {
        public string TitleColor { get; set; }
        public string BodyTextColor { get; set; }
        public string ButtonBorderColor { get; set; }
        public string NormalButtonColor { get; set; }
        public string NormalButtonLabelColor { get; set; }
        public ICommand GoToScreen_emM_HJG_Command { get; }
        public ViewModel_emM_HRESJ()
        {
            GoToScreen_emM_HJG_Command = new Command(GoToScreen_emM_HJG);
        }

        public void GoToScreen_emM_HJG()
        {
            Common.Utilities.Log.Info("Press button [OK]");
            Session.ScreenState.NextState = StateMachine.State.emM_HJG;
            Session.TimerCount = 0;
        }

        public override void UpdateView()
        {
        }
    }
}
