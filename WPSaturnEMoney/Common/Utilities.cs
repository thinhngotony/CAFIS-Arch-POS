using Microsoft.PointOfService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WPSaturnEMoney.Models;
using WPSaturnEMoney.State;

namespace WPSaturnEMoney.Common
{
    public static class Utilities
    {
        public enum MPStep
        {
            Init,
            emMT,
            emMT_PW,
            emMP_EMSG
        }
        public enum ErrorType
        {
            Success,
            AppError,
            POSTerminalCommunication,
            Failure,
            ProcessingNotCompleted,
            RecoveryMode,
            Timeout,
            PrintReceipt,
            Others,
        }
        public enum TotalRecordType
        {
            SS, // payment success
            NA, // unfinished processing
            NASS // unfinished processing -> defeat success
        }

        public static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Get ServiceName (also BrandCode) from BrandName.
        /// </summary>
        /// <param name="brandName">BrandName (use for button content).</param>
        /// <returns>ServiceName for Vesca API.</returns>
        public static string GetServiceName(string brandName)
        {
            string ret = "";
            var temp = FileStruct.Find(GlobalData.ServiceIdConfig, m => m.brand_name == brandName);
            if (temp != null) ret = temp.brand_code;
            else Log.Error("▲ ServiceId.json doesn't have brand_name " + brandName + ".");
            /*foreach (var item in GlobalData.BasicConfig.brand_name)
            {
                if (item.Value == brandName)
                {
                    ret = item.Key;
                    break;
                }
            }*/
            return ret;
        }

        /// <summary>
        /// Get BrandName from ServiceName.
        /// </summary>
        /// <param name="serviceName">ServiceName (also BrandCode) use for Vesca API.</param>
        /// <returns>BrandName.</returns>
        public static string GetBrandName(string serviceName)
        {
            string ret = "";
            var temp = FileStruct.Find(GlobalData.ServiceIdConfig, m => m.brand_code == serviceName);
            if (temp != null) ret = temp.brand_name;
            else Log.Error("▲ ServiceId.json doesn't have brand_code " + serviceName + ".");
            /*foreach (var item in GlobalData.BasicConfig.brand_name)
            {
                if (item.Key == serviceName)
                {
                    ret = item.Value;
                    break;
                }
            }*/
            return ret;
        }

        /// <summary>
        /// Get sequence number (4 digits) = [number read from file (4 digits zero padding)].
        /// </summary>
        /// <returns>SettlementSequenceNumber</returns>
        public static string GetSettlementSequenceNumber()
        {
            string numberFromFile = "";
            if (File.Exists(GlobalData.AppPath + GlobalData.SettlementSeqNoPath))
            {
                using (StreamReader reader = new StreamReader(GlobalData.AppPath + GlobalData.SettlementSeqNoPath))
                {
                    numberFromFile = reader.ReadToEnd();
                }
            }

            if (!int.TryParse(numberFromFile, out int numberValue))
            {
                // Sequence number file does not exist or cannot convert to int, use 1.
                numberValue = 9999;
            }

            numberValue++;
            if (numberValue > 9999)
            {
                numberValue = 0;
            }

            // Re-write to the file (for the next reading)
            using (StreamWriter writer = new StreamWriter(GlobalData.AppPath + GlobalData.SettlementSeqNoPath, false))
            {
                writer.Write(numberValue);
            }
            return numberValue.ToString().PadLeft(4, '0');
        }

        /// <summary>
        /// Get ErrorType and error message from json config file of error message.
        /// </summary>
        /// <param name="key"> Error level: "application", "result", "ResponseCode", "Errorcodedetail" or "ResultCodeExtended".</param>
        /// <param name="errorCode">Error code according to level (string of digits if key is not "application").</param>
        /// <param name="serviceName">ServiceName for Vesca API, must be passed in case the key is Errorcodedetail.</param>
        /// <param name="errorEventMsg">VescaOCX ErrorEvent -> Message, must be passed in case the key is Errorcodedetail</param>
        /// <param name="lastOperator">"0" clerk operation, "1" customer operation, must be passed in case the key is Errorcodedetail</param>
        /// <returns>Error type and error message</returns>
        public static Tuple<ErrorType, string> GetErrorTypeAndMsg(string key, string errorCode, string serviceName = "",
                                                                  string errorEventMsg = "", string lastOperator = "1")
        {
            ErrorType errorTypeResult;
            string errorMsgResult;

            if (key == "result")
            {
                var errorSettingData = GlobalData.MsgCommonOldConfig;
                string errorCodeFound = "default";
                foreach (var item in errorSettingData.result)
                {
                    if (item.Key == errorCode)
                    {
                        errorCodeFound = item.Key;
                        break;
                    }
                }

                errorTypeResult = (ErrorType)int.Parse(errorSettingData.result[errorCodeFound].type);
                errorMsgResult = errorSettingData.result[errorCodeFound].msg.Replace("%code%", errorCodeFound.Replace("default", errorCode));
            }
            else if (key == "ResultCodeExtended")
            {
                var errorSettingData = GlobalData.MsgCommonOldConfig;
                string errorCodeFound = "default";
                foreach (var item in errorSettingData.ResultCodeExtended)
                {
                    if (item.Key == errorCode)
                    {
                        errorCodeFound = item.Key;
                        break;
                    }
                }
                errorTypeResult = (ErrorType)int.Parse(errorSettingData.ResultCodeExtended[errorCodeFound].type);
                errorMsgResult = errorSettingData.ResultCodeExtended[errorCodeFound].msg.Replace("%code%", errorCodeFound.Replace("default", errorCode));
            }
            else if (key == "ResponseCode")
            {
                var errorSettingData = GlobalData.MsgCommonOldConfig;
                string errorCodeFound = "default";
                foreach (var item in errorSettingData.ResponseCode)
                {
                    if (item.Key == errorCode)
                    {
                        errorCodeFound = item.Key;
                        break;
                    }
                }
                errorTypeResult = (ErrorType)int.Parse(errorSettingData.ResponseCode[errorCodeFound].type);
                errorMsgResult = errorSettingData.ResponseCode[errorCodeFound].msg.Replace("%code%", errorCodeFound.Replace("default", errorCode));
            }
            else if (key == "Errorcodedetail")
            {
                dynamic errorSettingData;
                if (serviceName == "iD")
                {
                    errorSettingData = GlobalData.MsgiDConfig;
                }
                else if (serviceName == "WAON")
                {
                    errorSettingData = GlobalData.MsgWAONConfig;
                }
                else
                {
                    // Suica, nanaco, Edy, QUICPay using the same error setting
                    errorSettingData = GlobalData.MsgSuicaConfig;
                }

                // get only 3 last digits of Errorcodedetail (according to error message setting file)
                if (errorCode.Length >= 3)
                {
                    errorCode = errorCode.Substring(errorCode.Length - 3);
                }
                else
                {
                    errorTypeResult = ErrorType.Others;
                    errorMsgResult = errorSettingData.Errorcodedetail["default"].msg.Replace("%code%", errorCode).Replace("%Message%", errorEventMsg);
                    return new Tuple<ErrorType, string>(errorTypeResult, errorMsgResult);
                }

                string errorCodeFound = "default";
                foreach (var item in errorSettingData.Errorcodedetail)
                {
                    if (item.Key == errorCode)
                    {
                        errorCodeFound = item.Key;
                        break;
                    }
                }

                errorTypeResult = (ErrorType)int.Parse(errorSettingData.Errorcodedetail[errorCodeFound].type);
                errorMsgResult = errorSettingData.Errorcodedetail[errorCodeFound].msg.Replace("%code%", errorCodeFound.Replace("default", errorCode))
                                                                                     .Replace("%Message%", errorEventMsg);

                if ((serviceName == "WAON" || serviceName == "iD") && lastOperator == "0")
                {
                    errorMsgResult = errorEventMsg;
                }
            }
            else // key == "application"
            {
                // load error setting data
                var errorSettingData = GlobalData.MsgCommonOldConfig;

                // search by error code
                string errorCodeFound = "default";
                foreach (var item in errorSettingData.application)
                {
                    if (item.Key == errorCode)
                    {
                        errorCodeFound = item.Key;
                        break;
                    }
                }

                // get result error found
                errorTypeResult = (ErrorType)int.Parse(errorSettingData.application[errorCodeFound].type);
                errorMsgResult = errorSettingData.application[errorCodeFound].msg.Replace("%code%", errorCodeFound.Replace("default", errorCode));
            }

            return new Tuple<ErrorType, string>(errorTypeResult, errorMsgResult);
        }

        /*public static void Log(string msg)
        {
            // If log directory don't exists, create it.
            if (!Directory.Exists(GlobalData.AppPath + GlobalData.LogPath))
            {
                new DirectoryInfo(GlobalData.AppPath + GlobalData.LogPath).Create();
            }

            string logFileName = DateTime.Now.Year.ToString()
                                + DateTime.Now.Month.ToString("D2")
                                + DateTime.Now.Day.ToString("D2") + ".log";
            try
            {
                using (StreamWriter logWriter = new StreamWriter(GlobalData.AppPath + GlobalData.LogPath + "/" + logFileName, true))
                {
                    logWriter.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + " " + msg);
                }
            }
            catch { return; }
        }*/

        public static void DeleteOldLog()
        {
            var files = new DirectoryInfo(GlobalData.AppPath + GlobalData.LogPath).GetFiles("*.log");
            foreach (var file in files)
            {
                string fileNameNoExtension = Path.GetFileNameWithoutExtension(file.Name);

                // Not check file name don't match the format: YYYYMMDD.
                if (fileNameNoExtension.Length != 8) continue;
                if (!int.TryParse(fileNameNoExtension.Substring(0, 4), out int year)) continue;
                if (!int.TryParse(fileNameNoExtension.Substring(4, 2), out int month)) continue;
                if (!int.TryParse(fileNameNoExtension.Substring(6, 2), out int day)) continue;

                // Convert to DateTime format if possible.
                if (year < 1 || month < 1 || month > 12) continue;
                if (day < 1 || day > DateTime.DaysInMonth(year, month)) continue;
                DateTime dateTimeFromFileName = new DateTime(year, month, day);

                // Delete the file is older than log_keep_days.
                if (DateTime.Now - dateTimeFromFileName > TimeSpan.FromDays((double)GlobalData.BasicConfig.log_keep_days))
                {
                    File.Delete(file.FullName);
                    Log.Info(string.Format("Delete log file: {0} (log_keep_days: {1})", file.Name, GlobalData.BasicConfig.log_keep_days));
                }
            }
        }

        public static void UpdateTotalInfo(string serviceName, decimal settledAmount, TotalRecordType recordType)
        {
            // If the processed daily total sequence number is recorded, clear the data and record in a new one.
            if (GlobalData.TotalInfo.done_num.Length > 0)
            {
                ClearTotalInfo();
            }

            if (recordType == TotalRecordType.SS)
            {
                GlobalData.TotalInfo.service[serviceName].SS_CNT++;
                GlobalData.TotalInfo.service[serviceName].SS_AMT += decimal.ToInt32(settledAmount);
            }
            else if (recordType == TotalRecordType.NA)
            {
                GlobalData.TotalInfo.service[serviceName].NA_CNT++;
                GlobalData.TotalInfo.service[serviceName].NA_AMT += decimal.ToInt32(settledAmount);
            }
            else if (recordType == TotalRecordType.NASS)
            {
                GlobalData.TotalInfo.service[serviceName].NASS_CNT++;
                GlobalData.TotalInfo.service[serviceName].NASS_AMT += decimal.ToInt32(settledAmount);
            }
            FileStruct.CsFileWrite(GlobalData.AppPath, GlobalData.DailyTotalPath, GlobalData.TotalInfo);
        }

        public static void ClearTotalInfo()
        {
            foreach (var ser in GlobalData.TotalInfo.service)
            {
                ser.Value.SS_CNT = 0;
                ser.Value.SS_AMT = 0;
                ser.Value.NA_CNT = 0;
                ser.Value.NA_AMT = 0;
                ser.Value.NASS_CNT = 0;
                ser.Value.NASS_AMT = 0;
            }
            GlobalData.TotalInfo.done_num = "";

            if (File.Exists(GlobalData.AppPath + GlobalData.DailyTotalPath))
            {
                File.Delete(GlobalData.AppPath + GlobalData.DailyTotalPath);
                Log.Info("Delete " + GlobalData.DailyTotalPath);
            }
        }

        /*public static void PrintReceiptOperation()
        {
            do
            {
                try
                {
                    if (GlobalData.LastOperator == "1") // customer operation
                    {
                        PrintReceipt(PaymentAPI.TempReceiptData[PaymentAPI.RECEIPT_TYPE.RT_CUSTOMER]);
                    }
                    else // clerk operation
                    {
                        PrintReceipt(PaymentAPI.TempReceiptData[PaymentAPI.RECEIPT_TYPE.RT_CUSTOMER]);
                        PrintReceipt(PaymentAPI.TempReceiptData[PaymentAPI.RECEIPT_TYPE.RT_MERCHANT]);
                        PrintReceipt(PaymentAPI.TempReceiptData[PaymentAPI.RECEIPT_TYPE.RT_CARD_COMPANY]);
                        PrintReceipt(PaymentAPI.TempReceiptData[PaymentAPI.RECEIPT_TYPE.RT_TENANT]);
                    }

                    EndPrintReceipt();

                }
                catch (PosControlException ex)
                {
                    Log.Error($"▲ Print error occurred! PosControlException ErrorCode: {ex.ErrorCode}, ErrorCodeExtended: {ex.ErrorCodeExtended}");
                    if (ex.ErrorCode == ErrorCode.Extended && ex.ErrorCodeExtended == 201) Log.Error("Printer Cover is open!");
                    else if (ex.ErrorCode == ErrorCode.Extended && ex.ErrorCodeExtended == 203) Log.Error("Printer runs out of paper!");

                    GlobalData.MPErrorCode = ex.ErrorCode;
                    GlobalData.MPErrorCodeExtended = ex.ErrorCodeExtended;
                    // ShowPrintErrorWindow();
                    //PrintAPI.Printer.ClearOutput();
                    GlobalData.IsPrintError = true;
                }
            } while (GlobalData.IsInPrinterMaintenance);
        }*/

        /*public static void PrintConvertedReceipt(List<PaymentAPI.RowData> convertedReceiptData)
        {
            do
            {
                try
                {
                    PrintReceipt(convertedReceiptData);
                    EndPrintReceipt();
                }
                catch (PosControlException ex)
                {
                    Log.Error($"▲ Print error occurred! PosControlException ErrorCode: {ex.ErrorCode}, ErrorCodeExtended: {ex.ErrorCodeExtended}");
                    if (ex.ErrorCode == ErrorCode.Extended && ex.ErrorCodeExtended == 201) Log.Error("Printer Cover is open!");
                    else if (ex.ErrorCode == ErrorCode.Extended && ex.ErrorCodeExtended == 203) Log.Error("Printer runs out of paper!");

                    GlobalData.MPErrorCode = ex.ErrorCode;
                    GlobalData.MPErrorCodeExtended = ex.ErrorCodeExtended;
                    ShowPrintErrorWindow();
                    //PrintAPI.Printer.ClearOutput();
                }
            } while (GlobalData.IsInPrinterMaintenance);

            PaymentAPI.UnprocessedReceipt.Clear();
        }*/

        /*public static void ReprintConvertedReceipt()
        {
            List<PaymentAPI.RowData> convertedReceiptData = new List<PaymentAPI.RowData>();
            FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.ConvertedReceiptPath, ref convertedReceiptData);
            do
            {
                try
                {
                    PrintReceipt(convertedReceiptData);
                    EndPrintReceipt();
                }
                catch (PosControlException ex)
                {
                    Log.Error($"▲ Print error occurred! PosControlException ErrorCode: {ex.ErrorCode}, ErrorCodeExtended: {ex.ErrorCodeExtended}");
                    if (ex.ErrorCode == ErrorCode.Extended && ex.ErrorCodeExtended == 201) Log.Error("Printer Cover is open!");
                    else if (ex.ErrorCode == ErrorCode.Extended && ex.ErrorCodeExtended == 203) Log.Error("Printer runs out of paper!");

                    GlobalData.MPErrorCode = ex.ErrorCode;
                    GlobalData.MPErrorCodeExtended = ex.ErrorCodeExtended;
                    ShowPrintErrorWindow();
                    //PrintAPI.Printer.ClearOutput();
                }
            }
            while (GlobalData.IsInPrinterMaintenance);
        }*/

        /*public static void PrintReceipt(List<PaymentAPI.RowData> receiptData)
        {
            int lines = receiptData.Count;
            bool condition = lines < 1 || GlobalData.PrinterData.header is null || GlobalData.PrinterData.footer is null;
            if (condition) return;

            if (PrintAPI.Printer != null && PrintAPI.Printer.DeviceEnabled)
            {
                PrintAPI.GetHeaderFooter(PrintAPI.HeaderFooterType.HEADER);

                string logMsg = "Receipt data:";
                string attribute, label, val;
                for (int i = 1; i <= lines; i++)
                {
                    attribute = receiptData[i - 1].Attr;
                    label = receiptData[i - 1].Label;
                    val = receiptData[i - 1].Value;

                    logMsg += $"\r\n\trowid={i} attr={attribute} label={label} value={val}";

                    switch (attribute)
                    {
                        case "Left":
                            PrintAPI.PrintOneRecord(label, val, "Left");
                            break;
                        case "Center":
                            PrintAPI.PrintOneRecord(label, val, "Center");
                            break;
                        case "Right":
                            PrintAPI.PrintOneRecord(label, val, "Right");
                            break;
                        case "Justify":
                            PrintAPI.PrintOneRecord(label, val, "Justify");
                            break;
                        case "Line":
                            PrintAPI.PrintOneRecord(label, val, "Line");
                            break;
                        case "Empty":
                            PrintAPI.PrintOneRecord(label, val, "Empty");
                            break;
                    }
                }
                Log.Info(logMsg);
                PrintAPI.GetHeaderFooter(PrintAPI.HeaderFooterType.FOOTER);
            }
        }*/

        public static void ClearTempReceiptData()
        {
            PaymentAPI.TempReceiptData[PaymentAPI.RECEIPT_TYPE.RT_CUSTOMER].Clear();
            PaymentAPI.TempReceiptData[PaymentAPI.RECEIPT_TYPE.RT_MERCHANT].Clear();
            PaymentAPI.TempReceiptData[PaymentAPI.RECEIPT_TYPE.RT_CARD_COMPANY].Clear();
            PaymentAPI.TempReceiptData[PaymentAPI.RECEIPT_TYPE.RT_TENANT].Clear();
        }

        public static void DeleteConvertedReceiptFile()
        {
            if (File.Exists(GlobalData.AppPath + GlobalData.ConvertedReceiptPath))
            {
                File.Delete(GlobalData.AppPath + GlobalData.ConvertedReceiptPath);
                Log.Info("Delete " + GlobalData.ConvertedReceiptPath);
            }
        }

        public static List<PaymentAPI.RowData> ConvertReceipt()
        {
            GlobalData.IsConvertReceiptError = false;
            List<PaymentAPI.RowData> resultReceipt = new List<PaymentAPI.RowData>();

            FileStruct.convReceipt_detail[] convReceiptConfig;
            switch (GlobalData.PreCurrentServiceSV)
            {
                case "Suica":
                    convReceiptConfig = GlobalData.ConvReceiptConfig.Suica;
                    break;
                case "Edy":
                    convReceiptConfig = GlobalData.ConvReceiptConfig.Edy;
                    break;
                case "nanaco":
                    convReceiptConfig = GlobalData.ConvReceiptConfig.nanaco;
                    break;
                case "WAON":
                    convReceiptConfig = GlobalData.ConvReceiptConfig.WAON;
                    break;
                default:
                    convReceiptConfig = new FileStruct.convReceipt_detail[0];
                    break;
            }

            int resultRowId = 0;
            for (int i = 0; i < convReceiptConfig.Length; i++)
            {
                if (convReceiptConfig[i].type == 0)
                {
                    // Get receipt data from value of the "data" key
                    for (int j = 0; j < convReceiptConfig[i].data.Length; j++)
                    {
                        resultRowId++;
                        resultReceipt.Add(new PaymentAPI.RowData
                        {
                            RowId = resultRowId,
                            Attr = convReceiptConfig[i].data[j].Attr,
                            Label = convReceiptConfig[i].data[j].Label,
                            Value = convReceiptConfig[i].data[j].Value,
                        });
                    }
                }
                else // type = 1
                {
                    // Find rowid range to get from unfinished progressing receipt data
                    var areaConfig = convReceiptConfig[i].area;
                    int startId = FindRowIdForArea(1, areaConfig.st, areaConfig.stkey, areaConfig.stpos);
                    if (startId == 0)
                    {
                        GlobalData.IsConvertReceiptError = true;
                        Log.Error($"Not found rowid with st={areaConfig.st} stkey={areaConfig.stkey}");
                        // return nothing if error
                        return new List<PaymentAPI.RowData>();
                    }
                    int endId = FindRowIdForArea(startId, areaConfig.ed, areaConfig.edkey, areaConfig.edpos);
                    if (endId == 0)
                    {
                        GlobalData.IsConvertReceiptError = true;
                        Log.Error($"Not found rowid with ed={areaConfig.ed} edkey={areaConfig.edkey}");
                        // return nothing if error
                        return new List<PaymentAPI.RowData>();
                    }

                    // Get data area from startId to endId of rowid
                    foreach (var row in PaymentAPI.UnprocessedReceipt)
                    {
                        if (row.RowId < startId || row.RowId > endId) continue;

                        resultRowId++;
                        resultReceipt.Add(new PaymentAPI.RowData
                        {
                            RowId = resultRowId,
                            Attr = row.Attr,
                            Label = row.Label,
                            Value = row.Value,
                        });
                    }
                }
            }

            // Save result to ConvertedReceipt.dat (for reprinting receipt)
            FileStruct.CsFileWrite(GlobalData.AppPath, GlobalData.ConvertedReceiptPath, resultReceipt, writeLog: false);
            Log.Info("Save converted receipt to " + GlobalData.ConvertedReceiptPath);

            // Save journal file
            SaveJournalFile(PaymentAPI.SeqNumOfUnprocessedReceipt, PaymentAPI.RECEIPT_TYPE.RT_CUSTOMER, resultReceipt, isConvReceipt: true);

            return resultReceipt;
        }

        private static int FindRowIdForArea(int fromId, string st_ed, string stkey_edkey, int stpos_edpos)
        {
            var data = PaymentAPI.UnprocessedReceipt;
            if (st_ed == "label")
            {
                foreach (var row in data)
                {
                    if (row.RowId < fromId) continue;
                    if (row.Label.Contains(stkey_edkey)) return (row.RowId + stpos_edpos);
                }
            }
            else // st = "value"
            {
                foreach (var row in data)
                {
                    if (row.RowId < fromId) continue;
                    if (row.Value.Contains(stkey_edkey)) return (row.RowId + stpos_edpos);
                }
            }
            return 0; // not found
        }

        public static void ShowPrintErrorWindow()
        {
            // [Reprint] button normal release
            GlobalData.MPPressReprint = false;

            // Invoke new window to enter maintenance mode if currently not in maintenance mode,
            // update rrror message of emMP_EMSG screen if currently in maintenance mode
            if (!GlobalData.IsInPrinterMaintenance)
            {
                GlobalData.IsInPrinterMaintenance = true;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    GlobalData.PrintErrorWindow = new Window
                    {
                        Topmost = true,
                        Title = "PrintError",
                        WindowStyle = WindowStyle.None,
                        WindowState = WindowState.Normal,
                        ResizeMode = ResizeMode.NoResize,
                        WindowStartupLocation = WindowStartupLocation.Manual,
                        Owner = Application.Current.MainWindow,
                        Top = Application.Current.MainWindow.Top,
                        Left = Application.Current.MainWindow.Left,
                        Width = Application.Current.MainWindow.Width,
                        Height = Application.Current.MainWindow.Height,
                        Content = new Views.PrintErrorWindow() { DataContext = new ViewModels.PrintErrorViewModel() }
                    };
                    GlobalData.PrintErrorWindow.Show();
                });
            }
            else
            {
                string errMsg;
                if (GlobalData.MPErrorCode == ErrorCode.Extended && GlobalData.MPErrorCodeExtended == 201)
                {
                    // errMsg = GetErrorTypeAndMsg("application", "AE9").Item2;
                    errMsg = GlobalData.MsgModeConfig.Find(m => m.msg_code == "B0022").msg2;
                }
                else if (GlobalData.MPErrorCode == ErrorCode.Extended && GlobalData.MPErrorCodeExtended == 203)
                {
                    // errMsg = GetErrorTypeAndMsg("application", "AE10").Item2;
                    errMsg = GlobalData.MsgModeConfig.Find(m => m.msg_code == "B0021").msg2;
                }
                else
                {
                    // errMsg = GetErrorTypeAndMsg("application", "AE7").Item2;
                    errMsg = GlobalData.MsgModeConfig.Find(m => m.msg_code == "B0023").msg2;
                }
                ViewModels.PrintErrorViewModel._viewModel_emMP_EMSG.ErrorMessage = errMsg;
                ViewModels.PrintErrorViewModel._viewModel_emMP_EMSG.UpdateView();
            }

            // Wait for press [Reprint] or [End] button
            while (!GlobalData.MPPressReprint && GlobalData.IsInPrinterMaintenance) Thread.Sleep(100);

            // Release [Reprint] button
            GlobalData.MPPressReprint = false;
        }

        /*public static void EndPrintReceipt()
        {
            if (GlobalData.PrintErrorWindow != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    GlobalData.PrintErrorWindow.Close();
                });
                GlobalData.PrintErrorWindow = null;
            }
            GlobalData.IsInPrinterMaintenance = false;

            ClearTempReceiptData();
        }*/

        public static void SaveJournalFile(int sequenceNumber, PaymentAPI.RECEIPT_TYPE type, List<PaymentAPI.RowData> data, bool isConvReceipt = false)
        {
            // If have no data, do nothing (not create emty file)
            if (data.Count == 0) return;

            // Specify folder name and file name to save
            string seq = sequenceNumber.ToString();
            string type_num;
            switch (type)
            {
                case PaymentAPI.RECEIPT_TYPE.RT_CUSTOMER: type_num = "0"; break;
                case PaymentAPI.RECEIPT_TYPE.RT_MERCHANT: type_num = "1"; break;
                case PaymentAPI.RECEIPT_TYPE.RT_CARD_COMPANY: type_num = "2"; break;
                case PaymentAPI.RECEIPT_TYPE.RT_TENANT: type_num = "3"; break;
                default: type_num = "n"; break;
            }
            DateTime timeEventOccur = isConvReceipt ? PaymentAPI.TimeOfUnprocessedReceipt : PaymentAPI.LastTimeEventOccur;
            string folderPath = GlobalData.TransactionDataPath + @"\" + timeEventOccur.ToString("yyyyMMdd");
            string fileName = @"\" + timeEventOccur.ToString("HHmmss") + $"_{seq}_{type_num}.dat";

            // If folder don't exists, create it
            if (!Directory.Exists(GlobalData.AppPath + folderPath))
            {
                new DirectoryInfo(GlobalData.AppPath + folderPath).Create();
            }

            // Write data to file
            FileStruct.CsFileWrite(GlobalData.AppPath, folderPath + fileName, data, writeLog: false);
            Log.Info("Saved receipt data to " + folderPath + fileName);
        }

        public static void PlaySound(string fileName, bool isPlayOnce = false)
        {
            string filePath = GlobalData.AppPath + GlobalData.SoundPath + @"\" + fileName;
            if (!File.Exists(filePath))
            {
                Log.Error($"▲ {fileName} does not exist!");
                return;
            }

            bool isScreenChanged = false;

            Task detectScreenChange = Task.Run(async () =>
            {
                StateMachine.State currentState = Session.ScreenState.CurrentState;
                while (true)
                {
                    if (Session.ScreenState.CurrentState != currentState)
                    {
                        isScreenChanged = true;
                        break;
                    }

                    // detectScreenChange should run as fast as possible ? 
                    // Below seem to be work just fine
                    await Task.Delay(10);
                }
            });

            Task taskPlaySound = Task.Run(async () =>
            {
                // Delay sound_interval_time before playing sound
                await Task.Delay((int)GlobalData.BasicConfig.sound_ready_time);

                // Screen changed before playing sound
                if (isScreenChanged) return;

                bool stopSoundLoop = false;
                while (true)
                {
                    Session.WMP.Play(filePath);

                    while (Session.WMP.IsPlaying)
                    {
                        // Check if screen change while sound is playing
                        if (isScreenChanged)
                        {
                            stopSoundLoop = true;
                            Session.WMP.CloseCurrent();
                            break;
                        }
                        await Task.Delay(10);
                    }

                    // If setting sound loop only once
                    if (isPlayOnce) break;

                    // End the sound loop if screen has changed
                    if (stopSoundLoop) break;

                    // If not changing screen and playing sound is finished,
                    // delay sound_interval_time and then replay.
                    if (!Session.WMP.IsPlaying)
                    {
                        bool stopLoop = false;
                        int count = (int)GlobalData.BasicConfig.sound_interval_time;

                        while (count > 0)
                        {
                            if (isScreenChanged)
                            {
                                stopLoop = true;
                                break;
                            }

                            // Decrease interval time
                            await Task.Delay(100); // Replace Thread.Sleep by Task.Delay  
                            count -= 100;
                        }

                        if (stopLoop) break;
                    }
                }
            });
        }

        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string GenerateOutputID()
        {
            DateTime firstDay = new DateTime(2000, 1, 1);
            DateTime now = DateTime.Now; // DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
            string days = (now - firstDay).Days.ToString().PadLeft(5, '0');
            string nanoSecond = (now.Ticks % 1000).ToString().PadLeft(3, '0'); // Actually, this is Ticks, 10^-7 second, not nano 10^-9 second
            return $"{GlobalData.BasicConfig.setting_pos_no}{days}{now.Hour.ToString().PadLeft(2, '0')}" +
                $"{now.Minute.ToString().PadLeft(2, '0')}{now.Second.ToString().PadLeft(2, '0')}{nanoSecond}";
        }

        // Parameters:
        //   start:
        //     The zero-based starting index for masking.
        //   end:
        //     The zero-based ending index for masking (include this index).
        public static string MaskString(string input, int start, int end, char maskingChar = '*')
        {
            int charsToMask = end - start + 1;
            return charsToMask > 0 ? $"{input.Substring(0, start)}{new string(maskingChar, charsToMask)}{input.Substring(end + 1)}" : input;
        }

        public static void ExitApp()
        {
            Log.Info("アプリ終了");
            Application.Current.Shutdown();
        }

        public static (List<FileStruct.RowData>, List<FileStruct.RowData>) PreparePaymentReceiptData(FileStruct.SaturnAPIResponse saturnAPIResponse)
        {
            List<FileStruct.RowData> printReceiptData = new List<FileStruct.RowData>();
            List<FileStruct.RowData> saveReceiptData = new List<FileStruct.RowData>();
            string[] spearator = { "\\n" };
            string[] infoLines = saturnAPIResponse.bizInfo.info.Split(spearator, StringSplitOptions.RemoveEmptyEntries);

            List<FileStruct.RowData> GeneralData = new List<FileStruct.RowData>();

            switch (GlobalData.ServiceName)
            {
                case "Suica":
                    if (saturnAPIResponse.bizInfo.tradeStatus == "01" || string.IsNullOrEmpty(saturnAPIResponse.bizInfo.tradeStatus))
                    {
                        // Create General Data
                        GeneralData.Add(new FileStruct.RowData { Label = "", Value = "", Attr = "Header" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 1, Label = "", Value = "＜" + GetBrandName(GlobalData.ServiceName) + "＞", Attr = "Center" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 2, Label = "", Value = "［支払票（支払）］", Attr = "Center" });
                        GeneralData.Add(new FileStruct.RowData { Label = "", Value = "1", Attr = "Empty" });
                        GeneralData.Add(new FileStruct.RowData { Label = "", Value = "加盟店名", Attr = "Left" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 3, Label = "", Value = "  " + saturnAPIResponse.bizInfo.merchInfo1, Attr = "Left" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 4, Label = "", Value = "  " + saturnAPIResponse.bizInfo.merchInfo2, Attr = "Left" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 5, Label = "店舗端末ＩＤ", Value = saturnAPIResponse.bizInfo.rwId, Attr = "Justify" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 6, Label = "端末番号", Value = saturnAPIResponse.bizInfo.termIdentityNo, Attr = "Justify" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 7, Label = "伝票番号", Value = saturnAPIResponse.bizInfo.slipNo, Attr = "Justify" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 8, Label = "ご利用日時", Value = saturnAPIResponse.bizInfo.useDateTime, Attr = "Justify" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 9, Label = "カード番号", Value = MaskString(saturnAPIResponse.bizInfo.customer, 2, 12), Attr = "Justify" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 10, Label = "支払金額", Value = "￥" + string.Format("{0:N0}", saturnAPIResponse.bizInfo.tradeAmount), Attr = "Justify" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 11, Label = "カード残高", Value = "￥" + string.Format("{0:N0}", saturnAPIResponse.bizInfo.cardBalance), Attr = "Justify" });
                        GeneralData.Add(new FileStruct.RowData { Label = "", Value = "1", Attr = "Empty" });
                        foreach (string line in infoLines)
                        {
                            GeneralData.Add(new FileStruct.RowData { RowID = 12, Label = "", Value = line, Attr = "Left" });
                        }
                        GeneralData.Add(new FileStruct.RowData { Label = "", Value = "1", Attr = "Empty" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 1314, Label = "売場：", Value = "係員：", Attr = "LeftCenter" });
                        GeneralData.Add(new FileStruct.RowData { Label = "", Value = "1", Attr = "Empty" });

                        List<FileStruct.RowData> CustomerData = (new List<FileStruct.RowData>()).Concat(GeneralData).ToList();
                        List<FileStruct.RowData> CompanyData = (new List<FileStruct.RowData>()).Concat(GeneralData).ToList();
                        List<FileStruct.RowData> MerchantData = (new List<FileStruct.RowData>()).Concat(GeneralData).ToList();
                        List<FileStruct.RowData> AggerateData = (new List<FileStruct.RowData>()).Concat(GeneralData).ToList();

                        // Customer Data
                        CustomerData.Add(new FileStruct.RowData { RowID = 15, Label = "", Value = "お客様控え", Attr = "Center" });
                        CustomerData.Add(new FileStruct.RowData { Label = "", Value = "", Attr = "Footer" });
                        // Company Data
                        CompanyData.Add(new FileStruct.RowData { RowID = 15, Label = "", Value = "カード会社控え", Attr = "Right" });
                        CompanyData.Add(new FileStruct.RowData { Label = "", Value = "", Attr = "Footer" });
                        // Merchant Data
                        MerchantData.Add(new FileStruct.RowData { RowID = 15, Label = "", Value = "加盟店控え", Attr = "Right" });
                        MerchantData.Add(new FileStruct.RowData { RowID = 16, Label = "", Value = saturnAPIResponse.bizInfo.seqNo, Attr = "Right" });
                        MerchantData.Add(new FileStruct.RowData { Label = "", Value = "", Attr = "Footer" });
                        // Aggerate Data
                        AggerateData.Add(new FileStruct.RowData { RowID = 15, Label = "", Value = GlobalData.BasicConfig.receipt_print_aggregate_title, Attr = "Right" });
                        AggerateData.Add(new FileStruct.RowData { RowID = 16, Label = "", Value = saturnAPIResponse.bizInfo.seqNo, Attr = "Right" });
                        AggerateData.Add(new FileStruct.RowData { Label = "", Value = "", Attr = "Footer" });

                        // add Print Data
                        printReceiptData = printReceiptData.Concat(CustomerData).ToList();
                        if (checkPrint(GlobalData.BasicConfig.receipt_print_company_setting)) printReceiptData =  printReceiptData.Concat(CompanyData).ToList();
                        if (checkPrint(GlobalData.BasicConfig.receipt_print_merchant_setting)) printReceiptData = printReceiptData.Concat(MerchantData).ToList();
                        if (checkPrint(GlobalData.BasicConfig.receipt_print_aggregate_setting)) printReceiptData = printReceiptData.Concat(AggerateData).ToList();
                        // Add Save Data
                        if (checkSave(GlobalData.BasicConfig.receipt_print_customer_setting)) saveReceiptData = saveReceiptData.Concat(CustomerData).ToList();
                        if (checkSave(GlobalData.BasicConfig.receipt_print_company_setting)) saveReceiptData = saveReceiptData.Concat(CompanyData).ToList();
                        if (checkSave(GlobalData.BasicConfig.receipt_print_merchant_setting)) saveReceiptData = saveReceiptData.Concat(MerchantData).ToList();
                        if (checkSave(GlobalData.BasicConfig.receipt_print_aggregate_setting)) saveReceiptData = saveReceiptData.Concat(AggerateData).ToList();
                    }
                    else if (saturnAPIResponse.bizInfo.tradeStatus == "02")
                    {
                        string unfinishedCardNumber = saturnAPIResponse.bizInfo.unFinished1 + saturnAPIResponse.bizInfo.unFinished2 + saturnAPIResponse.bizInfo.unFinished3 + saturnAPIResponse.bizInfo.unFinished4;
                        unfinishedCardNumber = MaskString(unfinishedCardNumber, 2, 12);
                        // General Data
                        GeneralData.Add(new FileStruct.RowData { Label = "", Value = "", Attr = "Header" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 1, Label = "", Value = "＜" + GetBrandName(GlobalData.ServiceName) + "＞", Attr = "Center" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 2, Label = "", Value = "［支払票（支払）］", Attr = "Center" });
                        GeneralData.Add(new FileStruct.RowData { Label = "", Value = "1", Attr = "Empty" });
                        GeneralData.Add(new FileStruct.RowData { Label = "", Value = "加盟店名", Attr = "Left" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 3, Label = "", Value = "  " + saturnAPIResponse.bizInfo.merchInfo1, Attr = "Left" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 4, Label = "", Value = "  " + saturnAPIResponse.bizInfo.merchInfo2, Attr = "Left" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 5, Label = "店舗端末ＩＤ", Value = saturnAPIResponse.bizInfo.rwId, Attr = "Justify" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 6, Label = "端末番号", Value = saturnAPIResponse.bizInfo.termIdentityNo, Attr = "Justify" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 7, Label = "伝票番号", Value = saturnAPIResponse.bizInfo.slipNo, Attr = "Justify" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 8, Label = "ご利用日時", Value = saturnAPIResponse.bizInfo.useDateTime, Attr = "Justify" });
                        GeneralData.Add(new FileStruct.RowData { Label = "", Value = "1", Attr = "Empty" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 17, Label = "", Value = "＊＊＊　処理未了　＊＊＊", Attr = "Center" });
                        GeneralData.Add(new FileStruct.RowData { Label = "", Value = "1", Attr = "Empty" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 9, Label = "カード番号", Value = unfinishedCardNumber, Attr = "Justify" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 10, Label = "支払金額", Value = "￥" + string.Format("{0:N0}", saturnAPIResponse.bizInfo.tradeAmount), Attr = "Justify" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 11, Label = "取引前カード残高", Value = "￥" + string.Format("{0:N0}", saturnAPIResponse.bizInfo.beforeCardBalance), Attr = "Justify" });
                        GeneralData.Add(new FileStruct.RowData { Label = "", Value = "1", Attr = "Empty" });
                        foreach (string line in infoLines)
                        {
                            GeneralData.Add(new FileStruct.RowData { RowID = 12, Label = "", Value = line, Attr = "Left" });
                        }
                        GeneralData.Add(new FileStruct.RowData { Label = "", Value = "1", Attr = "Empty" });
                        GeneralData.Add(new FileStruct.RowData { RowID = 1314, Label = "売場：", Value = "係員：", Attr = "LeftCenter" });
                        GeneralData.Add(new FileStruct.RowData { Label = "", Value = "1", Attr = "Empty" });

                        List<FileStruct.RowData> CustomerData = (new List<FileStruct.RowData>()).Concat(GeneralData).ToList();
                        List<FileStruct.RowData> CompanyData = (new List<FileStruct.RowData>()).Concat(GeneralData).ToList();
                        List<FileStruct.RowData> MerchantData = (new List<FileStruct.RowData>()).Concat(GeneralData).ToList();
                        List<FileStruct.RowData> AggerateData = (new List<FileStruct.RowData>()).Concat(GeneralData).ToList();

                        // Customer Data
                        CustomerData.Add(new FileStruct.RowData { RowID = 15, Label = "", Value = "お客様控え", Attr = "Center" });
                        CustomerData.Add(new FileStruct.RowData { Label = "", Value = "", Attr = "Footer" });
                        // Company Data
                        CompanyData.Add(new FileStruct.RowData { RowID = 15, Label = "", Value = "カード会社控え", Attr = "Right" });
                        CompanyData.Add(new FileStruct.RowData { Label = "", Value = "", Attr = "Footer" });
                        // Merchant Data
                        MerchantData.Add(new FileStruct.RowData { RowID = 15, Label = "", Value = "加盟店控え", Attr = "Right" });
                        MerchantData.Add(new FileStruct.RowData { RowID = 16, Label = "", Value = saturnAPIResponse.bizInfo.seqNo, Attr = "Right" });
                        MerchantData.Add(new FileStruct.RowData { Label = "", Value = "", Attr = "Footer" });
                        // Aggerate Data
                        AggerateData.Add(new FileStruct.RowData { RowID = 15, Label = "", Value = GlobalData.BasicConfig.receipt_print_aggregate_title, Attr = "Right" });
                        AggerateData.Add(new FileStruct.RowData { RowID = 16, Label = "", Value = saturnAPIResponse.bizInfo.seqNo, Attr = "Right" });
                        AggerateData.Add(new FileStruct.RowData { Label = "", Value = "", Attr = "Footer" });

                        printReceiptData = printReceiptData.Concat(CompanyData).ToList();
                        if (checkPrint(GlobalData.BasicConfig.receipt_print_customer_unfinished_setting)) printReceiptData = printReceiptData.Concat(CustomerData).ToList();
                        if (checkPrint(GlobalData.BasicConfig.receipt_print_merchant_unfinished_setting)) printReceiptData = printReceiptData.Concat(MerchantData).ToList();
                        if (checkPrint(GlobalData.BasicConfig.receipt_print_aggregate_unfinished_setting)) printReceiptData = printReceiptData.Concat(AggerateData).ToList();
                        // Add Save Data
                        if (checkSave(GlobalData.BasicConfig.receipt_print_customer_unfinished_setting)) saveReceiptData =  saveReceiptData.Concat(CustomerData).ToList();
                        if (checkSave(GlobalData.BasicConfig.receipt_print_company_unfinished_setting)) saveReceiptData = saveReceiptData.Concat(CompanyData).ToList();
                        if (checkSave(GlobalData.BasicConfig.receipt_print_merchant_unfinished_setting)) saveReceiptData = saveReceiptData.Concat(MerchantData).ToList();
                        if (checkSave(GlobalData.BasicConfig.receipt_print_aggregate_unfinished_setting)) saveReceiptData = saveReceiptData.Concat(AggerateData).ToList();
                    }
                    break;
                default:
                    break;
            }
            return (printReceiptData, saveReceiptData);
        }

        public static List<FileStruct.RowData> PrepareBalanceReceiptData(FileStruct.SaturnAPIResponse saturnAPIResponse)
        {
            List<FileStruct.RowData> printReceiptData = new List<FileStruct.RowData>();
            string[] spearator = { "\\n" };

            if (saturnAPIResponse.controlInfo.procResult == "1")
            {
                string[] infoLines = saturnAPIResponse.bizInfo.info.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                printReceiptData.Add(new FileStruct.RowData { Label = "", Value = "", Attr = "Header" });
                printReceiptData.Add(new FileStruct.RowData { RowID = 1, Label = "", Value = "＜" + GetBrandName(GlobalData.ServiceName) + "＞", Attr = "Center" });
                printReceiptData.Add(new FileStruct.RowData { RowID = 2, Label = "", Value = "［残高照会］", Attr = "Center" });
                printReceiptData.Add(new FileStruct.RowData { Label = "", Value = "1", Attr = "Empty" });
                printReceiptData.Add(new FileStruct.RowData { Label = "", Value = "加盟店名", Attr = "Left" });
                printReceiptData.Add(new FileStruct.RowData { RowID = 3, Label = "", Value = "  " + saturnAPIResponse.bizInfo.merchInfo1, Attr = "Left" });
                printReceiptData.Add(new FileStruct.RowData { RowID = 4, Label = "", Value = "  " + saturnAPIResponse.bizInfo.merchInfo2, Attr = "Left" });
                printReceiptData.Add(new FileStruct.RowData { RowID = 5, Label = "端末番号", Value = saturnAPIResponse.bizInfo.termIdentityNo, Attr = "Justify" });
                printReceiptData.Add(new FileStruct.RowData { RowID = 6, Label = "ご利用日時", Value = saturnAPIResponse.bizInfo.useDateTime, Attr = "Justify" });
                printReceiptData.Add(new FileStruct.RowData { RowID = 789, Label = "", Value = saturnAPIResponse.bizInfo.resultCode + " " + saturnAPIResponse.bizInfo.resultCodeDetail + " " + saturnAPIResponse.bizInfo.centerResultCode, Attr = "Left" });
                printReceiptData.Add(new FileStruct.RowData { Label = "", Value = "1", Attr = "Empty" });
                foreach (string line in infoLines)
                {
                    printReceiptData.Add(new FileStruct.RowData { RowID = 12, Label = "", Value = line, Attr = "Left" });
                }
                printReceiptData.Add(new FileStruct.RowData { Label = "", Value = "", Attr = "Footer" });
            }
            else if (saturnAPIResponse.controlInfo.procResult == "9")
            {
                string errorCodeDetail1 = GlobalData.SaturnAPIResponse.controlInfo.errorCodeDetail1;
                FileStruct.Msg_Brand msg_Brand = FileStruct.Find(GlobalData.MsgBrandConfig, b => b.brand_code == GlobalData.ServiceName, GlobalData.ServiceName);
                FileStruct.MsgDetail msg = FileStruct.Find(msg_Brand.message, m => m.msg_code == errorCodeDetail1);
                if (msg.type != "0" && msg.type != "3")
                {
                    printReceiptData.Add(new FileStruct.RowData { Label = "", Value = "", Attr = "Header" });
                    printReceiptData.Add(new FileStruct.RowData { RowID = 1, Label = "", Value = "＜" + GetBrandName(GlobalData.ServiceName) + "＞", Attr = "Center" });
                    printReceiptData.Add(new FileStruct.RowData { RowID = 2, Label = "", Value = "［残高照会］", Attr = "Center" });
                    printReceiptData.Add(new FileStruct.RowData { Label = "", Value = "1", Attr = "Empty" });
                    printReceiptData.Add(new FileStruct.RowData { RowID = 7, Label = "取引日時", Value = saturnAPIResponse.controlInfo.dateTime, Attr = "Justify" });
                    printReceiptData.Add(new FileStruct.RowData { RowID = 6, Label = "端末固有ID", Value = saturnAPIResponse.controlInfo.termIdentityNo, Attr = "Justify" });
                    printReceiptData.Add(new FileStruct.RowData { RowID = 3, Label = "エラーコード", Value = saturnAPIResponse.controlInfo.errorCode, Attr = "Justify" });
                    printReceiptData.Add(new FileStruct.RowData { RowID = 4, Label = "エラーコード詳細1", Value = saturnAPIResponse.controlInfo.errorCodeDetail1, Attr = "Justify" });
                    printReceiptData.Add(new FileStruct.RowData { RowID = 5, Label = "エラーコード詳細2", Value = saturnAPIResponse.controlInfo.errorCodeDetail2, Attr = "Justify" });
                    printReceiptData.Add(new FileStruct.RowData { Label = "", Value = "1", Attr = "Empty" });
                    printReceiptData.Add(new FileStruct.RowData { RowID = 8, Label = "", Value = msg.msg, Attr = "Left" });
                    printReceiptData.Add(new FileStruct.RowData { Label = "", Value = "", Attr = "Footer" });
                }
            }
            return printReceiptData;
        }

        public static void PrintReceipt(List<FileStruct.RowData> receiptData)
        {
            int lines = receiptData.Count;
            bool isConditionOK = lines > 0; // && GlobalData.PrinterData.header != null && GlobalData.PrinterData.footer != null;
            if (!isConditionOK) return;

            if (PrintAPI.Printer != null && PrintAPI.Printer.DeviceEnabled)
            {
                string logMsg = "Receipt data:";
                string attribute, label, val;
                FileStruct.RowData row = new FileStruct.RowData();
                // PrintAPI.GetHeaderFooter(PrintAPI.HeaderFooterType.HEADER);

                for (int i = 0; i < lines; i++)
                {
                    row = receiptData[i];
                    attribute = row.Attr;
                    label = row.Label;
                    val = row.Value;

                    if (attribute == "Header")
                    {
                        PrintAPI.Printer.TransactionPrint(PrinterStation.Receipt, PrinterTransactionControl.Transaction);
                        logMsg += "\nStart TransactionPrint";
                        PrintAPI.Printer.RecLineChars = GlobalData.PrinterData.width;
                        logMsg += "\nPosPrinter.json >> width: " + GlobalData.PrinterData.width + ", Printer.RecLineChars: " + PrintAPI.Printer.RecLineChars + ", Printer.RecLineCharsList: ";
                        Array.ForEach(PrintAPI.Printer.RecLineCharsList, item => logMsg += item + " ");
                    }
                    else if (attribute == "Footer")
                    {
                        PrintAPI.Printer.PrintNormal(PrinterStation.Receipt, PrintAPI.OPOS_PTR_ESC + "|1C");
                        logMsg += "\nPrint PN_ESC: " + PrintAPI.OPOS_PTR_ESC + "|1C";
                        PrintAPI.Printer.TransactionPrint(PrinterStation.Receipt, PrinterTransactionControl.Normal);
                        logMsg += "\nEnd TransactionPrint";
                        int times = 4;
                        while (times > 0)
                        {
                            PrintAPI.Printer.PrintNormal(PrinterStation.Receipt, PrintAPI.OPOS_PTR_CRLF);
                            --times;
                        }
                        logMsg = "\nPrint PN_CRLF: " + times;
                        PrintAPI.Printer.PrintNormal(PrinterStation.Receipt, PrintAPI.OPOS_PTR_ESC + "|P");
                        logMsg += "\nPrint PN_ESC: " + PrintAPI.OPOS_PTR_ESC + "|P";
                    }
                    else
                    {
                        PrintAPI.PrintOneRecord(label, val, attribute);
                        logMsg += $"\r\n\trowid={row.RowID} attr={attribute} label={label} value={val}";
                    }
                }
                // PrintAPI.GetHeaderFooter(PrintAPI.HeaderFooterType.FOOTER);
                Log.Info(logMsg);
            }
        }

        public static void InitDailyTotal()
        {
            if (!Directory.Exists(GlobalData.AppPath + GlobalData.TransactionDataPath)) Directory.CreateDirectory(GlobalData.AppPath + GlobalData.TransactionDataPath);

            FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.DailyTotalPath, ref GlobalData.DailyTotalData);
            foreach (var service in GlobalData.BasicConfig.btn_order_ReadValue)
            {
                var temp = GlobalData.DailyTotalData.brand.Find(b => b.brand_code == service);
                if (temp == null)
                {
                    GlobalData.DailyTotalData.brand.Add(new FileStruct.BrandSumaryData { brand_code = service });
                }
            }
            FileStruct.CsFileWrite(GlobalData.AppPath, GlobalData.DailyTotalPath, GlobalData.DailyTotalData);
        }

        public static bool checkPrint(string flag)
        {
            if (flag == "1" || flag == "3") return true;
            return false;
        }
        public static bool checkSave(string flag)
        {
            if (flag == "1" || flag == "2") return true;
            return false;
        }
    }
}
