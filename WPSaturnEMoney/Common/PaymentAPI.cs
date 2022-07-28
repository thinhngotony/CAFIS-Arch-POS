using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AxOposCAT_VescaCO;
using WPSaturnEMoney.Models;
using Vjp.Saturn1000LaneIF.Main;
using static WPSaturnEMoney.Models.FileStruct;
using Newtonsoft.Json;

namespace WPSaturnEMoney.Common
{
    public class PaymentAPI
    {
        public static AxOPOSCAT_VESCA AxOp;

        // true: OutputCompleteEvent occurred, false: ErrorEvent occurred,
        // null: not receive any event after a timeout
        public static bool? IsCompleteEventReceived;

        // Last time occur an event (for saving journal file)
        public static DateTime LastTimeEventOccur;

        private const int _startServiceTimeout = 30; // second
        private const int _subtractValueTimeout = 120; // second
        private const int _readValueTimeout = 120; // second
        private const int _cardHistoryTimeout = 120; // second
        private const int _accessDailyLogTimeout = 120; // second
        private const int _printRetryTimeout = 30; // second

        // Processing Not Completed receipt info
        public static List<RowData> UnprocessedReceipt = new List<RowData>();
        public static int SeqNumOfUnprocessedReceipt;
        public static DateTime TimeOfUnprocessedReceipt;

        // Temporary receipt data to print, clear data after printing
        public static Dictionary<RECEIPT_TYPE, List<RowData>> TempReceiptData = new Dictionary<RECEIPT_TYPE, List<RowData>>
        {
            {RECEIPT_TYPE.RT_CUSTOMER, new List<RowData>() },
            {RECEIPT_TYPE.RT_MERCHANT, new List<RowData>() },
            {RECEIPT_TYPE.RT_CARD_COMPANY, new List<RowData>() },
            {RECEIPT_TYPE.RT_TENANT, new List<RowData>() }
        };

        private static Tuple<Utilities.ErrorType, string> Initialize()
        {
            IsCompleteEventReceived = null;

            int openSts = AxOp.Open("Vesca.CAT.001");
            Utilities.Log.Info("openSts = " + (ApiSts)openSts + " (" + openSts + ")");
            if (openSts == (int)ApiSts.OPOS_SUCCESS)
            {
                // Not set AxOp.ComPort here because using default setting in the register
                int claimDeviceSts = AxOp.ClaimDevice(-1);
                Utilities.Log.Info("claimDeviceSts = " + (ApiSts)claimDeviceSts + " (" + claimDeviceSts + "), " +
                              "ComPort: " + AxOp.ComPort + " [HKEY_CURRENT_USER/Software/Vesca/Config/Common]");
                if (claimDeviceSts == (int)ApiSts.OPOS_SUCCESS)
                {
                    return new Tuple<Utilities.ErrorType, string>(Utilities.ErrorType.Success, "");
                }
                else
                {
                    return Utilities.GetErrorTypeAndMsg("application", "AE5");
                }
            }
            else
            {
                return Utilities.GetErrorTypeAndMsg("application", "AE5");
            }
        }

        private static void Terminate()
        {
            int releaseDeviceSts = AxOp.ReleaseDevice();
            Utilities.Log.Info("releaseDeviceSts = " + (ApiSts)releaseDeviceSts + " (" + releaseDeviceSts + ")");
            int closeSts = AxOp.Close();
            Utilities.Log.Info("closeSts = " + (ApiSts)closeSts + " (" + closeSts + ")");
        }

        private async static Task WaitDoneAPI(int timeout_s)
        {
            Utilities.Log.Info("Waiting API Event...");
            int i;
            int iMax = timeout_s * 10;
            for (i = 0; i < iMax; i++)
            {
                // Cancel waiting when the event comes (IsCompleteEventReceived not null)
                if (IsCompleteEventReceived != null)
                {
                    Utilities.Log.Info($"Received API Event after {i / 10}s");
                    break;
                }
                await Task.Delay(100);
            }
        }

        public async static Task<Tuple<Utilities.ErrorType, string>> CheckConnection()
        {
            Utilities.Log.Info("CheckConnection start.");

            var openError = Initialize();
            if (openError.Item1 != Utilities.ErrorType.Success)
            {
                Terminate();
                Utilities.Log.Error("Initialize error: " + openError.Item1 + " " + openError.Item2);
                return openError;
            }

            Tuple<Utilities.ErrorType, string> ret;
            try
            {
                int startServiceSts = AxOp.StartService();
                Utilities.Log.Info("startServiceSts = " + (ApiSts)startServiceSts + " (" + startServiceSts + ")");
                if (startServiceSts != (int)ApiSts.OPOS_SUCCESS)
                {
                    Utilities.Log.Info("ResponseCode=" + AxOp.ResponseCode);
                    if (startServiceSts == (int)ApiSts.OPOS_E_EXTENDED)
                    {
                        ret = Utilities.GetErrorTypeAndMsg("ResponseCode", AxOp.ResponseCode);
                    }
                    else
                    {
                        ret = Utilities.GetErrorTypeAndMsg("result", startServiceSts.ToString());
                    }
                    Utilities.Log.Info("StartService return: " + ret.Item1 + " " + ret.Item2);
                }
                else
                {
                    await WaitDoneAPI(_startServiceTimeout);

                    if (IsCompleteEventReceived == true)
                    {
                        // OutputCompleteEvent occurred
                        ret = new Tuple<Utilities.ErrorType, string>(Utilities.ErrorType.Success, "");
                        Utilities.Log.Info("End of OutputCompleteEvent: " + ret.Item1);
                    }
                    else if (IsCompleteEventReceived == false)
                    {
                        // ErrorEvent occurred, get Errorcodedetail and Message when ResponseCode = "0000"
                        var type = Utilities.ErrorType.POSTerminalCommunication;
                        string msg;
                        if (AxOp.ResponseCode == "0000")
                        {
                            msg = $"[{AxOp.Errorcodedetail}] {AxOp.Message}";
                        }
                        else
                        {
                            msg = Utilities.GetErrorTypeAndMsg("ResponseCode", AxOp.ResponseCode).Item2;
                        }
                        ret = new Tuple<Utilities.ErrorType, string>(type, msg);
                        Utilities.Log.Info("End of ErrorEvent: " + ret.Item1 + " " + ret.Item2);
                    }
                    else
                    {
                        // Timeout occurred, return error "default" of "ResponseCode" key (or custom a new one)
                        ret = Utilities.GetErrorTypeAndMsg("ResponseCode", "TO");
                        Utilities.Log.Info("End of WaitEvent (timeout): " + ret.Item1 + " " + ret.Item2);
                    }
                }
            }
            finally
            {
                Terminate();
            }

            return ret;
        }

        public async static Task<Tuple<Utilities.ErrorType, string>> EMoney_SubtractValue(string serviceName, string amount)
        {
            Utilities.DeleteConvertedReceiptFile();
            Utilities.Log.Info("EMoney_SubtractValue start.");

            var openError = Initialize();
            if (openError.Item1 != Utilities.ErrorType.Success)
            {
                Terminate();
                Utilities.Log.Error("Initialize error: " + openError.Item1 + " " + openError.Item2);
                return openError;
            }

            Tuple<Utilities.ErrorType, string> ret;
            int sequenceNumber = int.Parse(Utilities.GetSettlementSequenceNumber());
            bool isPrintRetryNeeded = false;
            try
            {
                int timeout = -1; // timeout OPOS_FOREVER(-1): VescaOCX is unused
                AxOp.TrainingMode = false;
                AxOp.SetParameterInformation("ServiceName", serviceName);
                AxOp.SetParameterInformation("Amount", amount);
                int subtractValueSts = AxOp.SubtractValue(sequenceNumber, timeout);
                Utilities.Log.Info("subtractValueSts = " + (ApiSts)subtractValueSts + " (" + subtractValueSts + ")");

                if (subtractValueSts != (int)ApiSts.OPOS_SUCCESS)
                {
                    Utilities.Log.Info("ResponseCode=" + AxOp.ResponseCode);
                    if (subtractValueSts == (int)ApiSts.OPOS_E_EXTENDED)
                    {
                        ret = Utilities.GetErrorTypeAndMsg("ResponseCode", AxOp.ResponseCode);
                    }
                    else
                    {
                        ret = Utilities.GetErrorTypeAndMsg("result", subtractValueSts.ToString());
                    }
                    Utilities.Log.Info("SubtractValue return: " + ret.Item1 + " " + ret.Item2);
                }
                else
                {
                    await WaitDoneAPI(_subtractValueTimeout);

                    if (IsCompleteEventReceived == true)
                    {
                        // OutputCompleteEvent occurred, acquire sequence number and compare with the sequence number of the SubtractValue request,
                        // and if they are the same, the transaction is considered "successful"
                        if (AxOp.SequenceNumber == sequenceNumber)
                        {
                            ret = new Tuple<Utilities.ErrorType, string>(Utilities.ErrorType.Success, "");
                            GlobalData.CurrentService = AxOp.CurrentService;
                            GlobalData.SettledAmount = AxOp.SettledAmount;
                            AxOp.RetrieveResultInformation("statementID", ref GlobalData.statementID);

                            // Get receipt only for customer
                            GetAllReceiptData(EVENT_TYPE.ET_OUTPUT_COMPLETE, AxOp.SequenceNumber);
                        }
                        else
                        {
                            // Normally does not occur
                            ret = new Tuple<Utilities.ErrorType, string>(Utilities.ErrorType.Others, "OutputCompleteEvent検知、シーケンス番号が異なる。");
                        }
                        Utilities.Log.Info("End of OutputCompleteEvent: " + ret.Item1 + " " + ret.Item2);
                    }
                    else if (IsCompleteEventReceived == false)
                    {
                        // Save data for maintenance mode
                        GlobalData.PreCurrentServiceSV = AxOp.CurrentService;
                        GlobalData.PreSettledAmountSV = decimal.Parse(amount);
                        AxOp.RetrieveResultInformation("statementID", ref GlobalData.PreStatementIDSV);
                        GlobalData.PreSequenceNumberSV = AxOp.SequenceNumber;

                        Utilities.Log.Info($"PreCurrentServiceSV: {GlobalData.PreCurrentServiceSV}, PreSettledAmountSV: {GlobalData.PreSettledAmountSV}" +
                                      $", PreStatementIDSV: {GlobalData.PreStatementIDSV}, PreSequenceNumberSV: {GlobalData.PreSequenceNumberSV}");

                        if (AxOp.ResultCodeExtended == -1 || AxOp.ResultCodeExtended == -2)
                        {
                            // POS or terminal cancellation
                            ret = Utilities.GetErrorTypeAndMsg("ResultCodeExtended", AxOp.ResultCodeExtended.ToString());
                        }
                        else if (AxOp.ResponseCode == "0000")
                        {
                            ret = Utilities.GetErrorTypeAndMsg("Errorcodedetail", AxOp.Errorcodedetail, AxOp.CurrentService, AxOp.Message, GlobalData.LastOperator);
                        }
                        else
                        {
                            ret = Utilities.GetErrorTypeAndMsg("ResponseCode", AxOp.ResponseCode);
                        }

                        // Unreturned response data occurr
                        if (AxOp.ResponseCode == "0140")
                        {
                            // PrintRetry() will re-write the ret value
                            isPrintRetryNeeded = true;
                        }
                        else if (AxOp.ResponseCode == "0000")
                        {
                            // Save receipt data for printing later (after clerk login)
                            GetAllReceiptData(EVENT_TYPE.ET_ERROR_EVENT, AxOp.SequenceNumber);

                            // Save unprocessed/alarm receipt (to convert later if clerk defeats success)
                            if (ret.Item1 == Utilities.ErrorType.ProcessingNotCompleted)
                            {
                                UnprocessedReceipt = new List<RowData>(TempReceiptData[RECEIPT_TYPE.RT_MERCHANT]);
                                SeqNumOfUnprocessedReceipt = AxOp.SequenceNumber;
                                TimeOfUnprocessedReceipt = DateTime.Now;
                            }
                        }

                        Utilities.Log.Info("End of ErrorEvent: " + ret.Item1 + " " + ret.Item2);
                    }
                    else
                    {
                        // Timeout occurred, return error "default" of "ResponseCode" key (or custom a new one)
                        ret = Utilities.GetErrorTypeAndMsg("ResponseCode", "TO");
                        Utilities.Log.Info("End of WaitEvent (timeout): " + ret.Item1 + " " + ret.Item2);
                    }
                }
            }
            finally
            {
                Terminate();

                // Unreturned response data proccessing
                if (isPrintRetryNeeded)
                {
                    ret = await PrintRetry();
                }
            }

            return ret;
        }

        public async static Task<Tuple<Utilities.ErrorType, string>> EMoney_ReadValue(string serviceName)
        {
            Utilities.DeleteConvertedReceiptFile();
            Utilities.Log.Info("EMoney_ReadValue start.");

            var openError = Initialize();
            if (openError.Item1 != Utilities.ErrorType.Success)
            {
                Terminate();
                Utilities.Log.Error("Initialize error: " + openError.Item1 + " " + openError.Item2);
                return openError;
            }

            Tuple<Utilities.ErrorType, string> ret;
            int sequenceNumber = int.Parse(Utilities.GetSettlementSequenceNumber());
            try
            {
                int timeout = -1; // timeout OPOS_FOREVER(-1): VescaOCX is unused
                AxOp.TrainingMode = false;
                AxOp.SetParameterInformation("ServiceName", serviceName);
                int readValueSts = AxOp.ReadValue(sequenceNumber, timeout);
                Utilities.Log.Info("readValueSts = " + (ApiSts)readValueSts + " (" + readValueSts + ")");

                if (readValueSts != (int)ApiSts.OPOS_SUCCESS)
                {
                    Utilities.Log.Info("ResponseCode=" + AxOp.ResponseCode);
                    if (readValueSts == (int)ApiSts.OPOS_E_EXTENDED)
                    {
                        ret = Utilities.GetErrorTypeAndMsg("ResponseCode", AxOp.ResponseCode);
                    }
                    else
                    {
                        ret = Utilities.GetErrorTypeAndMsg("result", readValueSts.ToString());
                    }
                    Utilities.Log.Info("ReadValue return: " + ret.Item1 + " " + ret.Item2);
                }
                else
                {
                    await WaitDoneAPI(_readValueTimeout);

                    if (IsCompleteEventReceived == true)
                    {
                        // OutputCompleteEvent occurred, acquire sequence number and compare with the sequence number of the ReadValue request,
                        // and if they are the same, the ReadValue is considered "successful"
                        if (AxOp.SequenceNumber == sequenceNumber)
                        {
                            ret = new Tuple<Utilities.ErrorType, string>(Utilities.ErrorType.Success, "");
                            GlobalData.Balance = AxOp.Balance;
                            GlobalData.CurrentService = AxOp.CurrentService;

                            // Get receipt (if any)
                            GetAllReceiptData(EVENT_TYPE.ET_OUTPUT_COMPLETE, AxOp.SequenceNumber);
                        }
                        else
                        {
                            // Normally does not occur
                            ret = new Tuple<Utilities.ErrorType, string>(Utilities.ErrorType.Others, "OutputCompleteEvent検知、シーケンス番号が異なる。");
                        }
                        Utilities.Log.Info("End of OutputCompleteEvent: " + ret.Item1 + " " + ret.Item2);
                    }
                    else if (IsCompleteEventReceived == false)
                    {
                        if (AxOp.ResultCodeExtended == -1 || AxOp.ResultCodeExtended == -2)
                        {
                            // POS or terminal cancellation
                            ret = Utilities.GetErrorTypeAndMsg("ResultCodeExtended", AxOp.ResultCodeExtended.ToString());
                        }
                        else if (AxOp.ResponseCode == "0000")
                        {
                            ret = Utilities.GetErrorTypeAndMsg("Errorcodedetail", AxOp.Errorcodedetail, AxOp.CurrentService, AxOp.Message, GlobalData.LastOperator);
                        }
                        else
                        {
                            ret = Utilities.GetErrorTypeAndMsg("ResponseCode", AxOp.ResponseCode);
                        }

                        GetAllReceiptData(EVENT_TYPE.ET_ERROR_EVENT, AxOp.SequenceNumber);

                        Utilities.Log.Info("End of ErrorEvent: " + ret.Item1 + " " + ret.Item2);
                    }
                    else
                    {
                        // Timeout occurred, return error "default" of "ResponseCode" key (or custom a new one)
                        ret = Utilities.GetErrorTypeAndMsg("ResponseCode", "TO");
                        Utilities.Log.Info("End of WaitEvent (timeout): " + ret.Item1 + " " + ret.Item2);
                    }
                }
            }
            finally
            {
                Terminate();
            }

            return ret;
        }

        public async static Task<Tuple<Utilities.ErrorType, string>> EMoney_CardHistory(string serviceName)
        {
            Utilities.DeleteConvertedReceiptFile();
            Utilities.Log.Info("EMoney_CardHistory start.");

            var openError = Initialize();
            if (openError.Item1 != Utilities.ErrorType.Success)
            {
                Terminate();
                Utilities.Log.Error("Initialize error: " + openError.Item1 + " " + openError.Item2);
                return openError;
            }

            Tuple<Utilities.ErrorType, string> ret;
            int sequenceNumber = int.Parse(Utilities.GetSettlementSequenceNumber());
            try
            {
                int timeout = -1; // timeout OPOS_FOREVER(-1): VescaOCX is unused
                AxOp.TrainingMode = false;
                AxOp.SetParameterInformation("ServiceName", serviceName);
                int cardHistorySts = AxOp.CardHistory(sequenceNumber, timeout);
                Utilities.Log.Info("cardHistorySts = " + (ApiSts)cardHistorySts + " (" + cardHistorySts + ")");

                if (cardHistorySts != (int)ApiSts.OPOS_SUCCESS)
                {
                    Utilities.Log.Info("ResponseCode=" + AxOp.ResponseCode);
                    if (cardHistorySts == (int)ApiSts.OPOS_E_EXTENDED)
                    {
                        ret = Utilities.GetErrorTypeAndMsg("ResponseCode", AxOp.ResponseCode);
                    }
                    else
                    {
                        ret = Utilities.GetErrorTypeAndMsg("result", cardHistorySts.ToString());
                    }
                    Utilities.Log.Info("CardHistory return: " + ret.Item1 + " " + ret.Item2);
                }
                else
                {
                    await WaitDoneAPI(_cardHistoryTimeout);

                    if (IsCompleteEventReceived == true)
                    {
                        // OutputCompleteEvent occurred, acquire sequence number and compare with the sequence number of the CardHistory request,
                        // and if they are the same, the CardHistory is considered "successful"
                        if (AxOp.SequenceNumber == sequenceNumber)
                        {
                            ret = new Tuple<Utilities.ErrorType, string>(Utilities.ErrorType.Success, "");

                            // Get receipt (if any)
                            GetAllReceiptData(EVENT_TYPE.ET_OUTPUT_COMPLETE, AxOp.SequenceNumber);
                        }
                        else
                        {
                            // Normally does not occur
                            ret = new Tuple<Utilities.ErrorType, string>(Utilities.ErrorType.Others, "OutputCompleteEvent検知、シーケンス番号が異なる。");
                        }
                        Utilities.Log.Info("End of OutputCompleteEvent: " + ret.Item1 + " " + ret.Item2);
                    }
                    else if (IsCompleteEventReceived == false)
                    {
                        if (AxOp.ResultCodeExtended == -1 || AxOp.ResultCodeExtended == -2)
                        {
                            // POS or terminal cancellation
                            ret = Utilities.GetErrorTypeAndMsg("ResultCodeExtended", AxOp.ResultCodeExtended.ToString());
                        }
                        else if (AxOp.ResponseCode == "0000")
                        {
                            ret = Utilities.GetErrorTypeAndMsg("Errorcodedetail", AxOp.Errorcodedetail, AxOp.CurrentService, AxOp.Message, GlobalData.LastOperator);
                        }
                        else
                        {
                            ret = Utilities.GetErrorTypeAndMsg("ResponseCode", AxOp.ResponseCode);
                        }

                        // Get receipt (if any)
                        GetAllReceiptData(EVENT_TYPE.ET_ERROR_EVENT, AxOp.SequenceNumber);

                        Utilities.Log.Info("End of ErrorEvent: " + ret.Item1 + " " + ret.Item2);
                    }
                    else
                    {
                        // Timeout occurred, return error "default" of "ResponseCode" key (or custom a new one)
                        ret = Utilities.GetErrorTypeAndMsg("ResponseCode", "TO");
                        Utilities.Log.Info("End of WaitEvent (timeout): " + ret.Item1 + " " + ret.Item2);
                    }
                }
            }
            finally
            {
                Terminate();
            }

            return ret;
        }
        public async static Task<Tuple<Utilities.ErrorType, string>> EMoney_AccessDailyLog()
        {
            Utilities.DeleteConvertedReceiptFile();
            Utilities.Log.Info("EMoney_AccessDailyLog start.");

            var openError = Initialize();
            if (openError.Item1 != Utilities.ErrorType.Success)
            {
                Terminate();
                Utilities.Log.Error("Initialize error: " + openError.Item1 + " " + openError.Item2);
                return openError;
            }

            Tuple<Utilities.ErrorType, string> ret;
            int sequenceNumber = int.Parse(Utilities.GetSettlementSequenceNumber());
            try
            {
                int timeout = -1; // timeout OPOS_FOREVER(-1): VescaOCX is unused
                int sumType = (int)OPOSCATConstants.CAT_DL_SETTLEMENT;
                AxOp.PaymentMedia = (int)OPOSCATConstants.CAT_MEDIA_NONDEFINE;
                AxOp.TrainingMode = false;
                AxOp.SetParameterInformation("Type", "1"); // Type: "1" Summary, "2" Detail
                int accessDailyLogSts = AxOp.AccessDailyLog(sequenceNumber, sumType, timeout);
                Utilities.Log.Info("accessDailyLogSts = " + (ApiSts)accessDailyLogSts + " (" + accessDailyLogSts + ")");

                if (accessDailyLogSts != (int)ApiSts.OPOS_SUCCESS)
                {
                    Utilities.Log.Info("ResponseCode=" + AxOp.ResponseCode);
                    if (accessDailyLogSts == (int)ApiSts.OPOS_E_EXTENDED)
                    {
                        ret = Utilities.GetErrorTypeAndMsg("ResponseCode", AxOp.ResponseCode);
                    }
                    else
                    {
                        ret = Utilities.GetErrorTypeAndMsg("result", accessDailyLogSts.ToString());
                    }
                    Utilities.Log.Info("AccessDailyLog return: " + ret.Item1 + " " + ret.Item2);
                }
                else
                {
                    await WaitDoneAPI(_accessDailyLogTimeout);

                    if (IsCompleteEventReceived == true)
                    {
                        // OutputCompleteEvent occurred, acquire sequence number and compare with the sequence number of the AccessDailyLog request,
                        // and if they are the same, the AccessDailyLog is considered "successful"
                        if (AxOp.SequenceNumber == sequenceNumber)
                        {
                            ret = new Tuple<Utilities.ErrorType, string>(Utilities.ErrorType.Success, "");

                            // Get daily total receipt (Vesca OCX management output + POS management output)
                            GetDailyTotalReceiptData(AxOp.SequenceNumber);
                        }
                        else
                        {
                            // Normally does not occur
                            ret = new Tuple<Utilities.ErrorType, string>(Utilities.ErrorType.Others, "OutputCompleteEvent検知、シーケンス番号が異なる。");
                        }
                        Utilities.Log.Info("End of OutputCompleteEvent: " + ret.Item1 + " " + ret.Item2);
                    }
                    else if (IsCompleteEventReceived == false)
                    {
                        if (AxOp.ResultCodeExtended == -1 || AxOp.ResultCodeExtended == -2)
                        {
                            // POS or terminal cancellation
                            ret = Utilities.GetErrorTypeAndMsg("ResultCodeExtended", AxOp.ResultCodeExtended.ToString());
                        }
                        else if (AxOp.ResponseCode == "0000")
                        {
                            ret = Utilities.GetErrorTypeAndMsg("Errorcodedetail", AxOp.Errorcodedetail, AxOp.CurrentService, AxOp.Message, GlobalData.LastOperator);
                        }
                        else
                        {
                            ret = Utilities.GetErrorTypeAndMsg("ResponseCode", AxOp.ResponseCode);
                        }

                        // Get receipt (if any)
                        GetAllReceiptData(EVENT_TYPE.ET_ERROR_EVENT, AxOp.SequenceNumber);

                        Utilities.Log.Info("End of ErrorEvent: " + ret.Item1 + " " + ret.Item2);
                    }
                    else
                    {
                        // Timeout occurred, return error "default" of "ResponseCode" key (or custom a new one)
                        ret = Utilities.GetErrorTypeAndMsg("ResponseCode", "TO");
                        Utilities.Log.Info("End of WaitEvent (timeout): " + ret.Item1 + " " + ret.Item2);
                    }
                }
            }
            finally
            {
                Terminate();
            }

            return ret;
        }


        public async static Task<Tuple<Utilities.ErrorType, string>> PrintRetry()
        {
            Utilities.Log.Info("PrintRetry start.");

            var openError = Initialize();
            if (openError.Item1 != Utilities.ErrorType.Success)
            {
                Terminate();
                Utilities.Log.Error("Initialize error: " + openError.Item1 + " " + openError.Item2);
                return openError;
            }

            Tuple<Utilities.ErrorType, string> ret;
            int sequenceNumber = int.Parse(Utilities.GetSettlementSequenceNumber());
            try
            {
                AxOp.PaymentMedia = (int)OPOSCATConstants.CAT_MEDIA_NONDEFINE;
                int printRetrySts = AxOp.PrintRetry(sequenceNumber);
                Utilities.Log.Info("printRetrySts = " + (ApiSts)printRetrySts + " (" + printRetrySts + ")");

                if (printRetrySts != (int)ApiSts.OPOS_SUCCESS)
                {
                    if (printRetrySts == (int)ApiSts.OPOS_E_EXTENDED)
                    {
                        ret = Utilities.GetErrorTypeAndMsg("ResponseCode", AxOp.ResponseCode);
                    }
                    else
                    {
                        ret = Utilities.GetErrorTypeAndMsg("result", printRetrySts.ToString());
                    }
                    Utilities.Log.Info("PrintRetry return: " + ret.Item1 + " " + ret.Item2);
                }
                else
                {
                    await WaitDoneAPI(_printRetryTimeout);

                    if (IsCompleteEventReceived == true)
                    {
                        // Save SequenceNumber for checking later
                        GlobalData.SequenceNumberPR = AxOp.SequenceNumber;

                        ret = new Tuple<Utilities.ErrorType, string>(Utilities.ErrorType.Success, "");

                        // Get method code
                        string methodCode = "";
                        if (AxOp.SequenceNumber.ToString().Length == 9) methodCode = AxOp.SequenceNumber.ToString().Substring(0, 2);

                        // SubtractValue("30")
                        if (methodCode == "30")
                        {
                            GlobalData.CurrentService = AxOp.CurrentService;
                            GlobalData.SettledAmount = AxOp.SettledAmount;
                            AxOp.RetrieveResultInformation("statementID", ref GlobalData.statementID);
                            GetAllReceiptData(EVENT_TYPE.ET_OUTPUT_COMPLETE, AxOp.SequenceNumber);
                        }
                        // ReadValue("34")
                        else if (methodCode == "34")
                        {
                            GlobalData.Balance = AxOp.Balance;
                            GlobalData.CurrentService = AxOp.CurrentService;
                            GetAllReceiptData(EVENT_TYPE.ET_OUTPUT_COMPLETE, AxOp.SequenceNumber);
                        }
                        // AccessDailyLog("11")
                        else if (methodCode == "11")
                        {
                            GetDailyTotalReceiptData(AxOp.SequenceNumber);
                        }
                        // CardHistory("35"), CreditSales("12"), AddValue("31"), CancelValue("32"), CancelValue("33")
                        else if (methodCode == "35" || methodCode == "12" || methodCode == "31" || methodCode == "32" || methodCode == "33")
                        {
                            GetAllReceiptData(EVENT_TYPE.ET_OUTPUT_COMPLETE, AxOp.SequenceNumber);
                        }
                        // Other than electronic money
                        else
                        {
                            ret = Utilities.GetErrorTypeAndMsg("application", "AE11");
                        }

                        Utilities.Log.Info("End of OutputCompleteEvent: " + ret.Item1 + " " + ret.Item2);
                    }
                    else if (IsCompleteEventReceived == false)
                    {
                        // Save SequenceNumber for checking later
                        GlobalData.SequenceNumberPR = AxOp.SequenceNumber;

                        if (AxOp.ResultCodeExtended == -1 || AxOp.ResultCodeExtended == -2)
                        {
                            // POS or terminal cancellation
                            ret = Utilities.GetErrorTypeAndMsg("ResultCodeExtended", AxOp.ResultCodeExtended.ToString());
                        }
                        else if (AxOp.ResponseCode == "0000")
                        {
                            ret = Utilities.GetErrorTypeAndMsg("Errorcodedetail", AxOp.Errorcodedetail, AxOp.CurrentService, AxOp.Message, GlobalData.LastOperator);

                            // Get method code
                            string methodCode = "";
                            if (AxOp.SequenceNumber.ToString().Length == 9) methodCode = AxOp.SequenceNumber.ToString().Substring(0, 2);

                            // Save data for maintenance mode and unprocessed/alarm receipt (to convert later if clerk defeats success)
                            // when SubtractValue processing not completed
                            if (methodCode == "30" && ret.Item1 == Utilities.ErrorType.ProcessingNotCompleted)
                            {
                                GetAllReceiptData(EVENT_TYPE.ET_ERROR_EVENT, AxOp.SequenceNumber);

                                GlobalData.PreCurrentServiceSV = AxOp.CurrentService;
                                // GlobalData.PreSettledAmountSV got from SubtractValue ErrorEvent
                                AxOp.RetrieveResultInformation("statementID", ref GlobalData.PreStatementIDSV);
                                GlobalData.PreSequenceNumberSV = AxOp.SequenceNumber;

                                Utilities.Log.Info($"PreCurrentServiceSV: {GlobalData.PreCurrentServiceSV}, PreSettledAmountSV: {GlobalData.PreSettledAmountSV}" +
                                              $", PreStatementIDSV: {GlobalData.PreStatementIDSV}, PreSequenceNumberSV: {GlobalData.PreSequenceNumberSV}");

                                UnprocessedReceipt = new List<RowData>(TempReceiptData[RECEIPT_TYPE.RT_MERCHANT]);
                                SeqNumOfUnprocessedReceipt = AxOp.SequenceNumber;
                                TimeOfUnprocessedReceipt = DateTime.Now;
                            }
                            else if (methodCode == "30" || methodCode == "34" || methodCode == "35" || methodCode == "11" ||
                                     methodCode == "12" || methodCode == "31" || methodCode == "32" || methodCode == "33")
                            {
                                GetAllReceiptData(EVENT_TYPE.ET_ERROR_EVENT, AxOp.SequenceNumber);
                            }
                            else
                            {
                                ret = Utilities.GetErrorTypeAndMsg("application", "AE11");
                            }
                        }
                        else
                        {
                            ret = Utilities.GetErrorTypeAndMsg("ResponseCode", AxOp.ResponseCode);
                        }

                        Utilities.Log.Info("End of ErrorEvent: " + ret.Item1 + " " + ret.Item2);
                    }
                    else
                    {
                        // Timeout occurred, return error "default" of "ResponseCode" key (or custom a new one)
                        ret = Utilities.GetErrorTypeAndMsg("ResponseCode", "TO");
                        Utilities.Log.Info("End of WaitEvent (timeout): " + ret.Item1 + " " + ret.Item2);
                    }
                }
            }
            finally
            {
                Terminate();
            }
            return ret;
        }

        public static void GetAllReceiptData(EVENT_TYPE event_type, int sequenceNumber)
        {
            // Clear all data in TempReceiptData
            Utilities.ClearTempReceiptData();

            GetReceiptData(RECEIPT_TYPE.RT_CUSTOMER, event_type);
            GetReceiptData(RECEIPT_TYPE.RT_MERCHANT, event_type);
            GetReceiptData(RECEIPT_TYPE.RT_CARD_COMPANY, event_type);
            GetReceiptData(RECEIPT_TYPE.RT_TENANT, event_type);

            // Save journal file
            Utilities.SaveJournalFile(sequenceNumber, RECEIPT_TYPE.RT_CUSTOMER, TempReceiptData[RECEIPT_TYPE.RT_CUSTOMER]);
            Utilities.SaveJournalFile(sequenceNumber, RECEIPT_TYPE.RT_MERCHANT, TempReceiptData[RECEIPT_TYPE.RT_MERCHANT]);
            Utilities.SaveJournalFile(sequenceNumber, RECEIPT_TYPE.RT_CARD_COMPANY, TempReceiptData[RECEIPT_TYPE.RT_CARD_COMPANY]);
            Utilities.SaveJournalFile(sequenceNumber, RECEIPT_TYPE.RT_TENANT, TempReceiptData[RECEIPT_TYPE.RT_TENANT]);
        }

        public static void GetReceiptData(RECEIPT_TYPE receipt_type, EVENT_TYPE event_type)
        {
            // Clear old data
            TempReceiptData[receipt_type].Clear();

            int eventType = (int)event_type;
            int lines = 0; // number of lines of receipt data

            switch (receipt_type)
            {
                case RECEIPT_TYPE.RT_CUSTOMER: // customer copy
                    lines = AxOp.GetCustomerReceiptCount(eventType);
                    Utilities.Log.Info("お客様控え件数:" + lines);
                    break;
                case RECEIPT_TYPE.RT_MERCHANT: // merchant copy
                    lines = AxOp.GetMerchantReceiptCount(eventType);
                    Utilities.Log.Info("加盟店控え控え件数:" + lines);
                    break;
                case RECEIPT_TYPE.RT_CARD_COMPANY: // card company copy
                    lines = AxOp.GetCreditCompanyReceiptCount(eventType);
                    Utilities.Log.Info("クレジット会社控え件数:" + lines);
                    break;
                case RECEIPT_TYPE.RT_TENANT: // tenant copy
                    lines = AxOp.GetTenantReceiptCount(eventType);
                    Utilities.Log.Info("テナント控え控え件数:" + lines);
                    break;
            }

            for (int i = 1; i <= lines; i++)
            {
                TempReceiptData[receipt_type].Add(new RowData
                {
                    RowId = i,
                    Attr = AxOp.GetReceiptAttr(i),
                    Label = AxOp.GetReceiptLabel(i),
                    Value = AxOp.GetReceiptValue(i)
                });
            }
        }

        public static void GetDailyTotalReceiptData(int sequenceNumber)
        {
            // Clear all data in TempReceiptData
            Utilities.ClearTempReceiptData();

            int lines = AxOp.GetMerchantReceiptCount((int)EVENT_TYPE.ET_OUTPUT_COMPLETE);
            Utilities.Log.Info("加盟店控え控え件数:" + lines);

            for (int i = 1; i <= lines; i++)
            {
                TempReceiptData[RECEIPT_TYPE.RT_MERCHANT].Add(new RowData
                {
                    RowId = i,
                    Attr = AxOp.GetReceiptAttr(i),
                    Label = AxOp.GetReceiptLabel(i),
                    Value = AxOp.GetReceiptValue(i)
                });
            }

            // If the processed sequence number is recorded, and it's different with the current sequence number,
            // it's considered that the daily total was executed without payment, so clear and not use the total info.
            // (if the sequence numbers are the same, it's considered as reprinting, so use the total info to print).
            if (GlobalData.TotalInfo.done_num.Length > 0 && GlobalData.TotalInfo.done_num != AxOp.SequenceNumber.ToString())
            {
                Utilities.ClearTotalInfo();

                // Save journal file
                Utilities.SaveJournalFile(sequenceNumber, RECEIPT_TYPE.RT_MERCHANT, TempReceiptData[RECEIPT_TYPE.RT_MERCHANT]);

                return;
            }

            // Add POS daily total info to the receipt
            List<RowData> POSTotalInfo = CreatePOSDailyTotalReceipt(lines);
            Utilities.Log.Info("POS日計情報印刷データ:" + POSTotalInfo.Count);
            for (int i = 1; i <= POSTotalInfo.Count; i++)
            {
                TempReceiptData[RECEIPT_TYPE.RT_MERCHANT].Add(new RowData
                {
                    RowId = POSTotalInfo[i - 1].RowId,
                    Attr = POSTotalInfo[i - 1].Attr,
                    Label = POSTotalInfo[i - 1].Label,
                    Value = POSTotalInfo[i - 1].Value
                });
            }

            // Record the daily total sequence number
            GlobalData.TotalInfo.done_num = AxOp.SequenceNumber.ToString();
            FileStruct.CsFileWrite(GlobalData.AppPath, GlobalData.DailyTotalPath, GlobalData.TotalInfo);

            // Save journal file
            Utilities.SaveJournalFile(sequenceNumber, RECEIPT_TYPE.RT_MERCHANT, TempReceiptData[RECEIPT_TYPE.RT_MERCHANT]);
        }

        public static List<RowData> CreatePOSDailyTotalReceipt(int preRowId)
        {
            List<RowData> receiptInfo = new List<RowData>
            {
                new RowData { RowId = ++preRowId, Label = "", Value = "=", Attr = "Line" },
                new RowData { RowId = ++preRowId, Label = "", Value = "電子マネー処理未了情報", Attr = "Left" },
                new RowData { RowId = ++preRowId, Label = "", Value = "1", Attr = "Empty" },
                new RowData { RowId = ++preRowId, Label = "取引名", Value = "件数        合計", Attr = "Justify" }
            };

            foreach (var service in GlobalData.TotalInfo.service)
            {
                if (service.Value.SS_CNT + service.Value.NA_CNT == 0) continue;

                receiptInfo.Add(new RowData { RowId = ++preRowId, Label = "", Value = "-", Attr = "Line" });
                receiptInfo.Add(new RowData { RowId = ++preRowId, Label = "", Value = Utilities.GetBrandName(service.Key), Attr = "Left" });

                string count = (service.Value.SS_CNT + service.Value.NA_CNT).ToString();
                string total = string.Format("{0:N0}", service.Value.SS_AMT + service.Value.NA_AMT);
                string val = string.Format("{0,4}{1,12}", count, "¥" + total);
                receiptInfo.Add(new RowData { RowId = ++preRowId, Label = "決済", Value = val, Attr = "Justify" });

                count = service.Value.NA_CNT.ToString();
                total = string.Format("{0:N0}", service.Value.NA_AMT);
                val = string.Format("{0,4}{1,12}", count, "¥" + total);
                receiptInfo.Add(new RowData { RowId = ++preRowId, Label = "処理未了", Value = val, Attr = "Justify" });

                count = service.Value.NASS_CNT.ToString();
                total = string.Format("{0:N0}", service.Value.NASS_AMT);
                val = string.Format("{0,4}{1,12}", count, "¥" + total);
                receiptInfo.Add(new RowData { RowId = ++preRowId, Label = "　引去成功", Value = val, Attr = "Justify" });

                count = (service.Value.NA_CNT - service.Value.NASS_CNT).ToString();
                total = string.Format("{0:N0}", service.Value.NA_AMT - service.Value.NASS_AMT);
                val = string.Format("{0,4}{1,12}", count, "¥" + total);
                receiptInfo.Add(new RowData { RowId = ++preRowId, Label = "　引去失敗", Value = val, Attr = "Justify" });
            }

            receiptInfo.Add(new RowData { RowId = ++preRowId, Label = "", Value = "=", Attr = "Line" });
            receiptInfo.Add(new RowData { RowId = ++preRowId, Label = "", Value = "1", Attr = "Empty" });

            return receiptInfo;
        }

        private enum ApiSts
        {
            OPOS_SUCCESS = 0,
            OPOS_E_CLOSED = 101,
            OPOS_E_CLAIMED = 102,
            OPOS_E_NOTCLAIMED = 103,
            OPOS_E_NOSERVICE = 104,
            OPOS_E_DISABLED = 105,
            OPOS_E_ILLEGAL = 106,
            OPOS_E_NOHARDWARE = 107,
            OPOS_E_OFFLINE = 108,
            OPOS_E_NOEXIST = 109,
            OPOS_E_EXISTS = 110,
            OPOS_E_FAILURE = 111,
            OPOS_E_TIMEOUT = 112,
            OPOS_E_BUSY = 113,
            OPOS_E_EXTENDED = 114,
        }

        public enum RECEIPT_TYPE
        {
            RT_CUSTOMER = 1, // お客様控え
            RT_MERCHANT, // 加盟店控え
            RT_CARD_COMPANY, // カード会社控え
            RT_TENANT, // テナント控え
        }
        public enum EVENT_TYPE
        {
            ET_OUTPUT_COMPLETE = 0, // OutputCompleteEvent
            ET_ERROR_EVENT = 1, // ErrorEvent
        }
        public enum OPOSCATConstants
        {
            CAT_MEDIA_NONDEFINE = 0,
            CAT_DL_REPORTING = 1, // Intermediate total
            CAT_DL_SETTLEMENT = 2 // Daily total
        }
        public class RowData
        {
            [JsonProperty("rowid")]
            public int RowId { get; set; }

            [JsonProperty("attr")]
            public string Attr { get; set; }

            [JsonProperty("label")]
            public string Label { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }
        }
    }
}

