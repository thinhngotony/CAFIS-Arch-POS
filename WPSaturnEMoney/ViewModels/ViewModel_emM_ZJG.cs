using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.State;

namespace WPSaturnEMoney.ViewModels
{
    class ViewModel_emM_ZJG : BaseViewModel
    {
        public string TitleColor { get; set; }
        public string BodyTextColor { get; set; }
        public string AttentionTextColor { get; set; }
        public string ButtonBorderColor { get; set; }
        public string NormalButtonColor { get; set; }
        public string NormalButtonLabelColor { get; set; }
        public ICommand GoToScreen_emM_JGC1_Command { get; }
        public ICommand GoToScreen_emM_JGC2_Command { get; }
        public ViewModel_emM_ZJG()
        {
            GoToScreen_emM_JGC1_Command = new Command(GoToScreen_emM_JGC1);
            GoToScreen_emM_JGC2_Command = new Command(GoToScreen_emM_JGC2);
        }

        public void GoToScreen_emM_JGC1()
        {
            Utilities.Log.Info("Press button [決済完了]");
            Session.PreState_emM_JGCx = "emM_ZJG";
            Session.ScreenState.NextState = StateMachine.State.emM_JGC1;
            Session.TimerCount = 0;
        }
        public void GoToScreen_emM_JGC2()
        {
            Utilities.Log.Info("Press button [終了]");
            Session.PreState_emM_JGCx = "emM_ZJG";
            Session.ScreenState.NextState = StateMachine.State.emM_JGC2;
            Session.TimerCount = 0;
        }

        public override void UpdateView()
        {
        }
    }
}
