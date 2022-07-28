using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Timers;
using static Vjp.Saturn1000LaneIF.Common.Utilities;

namespace Vjp.Saturn1000LaneIF.Main
{
    public class Serial
    {
        // constant
        private string defPortName = "COM1";
        private int defBaudRate = 19200;
        private string defParity = "null";

        private SerialPort serialPort;
        private State state = State.IDLE;
        private string messageSent = "";
        private Queue<byte[]> messageBlock = new Queue<byte[]>();
        private List<byte> receiveBuffer = new List<byte>();
        private StringBuilder receiveMessage = new StringBuilder();
        private static ErrorType errorType;
        private Timer workingTimer;
        public delegate void Delegate(string message);
        private Delegate callbackFunction;
        public int timeOutOnSend { get; set; } = 5000;
        public int timeOutOnACKWaiting { get; set; } = 5000;
        public int timeOutOnReceive { get; set; } = 5000;
        public int timeOutOnEotReceive { get; set; } = 15000;

        enum State
        {
            IDLE,
            STX_WAITING, // Waiting for STX
            ETX_WAITING, // Waiting for ETX
            BCC_WAITING, // Waiting for BCC
            EOT_WAITING, // Waiting for EOT
            ACK_WAITING  // Waiting for ACK (after sending ETB block / after sending ETX block)
        }

        public Serial()
        {
            string portName = INIDict.TryGetValue("portName", out string name) ? name : defPortName;
            int bauRate = INIDict.TryGetValue("baudRate", out string rate) ? int.Parse(rate) : defBaudRate;
            string tempParity = INIDict.TryGetValue("parity", out string p) ? p : defParity;
            Parity parity = Parity.None;
            if (tempParity != null && tempParity != "null") { parity = (Parity)Enum.Parse(typeof(Parity), tempParity); }
            serialPort = new SerialPort(portName, bauRate, parity, 8, StopBits.One);
            serialPort.DataReceived += new SerialDataReceivedEventHandler(ReceiveMsg);
            serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(ErrorReceived);
            serialPort.WriteTimeout = timeOutOnSend;
        }

        public bool isPortConnected
        {
            get
            {
                return serialPort.IsOpen;
            }
        }

        public string PortName
        {
            get { return serialPort.PortName; }
            set {
                try
                {
                    serialPort.PortName = value;
                    defPortName = value;
                }
                catch (InvalidOperationException e) {
                    callbackFunction("The specified port is occupied.");
                    log.Error(e.Message);
                    throw e;
                }
                catch (Exception e)
                {
                    if (e is ArgumentException || e is ArgumentNullException)
                    {
                        callbackFunction("The port name was not valid.");
                        log.Error(e.Message);
                        throw e;
                    }
                }
            }
        }

        public int BauRate
        {
            get { return serialPort.BaudRate; }
            set {
                try
                {
                    serialPort.BaudRate = value;
                    defBaudRate = value;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    callbackFunction("The baud rate is not valid.");
                    log.Error(e.Message);
                    throw e;
                }
}
        }

        public Parity GetParity()
        {
            return serialPort.Parity;
        }

        public void SetParity(string tempParity)
        {
            try
            {
                Parity parity = Parity.None;
                if (tempParity != null && tempParity != "null") { parity = (Parity)Enum.Parse(typeof(Parity), tempParity); }
                serialPort.Parity = parity;
                defParity = parity.ToString();
            }
            catch (ArgumentOutOfRangeException e)
            {
                callbackFunction("The parity is not valid.");
                log.Error(e.Message);
                throw e;
            }
        }

        public void SetCallback(Delegate cb)
        {
            callbackFunction = cb;
        }

        public void SetSerialPort(string name)
        {
            foreach (string portName in SerialPort.GetPortNames())
            {
                if (portName == name)
                    return;
            }
            errorType = ErrorType.PORTNAME_ERROR;
        }

        public void OpenSerialPort()
        {
            
            if (!serialPort.IsOpen)
            {
                reloadConfig();
                try
                {
                    serialPort.PortName = INIDict.TryGetValue("portName", out string name) ? name : defPortName;
                    serialPort.BaudRate = INIDict.TryGetValue("baudRate", out string rate) ? int.Parse(rate) : defBaudRate;
                    string tempParity = INIDict.TryGetValue("parity", out string p) ? p : defParity;
                    Parity parity = Parity.None;
                    if (tempParity != null && tempParity != "null") { parity = (Parity)Enum.Parse(typeof(Parity), tempParity); }
                    serialPort.Parity = parity;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    if (e.ParamName == "BaudRate")
                    {
                        callbackFunction("The baud rate is not valid.");
                        log.Error(e.Message);
                        throw e;
                    }
                    else if (e.ParamName == "Parity")
                    {
                        callbackFunction("The parity is not valid.");
                        log.Error(e.Message);
                        throw e;
                    }
                }
                catch (Exception e)
                {
                    if (e is ArgumentException || e is ArgumentNullException)
                    {
                        callbackFunction("The port name was not valid.");
                        log.Error(e.Message);
                        throw e;
                    }
                }
                SetSerialPort(serialPort.PortName);
                if (errorType == ErrorType.PORTNAME_ERROR)
                {
                    callbackFunction("Port " + serialPort.PortName + " do not exist!");
                    log.Error("Port " + serialPort.PortName + " do not exist!");
                    errorType = ErrorType.SUCCESS;
                }
                else
                {
                    try
                    {
                        serialPort.Open();
                        state = State.IDLE;
                        callbackFunction("");
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        callbackFunction("Port " + serialPort.PortName + " is not available!");
                        log.Error(e.Message);
                        throw e;
                    }
                    catch (ArgumentException e)
                    {
                        callbackFunction("Port " + serialPort.PortName + " do not begin with \"COM\"!");
                        log.Error(e.Message);
                        throw e;
                    }
                    catch (Exception e)
                    {
                        callbackFunction("An unexpected error occurs when processing!");
                        log.Error(e.Message);
                        throw e;
                    }
                }
            }
        }

        public void CloseSerialPort()
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
            }
            catch (Exception e)
            {
                callbackFunction("An unexpected error occurs when processing!");
                log.Error(e.Message);
                throw e;
            }
        }

        // Send a message
        public void SendMsg(string msg)
        {
            try
            {
                // Send a message if the serial port is open.
                if (serialPort.IsOpen)
                {
                    if (!string.IsNullOrEmpty(msg) && state == State.IDLE)
                    {
                        messageSent = msg;
                        log.Info("Message input: " + messageSent);
                        // Send if the argument is not null or "" and the state is idle
                        // Base64 encoding
                        msg = EncodeBase64(msg);
                        log.Info("Message input: " + messageSent + "\n" + "Message encoded Base 64: " + msg);
                        // Block division
                        messageBlock.Clear();
                        int blocksz = 2048 - 3; // Specified block size-control code (STX, ETX, BCC)
                        int cnt = 1;
                        for (int i = 0; i < msg.Length; i += blocksz)
                        {
                            if (msg.Length > (cnt * blocksz))
                            {
                                // Take out blocks and encode with JIS-8
                                byte[] msgArray = EncodeJIS8(msg.Substring(i, blocksz));
                                // Store with control code
                                messageBlock.Enqueue(CreateMessage(msgArray, false));
                            }
                            else
                            {
                                // Take out blocks and encode with JIS-8
                                byte[] msgArray = EncodeJIS8(msg.Substring(i));
                                // Store with control code
                                messageBlock.Enqueue(CreateMessage(msgArray, true));
                            }
                            cnt++;
                        }
                        // Send a message
                        SendData(messageBlock.Peek(), 0, messageBlock.Peek().Length);
                        log.Info("Message input: " + messageSent + "\n" + "Data string (1st block) sent: " + string.Join("", Array.ConvertAll(messageBlock.Peek(), i => i.ToString())));
                        // Move to ACK wait
                        state = State.ACK_WAITING;
                        ReceiveTimerStart(OnACKWaitingTimeOver, timeOutOnACKWaiting);
                    }
                    // Show message if the argument is null or empty
                    else if (string.IsNullOrEmpty(msg))
                    {
                        callbackFunction("Message is Null or Empty!");
                        log.Error("Message is Null or Empty!");
                    }
                    // Show message if message is sending
                    /*else
                    {
                        callbackFunction("Message is being sent!");
                    }*/
                }
                else
                {
                    callbackFunction("Port " + serialPort.PortName + " is not connected!");
                    log.Error("Port " + serialPort.PortName + " is not connected!");
                }
            }
            catch (Exception e)
            {
                callbackFunction("An unexpected error occurs when processing!");
                log.Error(e.Message);
                throw e;
            }
        }

        // Serial transmission
        void SendData(byte[] data, int i, int length)
        {
            try
            {
                serialPort.Write(data, i, length);
            }
            catch (TimeoutException e)
            {
                callbackFunction("Time out when sending message!");
                log.Error(e.Message);
                throw e;
            }
            catch (Exception e)
            {
                callbackFunction("An unexpected error occurs when processing!");
                log.Error(e.Message);
                throw e;
            }
        }

        // Receive message
        void ReceiveMsg(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // Receive data from the port
                string readdata = serialPort.ReadExisting();
                byte[] bdata = Encoding.GetEncoding("UTF-8").GetBytes(readdata);

                // Phase 00: Move to STX wait if idle
                if (state == State.IDLE)
                {
                    state = State.STX_WAITING;
                }
                // Data reception event processing
                foreach (byte byteData in bdata)
                {
                    // status check
                    switch (state)
                    {
                        // Phase 01: When waiting for STX
                        case State.STX_WAITING:
                            // Confirmation of received data
                            switch (byteData)
                            {
                                // Event R01: Receive STX
                                case (byte)MARKED_BYTES.STX:
                                    // Clear receive buffer and store STX
                                    receiveBuffer.Clear();
                                    receiveBuffer.Add(byteData);

                                    // Move to ETX waiting
                                    state = State.ETX_WAITING;
                                    ReceiveTimerStart(OnReceiveTimeOver, timeOutOnReceive);
                                    break;
                            }
                            break;

                        // Phase 02: Waiting for ETX
                        case State.ETX_WAITING:
                            // Confirmation of received data
                            switch (byteData)
                            {
                                // Event R01: Receive STX
                                case (byte)MARKED_BYTES.STX:
                                    // do nothing
                                    break;

                                // Event R03: ETB reception
                                case (byte)MARKED_BYTES.ETB:
                                // Event R04: ETX reception
                                case (byte)MARKED_BYTES.ETX:
                                    // Store in receive buffer
                                    receiveBuffer.Add(byteData);

                                    //  Move to BCC waiting
                                    state = State.BCC_WAITING;
                                    ReceiveTimerStart(OnReceiveTimeOver, timeOutOnReceive);
                                    break;

                                // Event R02: General data reception
                                // Event R05: ACK reception
                                // Event R06: NAK reception
                                // Event R07: EOT reception
                                default:
                                    // Store in receive buffer
                                    receiveBuffer.Add(byteData);
                                    ReceiveTimerStart(OnReceiveTimeOver, timeOutOnReceive);
                                    break;
                            }
                            break;

                        // Phase 02: Waiting for BCC
                        // BCC check and ACK / NAK response
                        case State.BCC_WAITING:
                            log.Info("Data string received: " + string.Join("", Array.ConvertAll(receiveBuffer.ToArray(), i => i.ToString())));
                            byte receiveResponse = CheckMessage(receiveBuffer.ToArray(), byteData);
                            SendData(new byte[] { receiveResponse }, 0, 1);

                            // Block reception post-processing
                            // For ACK response
                            if (receiveResponse == (byte)MARKED_BYTES.ACK)
                            {
                                // Fetch data from receive buffer
                                byte[] msgArray = receiveBuffer.Skip(1).Take(receiveBuffer.Count - 2).ToArray();

                                // JIS-8 code decoding
                                string msg = DecodeJIS8(msgArray);

                                // Combine to received message
                                receiveMessage.Append(msg);

                                // State transition processing
                                // When receiving ETX block
                                if (receiveBuffer.Last() == (byte)MARKED_BYTES.ETX)
                                {
                                    // Move to EOT waiting
                                    state = State.EOT_WAITING;
                                    ReceiveTimerStart(OnEotReceiveTimeOver, timeOutOnEotReceive);
                                }
                                // When receiving ETB block
                                else
                                {
                                    // Move to STX wait
                                    state = State.STX_WAITING;
                                    ReceiveTimerStart(OnReceiveTimeOver, timeOutOnReceive);
                                }
                            }
                            // For NAK response
                            else
                            {
                                // Move to STX wait
                                state = State.STX_WAITING;
                                ReceiveTimerStart(OnReceiveTimeOver, timeOutOnReceive);
                            }
                            break;

                        // Phase 03: Waiting for EOT
                        case State.EOT_WAITING:
                            // Confirmation of received data
                            switch (byteData)
                            {
                                // Event R07: EOT reception
                                case (byte)MARKED_BYTES.EOT:
                                    // Base64 decoding of received message
                                    string message = DecodeBase64(receiveMessage.ToString());

                                    // Transition to idle
                                    state = State.IDLE;
                                    ReceiveTimerStop();

                                    // Return the decoded message
                                    callbackFunction(message);
                                    log.Info("Message decoded Base 64: " + message);
                                    receiveMessage = new StringBuilder();
                                    messageSent = "";
                                    break;
                            }
                            break;

                        // Phase 04: Waiting for ACK
                        // Phase 05: Waiting for ACK
                        case State.ACK_WAITING:
                            // Confirmation of received data
                            switch (byteData)
                            {
                                // Event R05: ACK reception
                                case (byte)MARKED_BYTES.ACK:
                                    // Read next block
                                    messageBlock.Dequeue();
                                    // If there is no next block
                                    if (messageBlock.Count == 0)
                                    {
                                        // Phase 05: EOT transmission because it is after sending the ETX block
                                        SendData(new byte[] { (byte)MARKED_BYTES.EOT }, 0, 1);
                                        log.Info("Message input: " + messageSent + "\n" + "EOT sent!");

                                        // Transition to idle
                                        state = State.IDLE;
                                        ReceiveTimerStop();
                                    }
                                    // If there is a next block
                                    else
                                    {
                                        // Phase 04: Since the ETB block has been sent, the next block will be sent.
                                        SendData(messageBlock.Peek(), 0, messageBlock.Peek().Length);
                                        log.Info("Message input: " + messageSent + "\n" + "Data string sent: " + string.Join("", Array.ConvertAll(messageBlock.Peek(), i => i.ToString())));

                                        // Move to ACK wait
                                        state = State.ACK_WAITING;
                                        ReceiveTimerStart(OnACKWaitingTimeOver, timeOutOnACKWaiting);
                                    }
                                    break;

                                // Event R06: NAK reception
                                case (byte)MARKED_BYTES.NAK:
                                    // Block retransmission
                                    SendData(messageBlock.Peek(), 0, messageBlock.Peek().Length);
                                    log.Info("Message input: " + messageSent + "\n" + "Data string re-sent: " + string.Join("", Array.ConvertAll(messageBlock.Peek(), i => i.ToString())));

                                    // Move to ACK wait
                                    state = State.ACK_WAITING;
                                    ReceiveTimerStart(OnACKWaitingTimeOver, timeOutOnACKWaiting);
                                    break;
                            }
                            break;
                    }
                }
                // Data reception event processing completed
            }
            catch (Exception err)
            {
                callbackFunction("An unexpected error occurs when processing!");
                log.Error(err.Message);
                throw err;
            }
        }

        void ReceiveTimerStart(ElapsedEventHandler handler, int timeOut)
        {
            // Stop the running timer
            if (workingTimer != null)
            {
                workingTimer.Stop();
            }
            // Create a timeOut in milisecond timer
            workingTimer = new Timer(timeOut);
            workingTimer.AutoReset = false;
            workingTimer.Elapsed += handler;
            // Timer start
            workingTimer.Start();
        }

        // Receive timer stopped
        void ReceiveTimerStop()
        {
            if (workingTimer != null)
            {
                workingTimer.Stop();
                workingTimer = null;
            }
        }

        void OnACKWaitingTimeOver(object sender, ElapsedEventArgs e)
        {
            // Stop timer
            Timer timer = sender as Timer;
            if (timer != null)
            {
                timer.Stop();
            }
            // NAK transmission
            SendData(new byte[] { (byte)MARKED_BYTES.NAK }, 0, 1);
            callbackFunction("Time out when waiting receive ACK after message was sent!");
            log.Error("Time out when waiting receive ACK after message was sent!");
            // Move to EOT waiting
            state = State.EOT_WAITING;
            ReceiveTimerStart(OnEotReceiveTimeOver, timeOutOnEotReceive);
            // Discard transmission block
            messageBlock.Clear();
            // Transition to idle
            state = State.IDLE;
        }

        void OnReceiveTimeOver(object sender, ElapsedEventArgs e)
        {
            // Stop timer
            Timer timer = sender as Timer;
            if (timer != null)
            {
                timer.Stop();
            }
            // NAK transmission
            SendData(new byte[] { (byte)MARKED_BYTES.NAK }, 0, 1);
            callbackFunction("Time out when waiting receive message!");
            log.Error("Time out when waiting receive message!");
            // Move to STX wait
            state = State.STX_WAITING;
            ReceiveTimerStart(OnReceiveTimeOver, timeOutOnReceive);
        }

        void OnEotReceiveTimeOver(object sender, ElapsedEventArgs e)
        {
            // Stop timer
            Timer timer = sender as Timer;
            if (timer != null)
            {
                timer.Stop();
            }
            // NAK transmission
            SendData(new byte[] { (byte)MARKED_BYTES.NAK }, 0, 1);
            callbackFunction("Time out when waiting EOT!");
            log.Error("Time out when waiting EOT!");
            // Move to EOT waiting
            state = State.EOT_WAITING;
            ReceiveTimerStart(OnEotReceiveTimeOver, timeOutOnEotReceive);
        }

        void ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            callbackFunction("An unexpected error occurs when processing!");
            log.Error(e.EventType.ToString());
        }
    }
}