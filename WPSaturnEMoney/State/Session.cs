using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPSaturnEMoney.ViewModels;
using WPSaturnEMoney.Common;

namespace WPSaturnEMoney.State
{
    internal class Session
    {
        public static StateMachine ScreenState;
        public static MainWindowViewModel MainViewModel;
        public static int TimerCount;
        public static int TimerCountRemaining;
        public static bool IsStatusOK = false;
        public static bool IsTransactionStart = false;
        public static bool IsIDCorrect = false;
        public static string MaintenanceMode; // "MS" (connection), "MT" (payment) or "M" (standby)
        public static string PreState_emTM; // "emP" or "emZ"
        public static string PreState_emCM; // "emS", "emP" or "emTM"
        public static string PreState_emMT_TM; // "emMT_RV" or "emMT_HV"
        public static string PreState_emM_TM; // "emM_RV" or "emM_CH"
        public static string PreState_emM_JGCx; // "emM_ZJG" or "emM_HJG"
        public static string PreState_emMT_JGCx; // "emMT_ZJG" or "emMT_HJG"
        public static bool IsPrintRetryPreState_emMT_F = false;
        public static bool IsProcNotCmpltAtStandby = false; // Standby maintenance PrintRetry got Processing Not Completed
        public static string ClickedButton_emMT_MENU; // "check_connection" or "reprint_receipt"
        public static string ClickedButton_emM_MENU; // "check_connection", "reprint_receipt" or "daily_total"

        // For deciding print receipt
        public static string PreFunc_emMT_SMSG; // "check_connection", "reprint_receipt" or "reprint_conv_receipt"
        public static string PreFunc_emMT_EMSG; // "check_connection", "reprint_receipt", "read_value", "card_history" or "app_error"
        public static string PreFunc_emMS_MENU;
        public static string PreFunc_emES_MENU;// "login" or "check_connection"
        public static string PreFunc_emM_SMSG; // "check_connection", "reprint_receipt", "reprint_conv_receipt" or "daily_total"
        public static string PreFunc_emM_EMSG; // "check_connection", "reprint_receipt", "read_value", "card_history", "daily_total" or "app_error"

        // For controlling sound
        public static WMPControl WMP = new WMPControl();
    }
}
