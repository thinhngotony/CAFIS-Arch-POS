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
    class ViewModel_emM_JGC1 : BaseViewModel
    {
        public string AttentionTitle { get; set; }
        public string AttentionTitleColor { get; set; }
        public string BodyText { get; set; }
        public string BodyTextColor { get; set; }
        public string ButtonBorderColor { get; set; }
        public string NormalButtonColor { get; set; }
        public string NormalButtonLabelColor { get; set; }
        public ICommand GoToScreen_emM_F_Command { get; }
        public ICommand GoToPreviousScreen_Command { get; }
        public ViewModel_emM_JGC1()
        {
            GoToScreen_emM_F_Command = new Command(GoToScreen_emM_F);
            GoToPreviousScreen_Command = new Command(GoToPreviousScreen);
        }

        public void GoToScreen_emM_F()
        {
            Utilities.Log.Info("Press button [はい]");
            Session.ScreenState.NextState = StateMachine.State.emM_F;
            Session.TimerCount = 0;
        }
        public void GoToPreviousScreen()
        {
            Utilities.Log.Info("Press button [いいえ]");

            if (Session.PreState_emM_JGCx == "emM_ZJG")
                Session.ScreenState.NextState = StateMachine.State.emM_ZJG;
            else if (Session.PreState_emM_JGCx == "emM_HJG")
                Session.ScreenState.NextState = StateMachine.State.emM_HJG;

            Session.PreState_emM_JGCx = "";
            Session.TimerCount = 0;
        }

        public override void UpdateView()
        {
        }
    }
}
