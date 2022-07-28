using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using WPSaturnEMoney.State;
using WPSaturnEMoney.Models;

namespace WPSaturnEMoney.ViewModels
{
    class ViewModel_emMAorMI_EMSG : BaseViewModel
    {
        public string BodyTextColor { get; set; }
        public string ButtonBorderColor { get; set; }
        public string NormalButtonColor { get; set; }
        public string NormalButtonLabelColor { get; set; }
        public string ErrorMessage { get; set; }
        public ICommand Exit_Command { get; }
        public ICommand GoToScreen_emI_Command { get; }
        public string BtnOKVisibility { get; set; }
        public string BtnReturnVisibility { get; set; }
        public ViewModel_emMAorMI_EMSG()
        {
            Exit_Command = new Command(ExitApp);
            GoToScreen_emI_Command = new Command(GoToScreen_emI);
        }
        public void ExitApp()
        {
            Common.Utilities.Log.Info("アプリ終了");
            Application.Current.Shutdown();
        }
        public async void GoToScreen_emI()
        {
            GlobalData.Data_ToPosDat.service = "EMONEY";
            GlobalData.Data_ToPosDat.sequence = GlobalData.Data_FromPosDat.sequence;
            GlobalData.Data_ToPosDat.last_operator = GlobalData.LastOperator;
            GlobalData.Data_ToPosDat.result = "0"; // canceled
            GlobalData.Data_ToPosDat.SettledAmount = "0";
            GlobalData.Data_ToPosDat.CurrentService = "";
            GlobalData.Data_ToPosDat.statementID = "";
            Session.TimerCount = 0;

            await Common.WinAPI.EndTransaction();
        }

        public override void UpdateView()
        {
        }
    }
}
