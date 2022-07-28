using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPSaturnEMoney.State;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;
using System.IO;

namespace WPSaturnEMoney.ViewModels
{
    class ViewModel_emES_MENU : BaseViewModel
    {
        public string TitleColor { get; set; }
        public string BodyTextColor { get; set; }
        public string ButtonBorderColor { get; set; }
        public string NormalButtonColor { get; set; }
        public string NormalButtonLabelColor { get; set; }
        public string BackButtonColor { get; set; }
        public string BackButtonLabelColor { get; set; }
        public string Message1 { get; set; }
        public string Message2 { get; set; }
        public string Message3 { get; set; }
        public string Message4 { get; set; }
        public ICommand GoToScreen_emP_Command { get; }
        public ICommand GoToScreen_emMT_MENU_Command { get; }
        public ICommand GoToState_emM_TA_Command { get; }

        public ViewModel_emES_MENU()
        {
            GoToScreen_emMT_MENU_Command = new Command(GoToScreen_emMT_MENU);
            GoToScreen_emP_Command = new Command(GoToScreen_emP);
            GoToState_emM_TA_Command = new Command(GoToState_emM_TA);
        }
        public void GoToScreen_emP()
        {
            Utilities.Log.Info("Press button [終了]");
            Session.ScreenState.NextState = StateMachine.State.emPayment;
            Session.TimerCount = 0;
        }
        public void GoToState_emM_TA()
        {
            Utilities.Log.Info("Press button [決済完了]");
            Session.TimerCount = 0;
            Session.ScreenState.NextState = StateMachine.State.emM_TA;
        }
        public void GoToScreen_emMT_MENU()
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

            GlobalData.ViewModelProperties.Message = "";
            GlobalData.CustomerViewModelProperties.Message = "";
            Session.ScreenState.NextState = StateMachine.State.emMaintenanceMenuFromError;
        }
        public override void UpdateView()
        {
        }
    }
}
