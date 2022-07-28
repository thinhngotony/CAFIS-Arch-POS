using System.Linq;
using System.Windows.Input;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;
using WPSaturnEMoney.State;

namespace WPSaturnEMoney.ViewModels
{
    class ViewModel_emMaintenanceMenu : BaseViewModel
    {
        public string TitleColor { get; set; }
        public string ButtonBorderColor { get; set; }
        public string NormalButtonColor { get; set; }
        public string NormalButtonLabelColor { get; set; }
        public string BackButtonColor { get; set; }
        public string BackButtonLabelColor { get; set; }
        public string BtnLastMinTransInquiryVisibility { get; set; } = "Visible";
        public string BtnBalanceInquiryVisibility { get; set; } = "Visible";
        public string BtnLastMinTransSuccessFailedVisibility { get; set; } = "Visible";
        public ICommand GoToScreen_emM_TA_Command { get; }
        public ICommand GoToScreen_emMT_HJG_Command { get; }
        public ICommand GoToScreen_emM_RV_Command { get; }
        public ICommand GoToScreen_emM_CH_Command { get; }
        public ICommand GoToScreen_emS_Command { get; }

        public ViewModel_emMaintenanceMenu()
        {
            GoToScreen_emS_Command = new Command(GoToScreen_emS);
            GoToScreen_emM_TA_Command = new Command(GoToScreen_emM_TA);
            GoToScreen_emMT_HJG_Command = new Command(GoToScreen_emMT_HJG);
        }

        public void GoToScreen_emS()
        {
            Utilities.Log.Info("Press button [終了]");
            Session.ScreenState.NextState = StateMachine.State.emStandBy;
            Session.TimerCount = 0;
        }
        public void GoToScreen_emM_TA()
        {
            if (Session.ClickedButton_emM_MENU == "check_connection")
            {
                Utilities.Log.Info("Press button [疎通確認]");
                Session.ScreenState.NextState = StateMachine.State.emM_TA;
            }
            else if (Session.ClickedButton_emM_MENU == "balance_inquiry")
            {
                Utilities.Log.Info("Press button [残高照会]");
                Session.ScreenState.NextState = StateMachine.State.emBalanceInquiry;
            }
            else if (Session.ClickedButton_emM_MENU == "reprint_receipt")
            {
                Utilities.Log.Info("Press button [レシート再印刷]");
                Session.ScreenState.NextState = StateMachine.State.emM_TA;
            }
            else if (Session.ClickedButton_emM_MENU == "transaction_inquiry")
            {
                Utilities.Log.Info("Press button [直前取引照会]");
                Session.ScreenState.NextState = StateMachine.State.emM_TA;
            }
            Session.TimerCount = 0;
        }
        public void GoToScreen_emMT_HJG()
        {
            Utilities.Log.Info("Press button [日計]");
            Session.ScreenState.NextState = StateMachine.State.emMT_HJG;
            Session.TimerCount = 0;
        }

        public override void UpdateView()
        {
        }

        //public void GoToScreen_emM_RV()
        //{
        //    Utilities.Log.Info("Press button [残高照会]");

        //    FileStruct.CheckBtnOrderConfig();
        //    if (FileStruct.IsBtnOrderConfigError("ReadValue").Item1)
        //    {
        //        GlobalData.ScreenMErrorMsg = FileStruct.IsBtnOrderConfigError("ReadValue").Item2;
        //        Session.ScreenState.NextState = StateMachine.State.emM_EMSG;
        //        Session.PreFunc_emM_EMSG = "app_error";
        //        GlobalData.BtnOrderConfigErrors.Clear();
        //        GlobalData.ApplicationErrorMsg = "";
        //        GlobalData.ApplicationError = "";
        //    }
        //    else
        //    {
        //        Session.ScreenState.NextState = StateMachine.State.emM_RV;
        //    }
        //    Session.TimerCount = 0;
        //}
        //public void GoToScreen_emM_CH()
        //{
        //    Utilities.Log.Info("Press button [カード履歴照会]");

        //    FileStruct.CheckBtnOrderConfig();
        //    if (FileStruct.IsBtnOrderConfigError("CardHistory").Item1)
        //    {
        //        GlobalData.ScreenMErrorMsg = FileStruct.IsBtnOrderConfigError("CardHistory").Item2;
        //        Session.ScreenState.NextState = StateMachine.State.emM_EMSG;
        //        Session.PreFunc_emM_EMSG = "app_error";
        //        GlobalData.BtnOrderConfigErrors.Clear();
        //        GlobalData.ApplicationErrorMsg = "";
        //        GlobalData.ApplicationError = "";
        //    }
        //    else
        //    {
        //        Session.ScreenState.NextState = StateMachine.State.emM_CH;
        //    }
        //    Session.TimerCount = 0;
        //}
    }
}
