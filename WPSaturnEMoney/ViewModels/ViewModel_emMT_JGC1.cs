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
    class ViewModel_emMT_JGC1 : BaseViewModel
    {
        public string AttentionTitle { get; set; }
        public string AttentionTitleColor { get; set; }
        public string BodyText { get; set; }
        public string BodyTextColor { get; set; }
        public string ButtonBorderColor { get; set; }
        public string NormalButtonColor { get; set; }
        public string NormalButtonLabelColor { get; set; }
        public ICommand GoToScreen_emMT_F_Command { get; }
        public ICommand GoToPreviousScreen_Command { get; }
        public ViewModel_emMT_JGC1()
        {
            GoToScreen_emMT_F_Command = new Command(GoToScreen_emMT_F);
            GoToPreviousScreen_Command = new Command(GoToPreviousScreen);
        }

        public void GoToScreen_emMT_F()
        {
            Utilities.Log.Info("Press button [はい]");
            Session.ScreenState.NextState = StateMachine.State.emMT_F;
            Session.TimerCount = 0;
        }
        public void GoToPreviousScreen()
        {
            Utilities.Log.Info("Press button [いいえ]");

            if (Session.PreState_emMT_JGCx == "emMT_ZJG")
                Session.ScreenState.NextState = StateMachine.State.emMT_ZJG;
            else if (Session.PreState_emMT_JGCx == "emMT_HJG")
                Session.ScreenState.NextState = StateMachine.State.emMT_HJG;

            Session.PreState_emMT_JGCx = "";
            Session.TimerCount = 0;
        }

        public override void UpdateView()
        {
        }
    }
}
