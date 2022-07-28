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
    class ViewModel_emMaintenanceMenuFromError : BaseViewModel
    {
        public string TitleColor { get; set; }
        public string BodyTextColor { get; set; }
        public string ButtonBorderColor { get; set; }
        public string NormalButtonColor { get; set; }
        public string NormalButtonLabelColor { get; set; }
        public string BackButtonColor { get; set; }
        public string BackButtonLabelColor { get; set; }
        public string ErrorMessage { get; set; }
        // public string SettlementSeqNoVisibility { get; set; }
        // public string SettlementSeqNo { get; set; }
        public ICommand GoToScreen_emMSorMT_TA_Command { get; }
        public ICommand GoToScreen_emMT_RV_Command { get; }
        public ICommand GoToScreen_emMT_HV_Command { get; }
        public ICommand GoToScreen_emES_MENU_Command { get; }

        public ViewModel_emMaintenanceMenuFromError()
        {
            GoToScreen_emES_MENU_Command = new Command(GoToScreen_emES_MENUAsync);
            GoToScreen_emMSorMT_TA_Command = new Command(GoToScreen_emMSorMT_TA);
            GoToScreen_emMT_RV_Command = new Command(GoToScreen_emMT_RV);
            GoToScreen_emMT_HV_Command = new Command(GoToScreen_emMT_HV);
        }

        public async void GoToScreen_emES_MENUAsync()
        {
            Utilities.Log.Info("Press button [終了]");

            GlobalData.Data_ToPosDat.service = "EMONEY";
            GlobalData.Data_ToPosDat.sequence = GlobalData.Data_FromPosDat.sequence;
            GlobalData.Data_ToPosDat.last_operator = GlobalData.LastOperator;
            GlobalData.Data_ToPosDat.result = "0"; // canceled
            GlobalData.Data_ToPosDat.SettledAmount = "0";
            GlobalData.Data_ToPosDat.CurrentService = "";
            GlobalData.Data_ToPosDat.statementID = "";
            
            await WinAPI.EndTransaction();
            Session.TimerCount = 0;

            // Session.ScreenState.NextState = StateMachine.State.emES_MENU;
        }
        public void GoToScreen_emMSorMT_TA()
        {
            if (Session.ClickedButton_emMT_MENU == "check_connection")
            {
                Utilities.Log.Info("Press button [疎通確認]");
            }
            else if (Session.ClickedButton_emMT_MENU == "reprint_receipt")
            {
                Utilities.Log.Info("Press button [レシート再印刷]");
            }

            Session.ScreenState.NextState = StateMachine.State.emMSorMT_TA;
            Session.TimerCount = 0;
            Utilities.Log.Info("Load screen emMT_TA");
        }
        public void GoToScreen_emMT_RV()
        {
            Utilities.Log.Info("Press button [残高照会]");

            FileStruct.CheckBtnOrderConfig();
            if (FileStruct.IsBtnOrderConfigError("ReadValue").Item1)
            {
                GlobalData.ScreenSubErrorMsg = FileStruct.IsBtnOrderConfigError("ReadValue").Item2;
                Session.ScreenState.NextState = StateMachine.State.emMT_EMSG;
                Session.PreFunc_emMT_EMSG = "app_error";
                GlobalData.BtnOrderConfigErrors.Clear();
                GlobalData.ApplicationErrorMsg = "";
                GlobalData.ApplicationError = "";
            }
            else
            {
                Session.ScreenState.NextState = StateMachine.State.emMT_RV;
            }
            Session.TimerCount = 0;
        }
        public void GoToScreen_emMT_HV()
        {
            Utilities.Log.Info("Press button [カード履歴照会]");

            FileStruct.CheckBtnOrderConfig();
            if (FileStruct.IsBtnOrderConfigError("CardHistory").Item1)
            {
                GlobalData.ScreenSubErrorMsg = FileStruct.IsBtnOrderConfigError("CardHistory").Item2;
                Session.ScreenState.NextState = StateMachine.State.emMT_EMSG;
                Session.PreFunc_emMT_EMSG = "app_error";
                GlobalData.BtnOrderConfigErrors.Clear();
                GlobalData.ApplicationErrorMsg = "";
                GlobalData.ApplicationError = "";
            }
            else
            {
                Session.ScreenState.NextState = StateMachine.State.emMT_HV;
            }
            Session.TimerCount = 0;
        }

        public override void UpdateView()
        {
        }
    }
}
