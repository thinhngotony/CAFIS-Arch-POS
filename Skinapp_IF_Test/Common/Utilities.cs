using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Timers;

namespace Vjp.Skinapp_IF_Test.Common
{
    public class Utilities
    {
        public static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /*public enum State
        {
            InitApp,
            emI,
            emS,
            emP,
            emTM,
            emF,
            emMAorMI_EMSG,
            emCM,
            emMSorMT,
            emMSorMT_PW,
            emMT_MSG,
            emMSorMT_TA,
            emMS_MENU,
            emZ,
            emZF,
            emMT_MENU,
            emMT_RV,
            emMT_HV,
            emMT_TM,
            emMT_ZRESJ,
            emMT_HRESJ,
            emMT_EMSG,
            emMT_SMSG,
            emMT_ZJG,
            emMT_HJG,
            emMT_JGC1,
            emMT_JGC2,
            emMT_F,
            emM_TA,
            emM_SMSG,
            emM_EMSG,
            emM_CH,
            emM_MENU,
            emM_RV,
            emM_TM,
            emM_ZRES,
            emM_ZRESJ,
            emM_ZJG,
            emM_HRES,
            emM_HRESJ,
            emM_HJG,
            emM_JGC1,
            emM_JGC2,
            emM_TTLC,
            emM_F,
        }*/

        public enum State
        {
            Idle, // 待機中
            Transacting, // 取引中
            Maintaining, // メンテ呼出中
            Closed // アプリ終了
        }

        // IFConfig struct
        public class PosIFConfig
        {
            public string msg_name { get; set; }
            public string pos_process_name { get; set; }
            public string pos_window_name { get; set; }
            public int[] pos_btn_loc { get; set; }
            public int pos_timeout { get; set; }
            public string from_pos_dat { get; set; }
            public string to_pos_dat { get; set; }
        }

        public class FromPos
        {
            public string service { get; set; }
            public string sequence { get; set; }
            public string location { get; set; }
            public string amount { get; set; }
            public string regi_tran_date { get; set; }
            public string customer_timeout { get; set; }
            // public string payment_method { get; set; }
            // public string tran_type { get; set; }
            // public string receipt_num { get; set; }
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
            // public string payment_method { get; set; }
            // public string SlipNumber { get; set; }
            // public string TransactionNumber { get; set; }
            // public string TransactionType { get; set; }
        }

        public static bool CsFileRead<T>(string appPath, string filePath, ref T dataList)
        {
            try
            {
                if (!File.Exists(appPath + filePath)) return false;
                using (StreamReader reader = new StreamReader(appPath + filePath, Encoding.GetEncoding("Shift_JIS")))
                {
                    string msg = reader.ReadToEnd();
                    dataList = JsonSerializer.Deserialize<T>(msg);
                    //reader.Close();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool CsFileWrite<T>(string appPath, string filePath, T args, bool writeLog = true)
        {
            using (StreamWriter writer = new StreamWriter(appPath + filePath, false, Encoding.GetEncoding("Shift_JIS")))
            {
                try
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string jsonString = JsonSerializer.Serialize(args, options);
                    writer.Write(jsonString);
                    //writer.Close();
                    if (writeLog) Log.Info($"Write to {filePath}: {jsonString}");
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Return true if data object contains null
        /// </summary>
        public static bool ContainNull<T>(T data)
        {
            return !data.GetType().GetProperties().All(p => p.GetValue(data) != null);
        }

        public static bool IsNullorEmpty<T>(T data)
        {
            T config = data;
            var props = config.GetType().GetProperties();

            foreach (var p in props)
            {
                if (p.GetValue(config) == null)
                {
                    Log.Error(data.GetType().Name + " config properties missing.");
                    return true;
                }
                if (p.PropertyType != typeof(string)) { continue; }
                if (string.IsNullOrEmpty((string)p.GetValue(config)))
                {
                    Log.Error(data.GetType().Name + " config contain an empty string.");
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
                if (string.IsNullOrEmpty((string)p.GetValue(config)))
                {
                    if (p.Name == "service" || p.Name == "location" || p.Name == "customer_timeout" || (config.location == "1" && p.Name == "regi_tran_date"))
                    {
                        Log.Error(p.Name + " is missing or empty.");
                        return (9, p.Name + " is missing or empty.");
                    }
                    else if (p.Name == "sequence" || (config.location == "1" && p.Name == "amount"))
                    {
                        Log.Info(p.Name + " is missing or empty.");
                        return (3, p.Name + " is missing or empty.");
                    }
                }
            }
            return (1, "");
        }

        public static int GetLoc(int x_value, int y_value)
        {
            return x_value + 65536 * y_value;
        }
    }

}
