using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WPSaturnEMoney.Models;
using Vjp.Saturn1000LaneIF.Main;
using static WPSaturnEMoney.Models.FileStruct;

namespace WPSaturnEMoney.Common
{
    public class SaturnAPI
    {
        // Interface to Saturn1000 Serial 
        private static Serial SaturnSerial = new Serial();

        private static bool? IsCompleteEventReceived;

        private static SaturnAPIResponse saturnAPIResponse;

        public static string OutputId;                 //  transaction id between POS <-> Arch
        public static string CurrentServiceID;

        // Arch Terminal Response status (Table 4.6-4)  
        public enum SaturnResponsePOSWaitingStatus
        {
            NOT_INIT = 0,                          // Arch terminal not start
            IDLE = 1,                              // Arch terminal idling  
            BUSY = 2,                              // Arch performs transaction 
            MODE_MISMATCH = 3,                     // 
            WAITING_REQUEST_REOPEN_TRANSACTION = 4 //  
        }
        public enum SaturnRequestMode
        {
            Normal = 0,
            Practice = 1,
        }
        public enum SaturnRequestBiz
        {
            Common = 0,
            Normal = 1, // for payment and health check
            LastMinuteTransactionCancel = 3,
            BalanceInquiry = 4,
            LastMinuteTransactionInquiry = 9,
        }
        public enum SaturnRequestService
        {
            HealthCheck = 3000001,
            Cancel = 3000003,
            StatusNotification = 3000004,
        }

        public static void Initialize(bool IsNeedResetOutputID = true)
        {
            Utilities.Log.Info("Initailize Connection to Saturn Serial Port...");
            try
            {
                if (IsNeedResetOutputID) OutputId = null;
                IsCompleteEventReceived = null;
                saturnAPIResponse = null;
                GlobalData.SaturnAPIResponse = null;
                if (!SaturnSerial.isPortConnected)
                {
                    SaturnSerial.PortName = GlobalData.BasicConfig.port_name;
                    SaturnSerial.BauRate = (int)GlobalData.BasicConfig.bau_rate;
                    SaturnSerial.SetParity(GlobalData.BasicConfig.parity_check);
                    SaturnSerial.SetCallback(OnReceiveMsg);
                    SaturnSerial.OpenSerialPort();
                }
            }
            catch (Exception ex)
            {
                Utilities.Log.Error("Initialize SaturnAPI Exception: \n" + ex.ToString());
            }
        }

        public static void Terminate()
        {
            Utilities.Log.Info("Terminate Connection to Saturn Serial Port...");
            try
            {
                if (SaturnSerial.isPortConnected)
                {
                    SaturnSerial.CloseSerialPort();
                }
            }
            catch (Exception ex)
            {
                Utilities.Log.Error("Terminate SaturnAPI Exception: \n" + ex.ToString());
            }
        }

        public static void OnReceiveMsg(string msg)
        {
            try
            {
                if (!string.IsNullOrEmpty(msg))
                {
                    Utilities.Log.Info("Response: " + msg);
                    SaturnAPIResponse msgToJson = JsonConvert.DeserializeObject<SaturnAPIResponse>(msg);

                    saturnAPIResponse = msgToJson;
                    if (saturnAPIResponse.controlInfo.service == ((int)SaturnRequestService.StatusNotification).ToString())
                    {
                        GlobalData.SaturnAPIResponse = saturnAPIResponse;
                    }
                    else if (saturnAPIResponse.controlInfo.service == ((int)SaturnRequestService.HealthCheck).ToString() && CurrentServiceID != ((int)SaturnRequestService.HealthCheck).ToString())
                    {
                        GlobalData.SaturnAPIResponse = saturnAPIResponse;
                        Utilities.Log.Error("Receive health check response!");
                    }
                    // validate outputId & procResult = 0 ? true : false 
                    // Set IsOutputEventComplete to break waiting event timer 
                    else if (msgToJson.controlInfo.outputId == OutputId || CurrentServiceID == ((int)SaturnRequestService.Cancel).ToString())
                    {
                        GlobalData.SaturnAPIResponse = saturnAPIResponse;
                        IsCompleteEventReceived = true;
                    }
                    else
                    {
                        GlobalData.SaturnAPIResponse = null;
                        Utilities.Log.Error("Output ID do not match!");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalData.SaturnAPIResponse = null;
                if (ex is JsonSerializationException || ex is JsonReaderException)
                {
                    Utilities.Log.Error("From Saturn1000LaneIF: " + msg);
                }
                else
                {
                    IsCompleteEventReceived = false;
                    Terminate();
                    Utilities.Log.Error(ex.ToString());
                }
            }
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

        public static async Task<SaturnAPIResponse> HealthCheck()
        {
            try
            {
                Utilities.Log.Info("Health Check start...");

                Initialize();

                // generate outputId & convert json to string 
                var obj = new SaturnAPIRequest
                {
                    mode = "0",
                    service = ((int)SaturnRequestService.HealthCheck).ToString(),
                    biz = ((int)SaturnRequestBiz.Normal).ToString(),
                    logOputputMode = "0",
                    printMode = "0",
                    outputId = Utilities.GenerateOutputID(),
                };
                OutputId = obj.outputId;
                CurrentServiceID = obj.service;
                var request = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Utilities.Log.Info("Health Check Output ID: " + OutputId);

                // Send message
                await Task.Delay(500);
                SaturnSerial.SendMsg(request.ToString());

                // Waiting for complete  
                await WaitDoneAPI((int)GlobalData.BasicConfig.healthcheck_timeout);

                // check after timeout
                if (IsCompleteEventReceived == true)
                    return saturnAPIResponse;

                return null;
            }
            catch (Exception ex)
            {
                Utilities.Log.Error(ex.ToString());
                return null;
            }
        }
        public static async void CancelRequest()
        {
            try
            {
                Utilities.Log.Info("Cancel request start...");

                Initialize(false);

                // generate outputId & convert json to string 
                var obj = new SaturnAPIRequest
                {
                    service = ((int)SaturnRequestService.Cancel).ToString(),
                    biz = ((int)SaturnRequestBiz.Common).ToString(),
                };
                CurrentServiceID = obj.service;
                var request = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                // send message
                await Task.Delay(500);
                SaturnSerial.SendMsg(request.ToString());

                // Waiting for complete  
                // await WaitDoneAPI((int)GlobalData.BasicConfig.settlement_cancel_timeout);

                // check after timeout
                /*if (IsCompleteEventReceived == true)
                    return saturnAPIResponse;

                return null;*/
            }
            catch (Exception ex)
            {
                Utilities.Log.Error(ex.ToString());
                // return null;
            }

        }
        public static async Task<SaturnAPIResponse> sendRequest(SaturnAPIRequest obj)
        {
            try
            {
                Utilities.Log.Info("Start sending request ...");

                Initialize();

                // generate outputId & convert json to string 
                //var obj = new SaturnAPIRequest
                //{
                //    mode = (int)SaturnRequestMode.Normal,
                //    service = (int)SaturnRequestService.HealthCheck,
                //    biz = 1,
                //    logOputputMode = 0
                //};

                OutputId = obj.outputId;
                CurrentServiceID = obj.service;
                var request = JsonConvert.SerializeObject(obj);
                Utilities.Log.Info("Output ID: " + OutputId);

                // send message  
                SaturnSerial.SendMsg(request.ToString());

                // Waiting for complete  
                await WaitDoneAPI((int)GlobalData.BasicConfig.healthcheck_timeout);

                // Close Connection after waiting 
                Terminate();

                // check after timeout             
                if (IsCompleteEventReceived == true)
                    return saturnAPIResponse;

                return null;
            }
            catch (Exception ex)
            {
                Utilities.Log.Error(ex.ToString());
                return null;
            }

        }
        public static void PaymentRequest(string mode, string service)
        {
            try
            {
                Utilities.Log.Info("Payment request start...");

                Initialize();

                // generate outputId & convert json to string 
                var obj = new SaturnAPIRequest
                {
                    mode = mode,
                    service = service,
                    biz = ((int)SaturnRequestBiz.Normal).ToString(),
                    logOputputMode = "0",
                    printMode = "0",
                    outputId = Utilities.GenerateOutputID(),
                    amount = GlobalData.Data_FromPosDat.amount,
                };
                OutputId = obj.outputId;
                CurrentServiceID = obj.service;
                var request = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Utilities.Log.Info("Payment request Output ID: " + OutputId);

                // send message
                SaturnSerial.SendMsg(request.ToString());

                // Waiting for complete  
                // await WaitDoneAPI(GlobalData.BasicConfig.settlement_request_timeout);

                // check after timeout
                /*if (IsCompleteEventReceived == null)
                    Terminate();
                else if (IsCompleteEventReceived == true)
                    return saturnAPIResponse;

                return null;*/
            }
            catch (Exception ex)
            {
                Utilities.Log.Error(ex.ToString());
                // return null;
            }
        }
        public static void BalanceInquiryRequest(string mode, string service)
        {
            try
            {
                Utilities.Log.Info("Balance Inquiry request start...");

                Initialize();

                // generate outputId & convert json to string 
                var obj = new SaturnAPIRequest
                {
                    mode = mode,
                    service = service,
                    biz = ((int)SaturnRequestBiz.BalanceInquiry).ToString(),
                    logOputputMode = "0",
                    printMode = "0",
                    outputId = Utilities.GenerateOutputID(),
                };
                OutputId = obj.outputId;
                CurrentServiceID = obj.service;
                var request = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Utilities.Log.Info("Balance Inquiry request Output ID: " + OutputId);

                // send message
                SaturnSerial.SendMsg(request.ToString());

                // Waiting for complete  
                // await WaitDoneAPI(GlobalData.BasicConfig.settlement_request_timeout);

                // check after timeout
                /*if (IsCompleteEventReceived == null)
                    Terminate();
                else if (IsCompleteEventReceived == true)
                    return saturnAPIResponse;

                return null;*/
            }
            catch (Exception ex)
            {
                Utilities.Log.Error(ex.ToString());
                // return null;
            }
        }
    }
}

