using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPSaturnEMoney.State;

namespace WPSaturnEMoney.ViewModels
{
    class ViewModel_emM_ZRESJ : BaseViewModel
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
        public ICommand GoToScreen_emM_ZJG_Command { get; }
        public ViewModel_emM_ZRESJ()
        {
            GoToScreen_emM_ZJG_Command = new Command(GoToScreen_emM_ZJG);
        }
        public void GoToScreen_emM_ZJG()
        {
            Common.Utilities.Log.Info("Press button [OK]");
            Session.ScreenState.NextState = StateMachine.State.emM_ZJG;
            Session.TimerCount = 0;
        }

        public override void UpdateView()
        {
        }
    }
}
