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
    class ViewModel_emMT_JGC2 : BaseViewModel
    {
        public string AttentionTitle { get; set; }
        public string AttentionTitleColor { get; set; }
        public string BodyText { get; set; }
        public string BodyTextColor { get; set; }
        public string ButtonBorderColor { get; set; }
        public string NormalButtonColor { get; set; }
        public string NormalButtonLabelColor { get; set; }
        public ICommand GoToScreen_emI_Command { get; }
        public ICommand GoToPreviousScreen_Command { get; }
        public ViewModel_emMT_JGC2()
        {
            GoToScreen_emI_Command = new Command(GoToScreen_emI);
            GoToPreviousScreen_Command = new Command(GoToPreviousScreen);
        }

        public async void GoToScreen_emI()
        {
            Utilities.Log.Info("Press button [はい]");

            GlobalData.Data_ToPosDat.service = "EMONEY";
            GlobalData.Data_ToPosDat.sequence = GlobalData.Data_FromPosDat.sequence;
            GlobalData.Data_ToPosDat.last_operator = GlobalData.LastOperator;
            GlobalData.Data_ToPosDat.result = "0"; // canceled
            GlobalData.Data_ToPosDat.SettledAmount = "0";
            GlobalData.Data_ToPosDat.CurrentService = "";
            GlobalData.Data_ToPosDat.statementID = "";
            Session.TimerCount = 0;

            await WinAPI.EndTransaction();
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
