using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;
using WPSaturnEMoney.State;

namespace WPSaturnEMoney.ViewModels
{
    class ViewModel_emMT_ZJG : BaseViewModel
    {
        public string TitleColor { get; set; }
        public string BodyTextColor { get; set; }
        public string AttentionTextColor { get; set; }
        public string ButtonBorderColor { get; set; }
        public string NormalButtonColor { get; set; }
        public string NormalButtonLabelColor { get; set; }
        public ICommand GoToScreen_emMT_JGC1_Command { get; }
        public ICommand GoToScreen_emMT_JGC2_Command { get; }
        public ViewModel_emMT_ZJG()
        {
            GoToScreen_emMT_JGC1_Command = new Command(GoToScreen_emMT_JGC1);
            GoToScreen_emMT_JGC2_Command = new Command(GoToScreen_emMT_JGC2);
        }

        public void GoToScreen_emMT_JGC1()
        {
            Utilities.Log.Info("Press button [決済完了]");
            Session.PreState_emMT_JGCx = "emMT_ZJG";
            Session.ScreenState.NextState = StateMachine.State.emMT_JGC1;
            Session.TimerCount = 0;
        }
        public void GoToScreen_emMT_JGC2()
        {
            Utilities.Log.Info("Press button [終了]");
            Session.PreState_emMT_JGCx = "emMT_ZJG";
            Session.ScreenState.NextState = StateMachine.State.emMT_JGC2;
            Session.TimerCount = 0;
        }

        public override void UpdateView()
        {
        }
    }
}
