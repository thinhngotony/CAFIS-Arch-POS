using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPSaturnEMoney.Models;

namespace WPSaturnEMoney.Common
{
    class Test
    {
        public static string ConfigTestPath = @"\Test\ConfigTest.json";
        public static string SuicaReceipPath = @"\Test\UnprocessedReceipt\Suica.json";
        public static string nanacoReceiptPath = @"\Test\UnprocessedReceipt\nanaco.json";
        public static string EdyReceiptPath = @"\Test\UnprocessedReceipt\Edy.json";
        public static string WAONReceiptPath = @"\Test\UnprocessedReceipt\WAON.json";
        public static string iDReceiptPath = @"\Test\UnprocessedReceipt\iD.json";
        public static string QUICPayReceiptPath = @"\Test\UnprocessedReceipt\QUICPay.json";
        public static Suica SuicaUnprocessedReceipt = new Suica();
        public static nanaco nanacoUnprocessedReceipt = new nanaco();
        public static Edy EdyUnprocessedReceipt = new Edy();
        public static WAON WAONUnprocessedReceipt = new WAON();
        public static iD iDUnprocessedReceipt = new iD();
        public static QUICPay QUICPayUnprocessedReceipt = new QUICPay();

        public static bool IsTestUnfinishedProcessing(string serviceName)
        {
            ConfigTest cfg = new ConfigTest { test_mode = 0 };

            // Read and check config file
            bool isSucess = FileStruct.CsFileRead(GlobalData.AppPath, ConfigTestPath, ref cfg);
            if (!isSucess || FileStruct.IsNullorEmpty(cfg)) return false;
            
            // Read and check receipt data file for testing
            if (serviceName == "Suica")
            {
                bool isReadSucess = FileStruct.CsFileRead(GlobalData.AppPath, SuicaReceipPath, ref SuicaUnprocessedReceipt);
                if (!isReadSucess || FileStruct.IsNullorEmpty(SuicaUnprocessedReceipt)) return false;
            }
            else if (serviceName == "nanaco")
            {
                bool isReadSucess = FileStruct.CsFileRead(GlobalData.AppPath, nanacoReceiptPath, ref nanacoUnprocessedReceipt);
                if (!isReadSucess || FileStruct.IsNullorEmpty(nanacoUnprocessedReceipt)) return false;
            }
            else if (serviceName == "Edy")
            {
                bool isReadSucess = FileStruct.CsFileRead(GlobalData.AppPath, EdyReceiptPath, ref EdyUnprocessedReceipt);
                if (!isReadSucess || FileStruct.IsNullorEmpty(EdyUnprocessedReceipt)) return false;
            }
            else if (serviceName == "WAON")
            {
                bool isReadSucess = FileStruct.CsFileRead(GlobalData.AppPath, WAONReceiptPath, ref WAONUnprocessedReceipt);
                if (!isReadSucess || FileStruct.IsNullorEmpty(WAONUnprocessedReceipt)) return false;
            }
            else if (serviceName == "iD")
            {
                bool isReadSucess = FileStruct.CsFileRead(GlobalData.AppPath, iDReceiptPath, ref iDUnprocessedReceipt);
                if (!isReadSucess || FileStruct.IsNullorEmpty(iDUnprocessedReceipt)) return false;
            }
            else if (serviceName == "QUICPay")
            {
                bool isReadSucess = FileStruct.CsFileRead(GlobalData.AppPath, QUICPayReceiptPath, ref QUICPayUnprocessedReceipt);
                if (!isReadSucess || FileStruct.IsNullorEmpty(QUICPayUnprocessedReceipt)) return false;
            }
            else
            {
                return false;
            }
            
            // Check mode is configured
            if (cfg.test_mode == 0)
            {
                return false;
            }
            else // test_mode != 0;
            {
                bool isConfigedTest = (serviceName == "Suica" && cfg.Suica != 0) ||
                                      (serviceName == "nanaco" && cfg.nanaco != 0) ||
                                      (serviceName == "Edy" && cfg.Edy != 0) ||
                                      (serviceName == "WAON" && cfg.WAON != 0) ||
                                      (serviceName == "iD" && cfg.iD != 0) ||
                                      (serviceName == "QUICPay" && cfg.QUICPay != 0);
                return isConfigedTest;
            }
        }

        public static Tuple<Utilities.ErrorType, string> Fake_SubtractValue(string serviceName, string amount)
        {
            Utilities.DeleteConvertedReceiptFile();
            Utilities.Log.Info("Fake_SubtractValue start.");

            int sequenceNumber = int.Parse(Utilities.GetSettlementSequenceNumber());
            string fakeResponseCode = "0140";
            Utilities.Log.Info($"-> ResponseCode={fakeResponseCode}, SequenceNumber={sequenceNumber}");

            // Save data for maintenance mode
            GlobalData.PreCurrentServiceSV = serviceName;
            GlobalData.PreSettledAmountSV = decimal.Parse(amount);
            GlobalData.PreStatementIDSV = "99999999999";
            GlobalData.PreSequenceNumberSV = sequenceNumber;

            Utilities.Log.Info($"PreCurrentServiceSV: {GlobalData.PreCurrentServiceSV}, PreSettledAmountSV: {GlobalData.PreSettledAmountSV}" +
                          $", PreStatementIDSV: {GlobalData.PreStatementIDSV}, PreSequenceNumberSV: {GlobalData.PreSequenceNumberSV}");

            Tuple<Utilities.ErrorType, string> ret;
            ret = Utilities.GetErrorTypeAndMsg("ResponseCode", fakeResponseCode);

            bool isPrintRetryNeeded = false;
            if (fakeResponseCode == "0140")
            {
                // Fake_PrintRetry() will re-write the ret value
                isPrintRetryNeeded = true;
            }
            else if (fakeResponseCode == "0000")
            {
                // Save receipt data for printing later (after clerk login)
                Fake_SaveAllReceipt(serviceName);

                // Save unprocessed/alarm receipt (to convert later if clerk defeats success)
                PaymentAPI.UnprocessedReceipt = new List<PaymentAPI.RowData>(PaymentAPI.TempReceiptData[PaymentAPI.RECEIPT_TYPE.RT_MERCHANT]);
                PaymentAPI.SeqNumOfUnprocessedReceipt = sequenceNumber;
                PaymentAPI.TimeOfUnprocessedReceipt = DateTime.Now;
            }
            Utilities.Log.Info("Return of Fake_SubtractValue: " + ret.Item1 + " " + ret.Item2);

            // Unreturned response data proccessing
            if (isPrintRetryNeeded)
            {
                ret = Fake_PrintRetry(serviceName, sequenceNumber);
            }

            return ret;
        }

        public static Tuple<Utilities.ErrorType, string> Fake_PrintRetry(string fakeCurrentService, int fakeSequenceNumber)
        {
            Utilities.Log.Info("Fake_PrintRetry start.");

            // Fake the the result property
            string fakeResponseCode = "0000";
            string fakeErrorcodedetail = "TFXX-803";
            if (fakeCurrentService == "Suica") fakeErrorcodedetail = fakeErrorcodedetail.Replace("XX", "05");
            else if (fakeCurrentService == "nanaco") fakeErrorcodedetail = fakeErrorcodedetail.Replace("XX", "07");
            else if (fakeCurrentService == "Edy") fakeErrorcodedetail = fakeErrorcodedetail.Replace("XX", "02");
            else if (fakeCurrentService == "WAON") fakeErrorcodedetail = fakeErrorcodedetail.Replace("XX", "06");
            else if (fakeCurrentService == "iD") fakeErrorcodedetail = fakeErrorcodedetail.Replace("XX", "03");
            else if (fakeCurrentService == "QUICPay") fakeErrorcodedetail = fakeErrorcodedetail.Replace("XX", "04");
            string fakeMessage = "[E803] 処理未了エラーが発生しました。\n所定の操作を行ってください。";
            Utilities.Log.Info($"-> ResponseCode={fakeResponseCode}, Errorcodedetail={fakeErrorcodedetail}, SequenceNumber={fakeSequenceNumber}");

            // Save SequenceNumber for checking later
            GlobalData.SequenceNumberPR = fakeSequenceNumber;

            Tuple<Utilities.ErrorType, string> ret;
            if (fakeResponseCode == "0000")
            {
                ret = Utilities.GetErrorTypeAndMsg("Errorcodedetail", fakeErrorcodedetail, fakeCurrentService, fakeMessage, GlobalData.LastOperator);

                // Get method code
                string methodCode = fakeSequenceNumber.ToString().Substring(0, 2);

                // Save unprocessed/alarm receipt (to convert later if clerk defeats success)
                if (methodCode == "30" && ret.Item1 == Utilities.ErrorType.ProcessingNotCompleted) // SubtractValue("30")
                {
                    Fake_SaveAllReceipt(fakeCurrentService);

                    GlobalData.PreCurrentServiceSV = fakeCurrentService;
                    GlobalData.PreStatementIDSV = "99999999999";
                    GlobalData.PreSequenceNumberSV = fakeSequenceNumber;

                    Utilities.Log.Info($"PreCurrentServiceSV: {GlobalData.PreCurrentServiceSV}, PreSettledAmountSV: {GlobalData.PreSettledAmountSV}" +
                                  $", PreStatementIDSV: {GlobalData.PreStatementIDSV}, PreSequenceNumberSV: {GlobalData.PreSequenceNumberSV}");

                    PaymentAPI.UnprocessedReceipt = new List<PaymentAPI.RowData>(PaymentAPI.TempReceiptData[PaymentAPI.RECEIPT_TYPE.RT_MERCHANT]);
                    PaymentAPI.SeqNumOfUnprocessedReceipt = fakeSequenceNumber;
                    PaymentAPI.TimeOfUnprocessedReceipt = DateTime.Now;
                }
            }
            else
            {
                ret = Utilities.GetErrorTypeAndMsg("ResponseCode", fakeResponseCode);
            }

            Utilities.Log.Info("Return of Fake_PrintRetry: " + ret.Item1 + " " + ret.Item2);
            return ret;
        }

        public static void Fake_SaveAllReceipt(string fakeCurrentService)
        {
            dynamic receiptData = "";
            if (fakeCurrentService == "Suica")
            {
                receiptData = SuicaUnprocessedReceipt.MerchantReceipt;
            }
            else if (fakeCurrentService == "nanaco")
            {
                receiptData = nanacoUnprocessedReceipt.MerchantReceipt;
            }
            else if (fakeCurrentService == "Edy")
            {
                receiptData = EdyUnprocessedReceipt.MerchantReceipt;
            }
            else if (fakeCurrentService == "WAON")
            {
                receiptData = WAONUnprocessedReceipt.MerchantReceipt;
            }
            else if (fakeCurrentService == "iD")
            {
                receiptData = iDUnprocessedReceipt.MerchantReceipt;
            }
            else if (fakeCurrentService == "QUICPay")
            {
                receiptData = QUICPayUnprocessedReceipt.MerchantReceipt;
            }

            // Clear all data in TempReceiptData
            Utilities.ClearTempReceiptData();

            for (int i = 0; i < receiptData.Length; i++)
            {
                PaymentAPI.TempReceiptData[PaymentAPI.RECEIPT_TYPE.RT_MERCHANT].Add(new PaymentAPI.RowData
                {
                    RowId = i + 1,
                    Attr = receiptData[i].attr,
                    Label = receiptData[i].label,
                    Value = receiptData[i].value
                });
            }
        }

        public class ConfigTest
        {
            public int test_mode { get; set; }
            public int Suica { get; set; }
            public int nanaco { get; set; }
            public int Edy { get; set; }
            public int WAON { get; set; }
            public int iD { get; set; }
            public int QUICPay { get; set; }
        }
        public class Suica
        {
            public FileStruct.RowData[] MerchantReceipt { get; set; }
        }
        public class nanaco
        {
            public FileStruct.RowData[] MerchantReceipt { get; set; }
        }
        public class Edy
        {
            public FileStruct.RowData[] MerchantReceipt { get; set; }
        }
        public class WAON
        {
            public FileStruct.RowData[] MerchantReceipt { get; set; }
        }
        public class iD
        {
            public FileStruct.RowData[] MerchantReceipt { get; set; }
        }
        public class QUICPay
        {
            public FileStruct.RowData[] MerchantReceipt { get; set; }
        }
    }
}
