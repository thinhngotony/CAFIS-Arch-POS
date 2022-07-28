using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPSaturnEMoney.State;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;

namespace WPSaturnEMoney.ViewModels
{
    class ViewModel_emMS_MENU : BaseViewModel
    {
        public string TitleColor { get; set; }
        public string BodyTextColor { get; set; }
        public string ButtonBorderColor { get; set; }
        public string NormalButtonColor { get; set; }
        public string NormalButtonLabelColor { get; set; }
        public string BackButtonColor { get; set; }
        public string BackButtonLabelColor { get; set; }
        public string ErrorMessage { get; set; }
        public ICommand GoToScreen_emMSorMT_TA_Command { get; }
        public ICommand GoToScreen_emI_Command { get; }

        public ViewModel_emMS_MENU()
        {
            GoToScreen_emI_Command = new Command(GoToScreen_emI);
            GoToScreen_emMSorMT_TA_Command = new Command(GoToScreen_emMSorMT_TA);
        }
        public void GoToScreen_emMSorMT_TA()
        {
            Utilities.Log.Info("Press button [疎通確認]");
            Session.ScreenState.NextState = StateMachine.State.emMSorMT_TA;
            Session.TimerCount = 0;
            Utilities.Log.Info("Load screen emMS_TA");
        }
        public async void GoToScreen_emI()
        {
            Utilities.Log.Info("Press button [終了]");

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
        public override void UpdateView()
        {
        }
    }
}
