using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using WPSaturnEMoney.Common;

namespace WPSaturnEMoney.Models
{
    public partial class FileStruct
    {
        public static bool CsFileRead<T>(string appPath, string filePath, ref T dataList)
        {
            try
            {
                if (!File.Exists(appPath + filePath)) return false;
                using (StreamReader reader = new StreamReader(appPath + filePath, Encoding.GetEncoding("Shift_JIS")))
                {
                    string msg = reader.ReadToEnd();
                    dataList = JsonConvert.DeserializeObject<T>(msg, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    //reader.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Utilities.Log.Error(ex.ToString());
                return false;
            }
        }

        public static bool CsFileWrite<T>(string appPath, string filePath, T args, bool writeLog = true, bool isAppend = false)
        {
            using (StreamWriter writer = new StreamWriter(appPath + filePath, isAppend, Encoding.GetEncoding("Shift_JIS")))
            {
                try
                {
                    string jsonString = JsonConvert.SerializeObject(args, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    writer.Write(jsonString);
                    //writer.Close();
                    if (writeLog) Utilities.Log.Info($"Write to {filePath}: {jsonString}");
                }
                catch (Exception ex)
                {
                    Utilities.Log.Error(ex.ToString());
                    return false;
                }
            }
            return true;
        }

        public static T Find<T>(List<T> list, Predicate<T> match, string code = "")
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (match(list[i]))
                {
                    return list[i];
                }
            }
            if (!string.IsNullOrEmpty(code)) Utilities.Log.Error(code + " code is not exist!");
            return (T)Activator.CreateInstance(typeof(T));
        }

        /*public static void SettingReadAndCheck()
        {
            // Check file read
            bool readConfigOK = ReadConfig();
            // Get Printer setting
            bool readPrinterOK = CsFileRead(GlobalData.AppPath, GlobalData.PosPrinterPath, ref GlobalData.PrinterData)
                                 && !IsNullorEmpty(GlobalData.PrinterData);
            bool readLoginOK = CsFileRead(GlobalData.AppPath, GlobalData.LoginPath, ref GlobalData.LoginData)
                                 && GlobalData.LoginData.Length > 0;
            if (!readConfigOK || !readPrinterOK || !readLoginOK || GlobalData.ApplicationError.Length > 0)
            {
                if (GlobalData.ApplicationError.Length > 0)
                {
                    GlobalData.ApplicationErrorMsg = Utilities.GetErrorTypeAndMsg("application", GlobalData.ApplicationError).Item2;
                }
                else
                {
                    GlobalData.ApplicationError = "AE1";
                    GlobalData.ApplicationErrorMsg = Utilities.GetErrorTypeAndMsg("application", GlobalData.ApplicationError).Item2;
                }
            }
        }*/

        /*public static bool ReadConfig()
        {
            if (CsFileRead(GlobalData.AppPath, GlobalData.ConfigPath, ref GlobalData.BasicConfig))
            {
                Config config = GlobalData.BasicConfig;
                if (IsNullorEmpty(config))
                {
                    GlobalData.ApplicationError = "AE8";
                    return false;
                }

                // Check all color config
                if (IsNullorEmpty(config.color.common))
                {
                    GlobalData.ApplicationError = "AE8";
                    Utilities.Log.Error("▲ Config.json >> color >> common: setting is not correct.");
                    return false;
                }
                else if (IsNullorEmpty(config.color.customer))
                {
                    GlobalData.ApplicationError = "AE8";
                    Utilities.Log.Error("▲ Config.json >> color >> customer: setting is not correct.");
                    return false;
                }
                else if (IsNullorEmpty(config.color.maintenance))
                {
                    GlobalData.ApplicationError = "AE8";
                    Utilities.Log.Error("▲ Config.json >> color >> maintenance: setting is not correct.");
                    return false;
                }

                // Check "brand_name"
                if (config.brand_name.Keys.Count != GlobalData.ServiceNameCorrect.Length)
                {
                    Utilities.Log.Error("▲ Config.json >> brand_name: number of services is not correct.");
                    GlobalData.ApplicationError = "AE1";
                    return false;
                }
                foreach (var key in config.brand_name.Keys)
                {
                    if (!GlobalData.ServiceNameCorrect.Contains(key))
                    {
                        Utilities.Log.Error("▲ Config.json >> brand_name: " + key + " is not correct.");
                        GlobalData.ApplicationError = "AE1";
                        return false;
                    }
                    if (string.IsNullOrEmpty(config.brand_name[key]))
                    {
                        Utilities.Log.Error("▲ Config.json >> brand_name: value of " + key + " is not correct.");
                        GlobalData.ApplicationError = "AE1";
                        return false;
                    }
                }

                // Check all "btn_order" items value
                return CheckBtnOrderConfig();
            }
            GlobalData.ApplicationError = "AE8";
            Utilities.Log.Error("▲ Config.json properties contain null or wrong format.");
            return false;
        }*/

        public static bool CheckBtnOrderConfig()
        {
            Config config = GlobalData.BasicConfig;
            string[] emptyArray = new string[] { "", "", "", "", "", "" };
            bool conditionSubtractValue = config.btn_order_SubtractValue.Length != 6 || Enumerable.SequenceEqual(config.btn_order_SubtractValue, emptyArray);
            bool conditionCardHistory = config.btn_order_CardHistory.Length != 6 || Enumerable.SequenceEqual(config.btn_order_CardHistory, emptyArray);
            bool conditionReadValue = config.btn_order_ReadValue.Length != 6 || Enumerable.SequenceEqual(config.btn_order_ReadValue, emptyArray);

            GlobalData.BtnOrderConfigErrors.Clear();
            if (conditionSubtractValue || conditionCardHistory || conditionReadValue)
            {
                // Note: in this case, use IsBtnOrderConfigError() to check and get error message,
                // don't use ApplicationError and ApplicationErrorMsg because it's only the last error in BtnOrderConfigErrors list.
                if (conditionSubtractValue)
                {
                    GlobalData.ApplicationError = "AE2";
                    GlobalData.BtnOrderConfigErrors.Add("AE2");
                    Utilities.Log.Error("▲ Config.json >> btn_order_SubtractValue: array is not correct.");
                }
                if (conditionReadValue)
                {
                    GlobalData.ApplicationError = "AE3";
                    GlobalData.BtnOrderConfigErrors.Add("AE3");
                    Utilities.Log.Error("▲ Config.json >> btn_order_ReadValue: array is not correct.");
                }
                if (conditionCardHistory)
                {
                    GlobalData.ApplicationError = "AE4";
                    GlobalData.BtnOrderConfigErrors.Add("AE4");
                    Utilities.Log.Error("▲ Config.json >> btn_order_CardHistory: array is not correct.");
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check button config: btn_order_SubtractValue, btn_order_ReadValue, btn_order_CardHistory
        /// </summary>
        /// <param name="configType">"SubtractValue", "ReadValue" or "CardHistory"</param>
        /// <returns>bool: is btn_order config error, string: error message</returns>
        public static (bool, string) IsBtnOrderConfigError(string configType)
        {
            if (configType == "SubtractValue")
            {
                return GlobalData.BtnOrderConfigErrors.Contains("AE2")
                       ? (true, Utilities.GetErrorTypeAndMsg("application", "AE2").Item2)
                       : (false, "");
            }
            else if (configType == "ReadValue")
            {
                return GlobalData.BtnOrderConfigErrors.Contains("AE3")
                       ? (true, Utilities.GetErrorTypeAndMsg("application", "AE3").Item2)
                       : (false, "");
            }
            else if (configType == "CardHistory")
            {
                return GlobalData.BtnOrderConfigErrors.Contains("AE4")
                       ? (true, Utilities.GetErrorTypeAndMsg("application", "AE4").Item2)
                       : (false, "");
            }
            else
            {
                return (false, "");
            }
        }

        /// <summary>
        /// Return true if data object contains null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool ContainNull<T>(T data)
        {
            return !data.GetType().GetProperties().All(p => p.GetValue(data) != null);
        }

        public static bool IsNullorEmpty<T>(T data, List<string> excludeProperties = default)
        {
            if (excludeProperties == default) excludeProperties = new List<string>();
            T config = data;
            var props = config.GetType().GetProperties();

            foreach (var p in props)
            {
                if (excludeProperties.Contains(p.Name)) { continue; }
                if (p.GetValue(config) == null)
                {
                    Utilities.Log.Error("▲ " + p.Name + " properties missing.");
                    return true;
                }
                if (p.PropertyType != typeof(string)) { continue; }
                if (string.IsNullOrEmpty((string)p.GetValue(config)))
                {
                    Utilities.Log.Error("▲ " + p.Name + " contain an empty string.");
                    return true;
                }
            }
            return false;
        }

        public static (int, string) ValidateFromPos(FromPos data)
        {
            FromPos config = data;
            var props = config.GetType().GetProperties();

            foreach (var p in props)
            {
                if (string.IsNullOrEmpty(p.GetValue(config).ToString()))
                {
                    if (p.Name == "service" || p.Name == "location" || p.Name == "customer_timeout" || (config.location == "1" && p.Name == "regi_tran_date"))
                    {
                        Utilities.Log.Error(p.Name + " is missing or empty.");
                        return (9, p.Name + " is missing or empty.");
                    }
                    else if (p.Name == "sequence" || (config.location == "1" && p.Name == "amount"))
                    {
                        Utilities.Log.Info(p.Name + " is missing or empty.");
                        return (3, p.Name + " is missing or empty.");
                    }
                }
            }
            return (1, "");
        }

        // IFConfig struct
        public class PosIFConfig
        {
            public string msg_name { get; set; }
            public string pos_process_name { get; set; }
            public string pos_window_name { get; set; }
            public int[] pos_btn_loc { get; set; }
            public int? pos_timeout { get; set; }
        }

        // Struct of file in Pos folder
        public class FromPos
        {
            public string service { get; set; }
            public string sequence { get; set; }
            public string location { get; set; }
            public string amount { get; set; }
            public string regi_tran_date { get; set; }
            public string customer_timeout { get; set; }
        }
        public class ToPos
        {
            public string service { get; set; }
            public string sequence { get; set; }
            public string last_operator { get; set; }
            public string result { get; set; }
            public string SettledAmount { get; set; }
            public string CurrentService { get; set; }
            public string statementID { get; set; }
        }
        public class ToPos_ClerkCallNotify : ToPos
        {
            public string error_code { get; set; }
        }
        public class Total
        {
            public Dictionary<string, TotalDetail> service { get; set; }
            public string done_num { get; set; }
        }
        public class TotalDetail
        {
            public int SS_CNT { get; set; }
            public int SS_AMT { get; set; }
            public int NA_CNT { get; set; }
            public int NA_AMT { get; set; }
            public int NASS_CNT { get; set; }
            public int NASS_AMT { get; set; }
        }

        // Struct of file in Config folder
        public class Config
        {
            public string[] brand_name { get; set; }
            public string[] btn_order_SubtractValue { get; set; }
            public string[] btn_order_ReadValue { get; set; }
            public string[] btn_order_CardHistory { get; set; }
            public int? log_keep_days { get; set; }
            public int? customer_error_wait { get; set; }
            public ColorCategory color { get; set; }
            public ScreenMsg screen_msg { get; set; }
            public int? sound_ready_time { get; set; }
            public int? sound_interval_time { get; set; }
            public int? sound_repeat_while_cat { get; set; }
            public string operation_mode { get; set; }
            public string maintenance_staff_check_flag { get; set; }
            public string customer_display_flag { get; set; } = "1"; // set this flag to "1" to show customer screen on init state when Config.json file is not ready yet
            public string receipt_print_setting { get; set; }
            public string receipt_print_customer_setting { get; set; }
            public string receipt_print_merchant_setting { get; set; }
            public string receipt_print_company_setting { get; set; }
            public string receipt_print_aggregate_setting { get; set; }
            public string receipt_print_aggregate_title { get; set; }
            public int? healthcheck_timeout { get; set; }
            public int? settlement_choice_timeout { get; set; }
            public int? settlement_request_timeout { get; set; }
            public int? settlement_cancel_timeout { get; set; }
            public string setting_pos_no { get; set; }
            public string port_name { get; set; }
            public int? bau_rate { get; set; }
            public string parity_check { get; set; }
            public int? daily_total_request_timeout { get; set; }
            public int? settlement_close_time { get; set; }
            public int? settlement_close_time_01 { get; set; }
            public int? settlement_close_time_02 { get; set; }
            public string receipt_print_customer_unfinished_setting { get; set; }
            public string receipt_print_merchant_unfinished_setting { get; set; }
            public string receipt_print_company_unfinished_setting { get; set; }
            public string receipt_print_aggregate_unfinished_setting { get; set; }
            public string selling_receipt_print_control_setting { get; set; }
            public int? journal_keep_days { get; set; }
            public int? daily_total_keep_days { get; set; }
        }
        public class ScreenMsg
        {
            [JsonProperty("m-ttlc_title")]
            public string m_ttlc_title { get; set; }

            [JsonProperty("m-ttlc_body")]
            public string m_ttlc_body { get; set; }

            [JsonProperty("m-jgc1_title")]
            public string m_jgc1_title { get; set; }

            [JsonProperty("m-jgc1_body")]
            public string m_jgc1_body { get; set; }

            [JsonProperty("m-jgc2_title")]
            public string m_jgc2_title { get; set; }

            [JsonProperty("m-jgc2_body")]
            public string m_jgc2_body { get; set; }

            [JsonProperty("mt-jgc1_title")]
            public string mt_jgc1_title { get; set; }

            [JsonProperty("mt-jgc1_body")]
            public string mt_jgc1_body { get; set; }

            [JsonProperty("mt-jgc2_title")]
            public string mt_jgc2_title { get; set; }

            [JsonProperty("mt-jgc2_body")]
            public string mt_jgc2_body { get; set; }
        }
        public class ColorCategory
        {
            public CommonColor common { get; set; }
            public CustomerColor customer { get; set; }
            public MaintenanceColor maintenance { get; set; }
        }
        public class CommonColor
        {
            [JsonProperty("base")]
            public string base_color { get; set; }
            public string title { get; set; }
            public string signal_void { get; set; }
            public string signal_normal { get; set; }
            public string signal_complete { get; set; }
            public string signal_caution { get; set; }
            public string signal_calling { get; set; }
            public string btn_touch { get; set; }
            public string btn_touch_label { get; set; }
        }
        public class CustomerColor
        {
            public string bg { get; set; }
            public string title { get; set; }
            public string txt { get; set; }
            public string amount { get; set; }
            public string borderline { get; set; }
            public string btn_line { get; set; }
            public string brand_btn { get; set; }
            public string brand_btn_label { get; set; }
            public string normal_btn { get; set; }
            public string normal_btn_label { get; set; }
            public string back_btn { get; set; }
            public string back_btn_label { get; set; }
        }
        public class MaintenanceColor
        {
            public string bg { get; set; }
            public string title { get; set; }
            public string txt { get; set; }
            public string amount { get; set; }
            public string borderline { get; set; }
            public string btn_line { get; set; }
            public string brand_btn { get; set; }
            public string brand_btn_label { get; set; }
            public string normal_btn { get; set; }
            public string normal_btn_label { get; set; }
            public string back_btn { get; set; }
            public string back_btn_label { get; set; }
            public string tenkey_btn { get; set; }
            public string tenkey_btn_label { get; set; }
            public string clear_btn { get; set; }
            public string clear_btn_label { get; set; }
            public string txt_attention { get; set; }
        }
        public class Msg_Brand
        {
            public string brand_code { get; set; }
            public List<MsgDetail> message { get; set; }
        }
        public class MsgMode
        {
            public string msg_code { get; set; }
            public string type { get; set; }
            public string header_title1 { get; set; }
            public string msg1 { get; set; }
            public string back_base1 { get; set; }
            public string header_title2 { get; set; }
            public string msg2 { get; set; }
            public string back_base2 { get; set; }
            public string header_title3 { get; set; }
            public string msg3 { get; set; }
            public string back_base3 { get; set; }
            public string header_title4 { get; set; }
            public string msg4 { get; set; }
            public string back_base4 { get; set; }
        }
        public class ServiceId
        {
            public string brand_code { get; set; }
            public string brand_name { get; set; }
            public string service_id { get; set; }
            public string reprint_service_id { get; set; }
            public string daily_total_service_id { get; set; }
        }
        public class MsgDetail
        {
            public string msg_code { get; set; }
            public string type { get; set; }
            public string msg { get; set; }
        }
        public class Msg_Common_Old
        {
            public Dictionary<string, MsgDetail> application { get; set; }
            public Dictionary<string, MsgDetail> result { get; set; }
            public Dictionary<string, MsgDetail> ResponseCode { get; set; }
            public Dictionary<string, MsgDetail> ResultCodeExtended { get; set; }
        }
        public class Msg_Suica
        {
            public Dictionary<string, MsgDetail> Errorcodedetail { get; set; }
        }
        public class Msg_WAON
        {
            public Dictionary<string, MsgDetail> Errorcodedetail { get; set; }
        }
        public class Msg_iD
        {
            public Dictionary<string, MsgDetail> Errorcodedetail { get; set; }
        }
        public class ConvReceipt
        {
            public convReceipt_detail[] Suica { get; set; }
            public convReceipt_detail[] nanaco { get; set; }
            public convReceipt_detail[] Edy { get; set; }
            public convReceipt_detail[] WAON { get; set; }
        }
        public class convReceipt_detail
        {
            public int type { get; set; }
            public RowData[] data { get; set; }
            public areaData area { get; set; }
        }
        public class RowData
        {
            [JsonProperty("rowid")]
            public int RowID { get; set; }
            [JsonProperty("label")]
            public string Label { get; set; }
            [JsonProperty("value")]
            public string Value { get; set; }
            [JsonProperty("attr")]
            public string Attr { get; set; }
        }
        public class areaData
        {
            public string st { get; set; }
            public string stkey { get; set; }
            public int stpos { get; set; }
            public string ed { get; set; }
            public string edkey { get; set; }
            public int edpos { get; set; }
        }
        public class PosPrinter
        {
            public string device_name { get; set; }
            public int width { get; set; }
            public string[][] header { get; set; }
            public string[][] footer { get; set; }
        }

        public class ViewModelProperties
        {
            // public string HeaderTitle { get; set; } = "";
            private List<string> listHeaderTitle = new List<string>();
            public List<string> ListHeaderTitle
            {
                get { return listHeaderTitle; }
                set {
                    if (value.Count < 2)
                    {
                        var temp = value.ToList();
                        temp.AddRange(Enumerable.Repeat("", 2 - value.Count));
                        value = temp;
                    }
                    listHeaderTitle = value;
                }
            }
            private List<string> listHeaderTitleMaintenance = new List<string>();
            public List<string> ListHeaderTitleMaintenance
            {
                get { return listHeaderTitleMaintenance; }
                set
                {
                    if (value.Count < 2)
                    {
                        var temp = value.ToList();
                        temp.AddRange(Enumerable.Repeat("", 2 - value.Count));
                        value = temp;
                    }
                    listHeaderTitleMaintenance = value;
                }
            }
            public string Message { get; set; } = "";
            public string MessageVisibility { get; set; } = "Collapsed";
            public string MessageMaintenance { get; set; } = "";
            public List<string> ListMessage { get; set; } = new List<string>();
            public string HeaderFooterColor { get; set; } = "#FFFFFF";
            public string HeaderTitleColor { get; set; } = "#828282";
            public string SignalBarColor { get; set; } = "#BDBDBD";
            public string SignalBarColorMaintenance { get; set; } = "#6079FF";
            public string BorderLineColor { get; set; } = "#CBCBCB";
            public string BackgroundColor { get; set; } = "#E1E1E1";
            public string TitleColor { get; set; } = "#254474";
            public string BodyTextColor { get; set; } = "#254474";
            public string NumberColor { get; set; } = "#000000";
            public string BtnVisibility { get; set; } = "Hidden";
            public string BtnContent { get; set; } = "";
            public System.Windows.Input.ICommand BtnCommand { get; set; } = new ViewModels.Command(WinAPI.HideWindow);
            public string ButtonBorderColor { get; set; } = "#254474";
            public string NormalButtonColor { get; set; } = "#EDEDED";
            public string NormalButtonLabelColor { get; set; } = "#254474";
            public string BrandButtonColor { get; set; } = "#FFFFFF";
            public string BrandButtonLabelColor { get; set; } = "#254474";
            public string BackButtonColor { get; set; } = "#A9A9A9";
            public string BackButtonLabelColor { get; set; } = "#FFFFFF";
            public string Btn2ndVisibility { get; set; } = "Collapsed";
            public string Btn2ndContent { get; set; } = "";
            public System.Windows.Input.ICommand Btn2ndCommand { get; set; } = new ViewModels.Command(WinAPI.HideWindow);
        }

        public class SaturnAPIRequest
        {
            public string mode { get; set; }
            public string service { get; set; }
            public string biz { get; set; }
            public string amount { get; set; }
            public string seqNo { get; set; }
            public string logOputputMode { get; set; }                                 // Required, 0: OFF, 1: ON; In case require to get log from Saturn1000L, set to 1 
            public string outputId { get; set; }                                     // Optional, transaction id between POS <-> Arch
            public string printMode { get; set; }                                      // Optional, 0: print from POS, 1: print from Arch  
            public string slivDiv { get; set; }                                        // 設定不要
            public string dailyTotalDiv { get; set; }                                  // Required, 0
        }

        public class SaturnAPIResponse
        {
            public BizInfo bizInfo { get; set; }
            public ControlInfo controlInfo { get; set; }

        }
        public class ControlInfo
        {
            public string mode { get; set; }
            public string service { get; set; }
            public string biz { get; set; }
            public string procResult { get; set; }
            public string outputId { get; set; }
            public string errorCode { get; set; }
            public string errorCodeDetail1 { get; set; }
            public string errorCodeDetail2 { get; set; }
            public string termIdentityNo { get; set; }
            public string dateTime { get; set; } // yyyy/MM/dd HH:mm:ss
        }

        public class BizInfo
        {
            public string posWaitingStatus { get; set; }
            public string kbn { get; set; }
            public List<dailyTransaction> transactionArray { get; set; }
            public BizInfo bizInfo { get; set; }
            public ControlInfo controlInfo { get; set; }
            public string resultCode { get; set; }
            public string resultCodeDetail { get; set; }
            public string centerResultCode { get; set; }
            public string merchInfo1 { get; set; }
            public string merchInfo2 { get; set; }
            public string rwId { get; set; }
            public string termIdentityNo { get; set; }
            public string useDateTime { get; set; }
            public string slipNo { get; set; }
            public string cardNumber { get; set; }
            public string tradeAmount { get; set; }
            public string cardBalance { get; set; }
            public string beforeCardBalance { get; set; }
            public string cashPayAmount { get; set; }
            public string info { get; set; }
            public string tradeStatus { get; set; } // 01: established, 02: unfinished
            public string cashConbiFlag { get; set; } // 00: Card payment, 01: Combined with cash
            public string seqNo { get; set; }
            public string customer { get; set; }
            public string merchant { get; set; }
            public string company { get; set; }
            public string aggregate { get; set; }
            public string unFinished1 { get; set; }
            public string unFinished2 { get; set; }
            public string unFinished3 { get; set; }
            public string unFinished4 { get; set; }
        }
        public class dailyTransaction
        {
            public int service { get; set; }
            public int dailyTotalResult { get; set; }
            public BizInfo bizInfo { get; set; }
        }

        public class DailyTotal
        {
            public string start_trade_datetime { get; set; } = "";
            public string last_trade_datetime { get; set; } = "";
            public List<BrandSumaryData> brand { get; set; } = new List<BrandSumaryData>();
        }
        public class BrandSumaryData
        {
            public string brand_code { get; set; } = "";
            public int payment_count { get; set; } = 0;
            public decimal payment_amount { get; set; } = 0;
            public int payment_count_unfinished { get; set; } = 0;
            public decimal payment_amount_unfinished { get; set; } = 0;
            public int payment_count_unfinished_confirm { get; set; } = 0;
            public decimal payment_amount_unfinished_confirm { get; set; } = 0;
            public int cancel_count { get; set; } = 0;
            public decimal cancel_amount { get; set; } = 0;
            public int cancel_count_unfinished { get; set; } = 0;
            public decimal cancel_amount_unfinished { get; set; } = 0;
            public int cancel_count_unfinished_confirm { get; set; } = 0;
            public decimal cancel_amount_unfinished_confirm { get; set; } = 0;
        }
    }
}
