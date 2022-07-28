using Microsoft.PointOfService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPSaturnEMoney.Common;

namespace WPSaturnEMoney.Models
{
    internal class GlobalData
    {
        public static string AppName = "WEBPOS_Saturn_EMoney"; // Main window name too
        public static string CustomerWindowName = "WEBPOS_Saturn_EMoney_Customer";
        public static MainWindow mainWindow;
        public static CustomerWindow customerWindow;
        public static bool IsWindowHidden = false;
        public static bool IsPopupNotificationOpen = false;

        public static int CustomMessageID = 0;
        public static string ServiceName; // brand code
        public static string SettlementSeqNo;
        public static string AppPath;
        public static string ScreenErrorMsg;
       	public static string ScreenSubErrorMsg;
        public static string ScreenMErrorMsg;
        public static string ScreenProgressMsg;
        public static string ScreenResultMsg;
        public static Utilities.ErrorType TransactionErrorType;

        // OutputCompleteEvent data
        public static decimal TotalPayment; // 円
        public static decimal SettledAmount;
        public static decimal Balance;
        public static string CurrentService;
        public static string statementID;
        public static string LastOperator; // "0" clerk operation, "1" customer operation
        public static FileStruct.SaturnAPIResponse SaturnAPIResponse;
        public static string CurrentServiceID;
        public static string CurrentStatusNotificationCode = "";

        // Previous SubtractValue ErrorEvent
        public static string PreCurrentServiceSV;
        public static decimal PreSettledAmountSV;
        public static int PreSequenceNumberSV;
        public static string PreStatementIDSV;

        // Response SequenceNumber of PrintRetry API
        public static int SequenceNumberPR;

        // true: cannot find the key when convert receipt
        public static bool IsConvertReceiptError = false;
        public static bool IsPrintError = false;

        public static FileStruct.FromPos Data_FromPosDat = new FileStruct.FromPos();
        public static FileStruct.ToPos Data_ToPosDat = new FileStruct.ToPos();

        public static FileStruct.Config BasicConfig = new FileStruct.Config
        {
            color = new FileStruct.ColorCategory
            {
                common = new FileStruct.CommonColor(),
                customer = new FileStruct.CustomerColor(),
                maintenance = new FileStruct.MaintenanceColor()
            }
        };
        public static List<FileStruct.Msg_Brand> MsgBrandConfig = new List<FileStruct.Msg_Brand>();
        public static List<FileStruct.MsgDetail> MsgCommonConfig = new List<FileStruct.MsgDetail>();
        public static List<FileStruct.MsgDetail> MsgConditionConfig = new List<FileStruct.MsgDetail>();
        public static List<FileStruct.MsgDetail> MsgHealthCheck1Config = new List<FileStruct.MsgDetail>();
        public static List<FileStruct.MsgDetail> MsgHealthCheck2Config = new List<FileStruct.MsgDetail>();
        public static List<FileStruct.MsgDetail> MsgHealthCheck3Config = new List<FileStruct.MsgDetail>();
        public static List<FileStruct.MsgMode> MsgModeConfig = new List<FileStruct.MsgMode>();
        public static FileStruct.PosIFConfig PosIFConfig = new FileStruct.PosIFConfig
        {
            msg_name = "WPSaturnEMoney",
            pos_process_name = "POS_INTERFACE_TEST",
            pos_window_name = "Mainview",
            pos_btn_loc = new int[] { 150, 50 },
            pos_timeout = 5000
        };
        public static List<FileStruct.ServiceId> ServiceIdConfig = new List<FileStruct.ServiceId>();

        public static List<FileStruct.RowData> SalesReceiptData = new List<FileStruct.RowData>();
        public static List<FileStruct.RowData> JournalData = new List<FileStruct.RowData>();
        public static FileStruct.DailyTotal DailyTotalData = new FileStruct.DailyTotal();

        public static FileStruct.PosPrinter PrinterData = new FileStruct.PosPrinter();
        public static FileStruct.ConvReceipt ConvReceiptConfig = new FileStruct.ConvReceipt();

        // Old config files 
        public static FileStruct.Msg_Common_Old MsgCommonOldConfig = new FileStruct.Msg_Common_Old();
        public static FileStruct.Msg_Suica MsgSuicaConfig = new FileStruct.Msg_Suica();
        public static FileStruct.Msg_iD MsgiDConfig = new FileStruct.Msg_iD();
        public static FileStruct.Msg_WAON MsgWAONConfig = new FileStruct.Msg_WAON();

        public static string[] ServiceNameCorrect = new string[] { "Credit", "UnionPay", "Debit", "Suica", "iD", "Edy", "WAON", "QUICPay", "nanaco", "PiTaPa", "NFCPayment" };
        public static FileStruct.Total TotalInfo = new FileStruct.Total
        {
            service = new Dictionary<string, FileStruct.TotalDetail> {
                { ServiceNameCorrect[0], new FileStruct.TotalDetail()},
                { ServiceNameCorrect[1], new FileStruct.TotalDetail()},
                { ServiceNameCorrect[2], new FileStruct.TotalDetail()},
                { ServiceNameCorrect[3], new FileStruct.TotalDetail()},
                { ServiceNameCorrect[4], new FileStruct.TotalDetail()},
                { ServiceNameCorrect[5], new FileStruct.TotalDetail()},
            },
            done_num = ""
        };

        public static string[] LoginData;

        public static FileStruct.ViewModelProperties ViewModelProperties = new FileStruct.ViewModelProperties();
        public static FileStruct.ViewModelProperties CustomerViewModelProperties = new FileStruct.ViewModelProperties();
        public static string MsgCode = "";

        public static string PosPath = @"\Pos";
        public static string LogPath = @"\Log";
        public static string ConfigFolderPath = @"\Config";
        public static string TransactionDataPath = @"\transaction_data";
        public static string ImagePath = @"\Img";
        public static string SoundPath = @"\Snd";

        public static string SettlementSeqNoPath = @"\SettlementSeqNo.dat";
        public static string LastSettlementSeqNoPath = @"\LastSettlementSeqNo.dat";
        public static string LastSettlementBrandCodePath = @"\LastSettlementBrandCode.dat";
        public static string LastTransactionPath = @"\LastTransactionInquiry.dat";
        public static string ConvertedReceiptPath = @"\ConvertedReceipt.dat";

        public static string FromPosPath = PosPath + @"\FromPos.dat";
        public static string ToPosPath = PosPath + @"\ToPos.dat";
        public static string LoginPath = PosPath + @"\login.json";
        public static string PosPrinterPath = PosPath + @"\PosPrinter.json";

        public static string DailyTotalPath = TransactionDataPath + @"\DailyTotal.dat";
        public static string JournalPath = TransactionDataPath + @"\Journal.dat";

        public static string ConfigPath = ConfigFolderPath + @"\Config.json";
        public static string MsgBrandPath = ConfigFolderPath + @"\Msg_Brand.json";
        public static string MsgCommonPath = ConfigFolderPath + @"\Msg_Common.json";
        public static string MsgConditionPath = ConfigFolderPath + @"\Msg_Condition.json";
        public static string MsgHealthCheck1Path = ConfigFolderPath + @"\Msg_HealthCheck1.json";
        public static string MsgHealthCheck2Path = ConfigFolderPath + @"\Msg_HealthCheck2.json";
        public static string MsgHealthCheck3Path = ConfigFolderPath + @"\Msg_HealthCheck3.json";
        public static string MsgModePath = ConfigFolderPath + @"\Msg_Mode.json";
        public static string PosIFConfigPath = ConfigFolderPath + @"\PosIFConfig.json";
        public static string ServiceIdPath = ConfigFolderPath + @"\ServiceId.json";

        public static string[] ListConfigFiles = new string[] { "Config.json", "Msg_Brand.json", "Msg_Common.json", "Msg_Condition.json", "Msg_HealthCheck1.json",
            "Msg_HealthCheck2.json", "Msg_HealthCheck3.json", "Msg_Mode.json", "PosIFConfig.json", "ServiceId.json" };

        public static string SaturnDeviceImagePath = ImagePath + @"\B01.png";
        public static string SaturnDeviceMaintenanceImagePath = ImagePath + @"\B02.png";

        // Old config files path
        public static string ConvReceiptPath = ConfigFolderPath + @"\ConvReceipt.json";
        public static string Msg_Common_OldPath = ConfigFolderPath + @"\Msg_Common_Old.json";
        public static string Msg_iDPath = ConfigFolderPath + @"\Msg_iD.json";
        public static string Msg_SuicaPath = ConfigFolderPath + @"\Msg_Suica.json";
        public static string Msg_WAONPath = ConfigFolderPath + @"\Msg_WAON.json";

        public static string ApplicationError = "";
        public static string ApplicationErrorMsg;
        public static List<string> BtnOrderConfigErrors = new List<string>();

        // Print error maintenance variable
        public static Window PrintErrorWindow = null;
        public static bool IsInPrinterMaintenance = false;
        public static bool MPPressReprint;
        public static Utilities.MPStep MPCurrentStep;
        public static Utilities.MPStep MPNextStep;
        public static ErrorCode MPErrorCode;
        public static int MPErrorCodeExtended;
    }
}
