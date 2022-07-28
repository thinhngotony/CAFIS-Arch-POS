using System;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.IO;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;
using WPSaturnEMoney.State;
using AxOposCAT_VescaCO;
using System.Windows.Forms;

namespace WPSaturnEMoney
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsStatusOK = false;
        public MainWindow()
        {
            InitializeComponent();

            // Other app communication config
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // VescaOCX initialization
            var host = new System.Windows.Forms.Integration.WindowsFormsHost();
            PaymentAPI.AxOp = new AxOPOSCAT_VESCA();
            host.Child = PaymentAPI.AxOp;
            axOpGrid.Children.Add(host);
            PaymentAPI.AxOp.OutputCompleteEvent += new _IOPOSCATEvents_OutputCompleteEventEventHandler(AxOp_OutputCompleteEventHandler);
            PaymentAPI.AxOp.ErrorEvent += new _IOPOSCATEvents_ErrorEventEventHandler(AxOp_ErrorEventHandler);
        }

        // Add hook to receive message from window procedure
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(new HwndSourceHook(WndProc));
        }

        // Handle and reply to messages from pos
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            Message m = Message.Create(hwnd, msg, wParam, lParam);
            if (m.Msg == GlobalData.CustomMessageID)
            {
                ReceiveMessage(m);
            }
            else if (m.Msg == WinAPI.WM_SHOWWINDOW)
            {
                if (m.WParam.ToInt32() == 1)
                {
                    GlobalData.IsWindowHidden = false;
                    if (GlobalData.BasicConfig.customer_display_flag != "0")
                    {
                        IntPtr hwnd1 = WinAPI.FindWindowByCaption(IntPtr.Zero, GlobalData.CustomerWindowName);
                        WinAPI.MoveWindow(hwnd1, (int)Left + (int)Width, (int)Top, (int)Width, (int)Height, false);
                        WinAPI.ShowWindow(hwnd1, WinAPI.SW_RESTORE);
                        System.Threading.Tasks.Task.Delay(50)
                            .ContinueWith((t) => {
                                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                                {
                                    PopupNotification.IsOpen = GlobalData.IsPopupNotificationOpen;
                                }, System.Windows.Threading.DispatcherPriority.ContextIdle);
                            });
                    }
                }
                else
                {
                    GlobalData.IsWindowHidden = true;
                    IntPtr hwnd1 = WinAPI.FindWindowByCaption(IntPtr.Zero, GlobalData.CustomerWindowName);
                    WinAPI.ShowWindow(hwnd1, WinAPI.SW_HIDE);
                    PopupNotification.IsOpen = false;
                }
            }
            return IntPtr.Zero;
        }

        private void ReceiveMessage(Message m)
        {
            Utilities.Log.Info("HWND: " + m.HWnd + ", UINT: " + m.Msg + ", WParam: " + m.WParam.ToInt32() + ", LParam: " + m.LParam.ToInt32());
            switch (m.WParam.ToInt32())
            {
                case 1: // Confirm status
                    if (Session.ScreenState.CurrentState == StateMachine.State.emIdle)
                    {
                        m.Result = (IntPtr)1;
                        IsStatusOK = true;
                    }
                    else if (Session.ScreenState.CurrentState != StateMachine.State.emMAorMI_EMSG)
                    {
                        m.Result = (IntPtr)2;
                        IsStatusOK = false;
                    }
                    else
                    {
                        m.Result = (IntPtr)9;
                        IsStatusOK = false;
                    }
                    break;
                case 2: // Read FromPos.dat and start transaction
                    bool readSuccess = FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.FromPosPath, ref GlobalData.Data_FromPosDat);
                    (int res, string msg) validFromPos = FileStruct.ValidateFromPos(GlobalData.Data_FromPosDat);
                    if (Session.ScreenState.CurrentState == StateMachine.State.emIdle) {
                        if (readSuccess && validFromPos.res == 1)
                        {
                            if (IsStatusOK)
                            {
                                Session.IsStatusOK = true;
                                IsStatusOK = false;
                            }
                            m.Result = (IntPtr)1;
                            Session.IsTransactionStart = true;
                        }
                        else if (validFromPos.res == 3)
                        {
                            m.Result = (IntPtr)3;
                            Session.IsTransactionStart = false;
                        }
                        else
                        {
                            m.Result = (IntPtr)9;
                            Session.IsTransactionStart = false;
                        }
                    }
                    else
                    {
                        m.Result = (IntPtr)9;
                        Session.IsTransactionStart = false;
                    }
                    if (File.Exists(GlobalData.AppPath + GlobalData.FromPosPath))
                    {
                        File.Delete(GlobalData.AppPath + GlobalData.FromPosPath);
                    }
                    break;
                case 9: // End add-on app
                    if (Session.ScreenState.CurrentState == StateMachine.State.emInit || Session.ScreenState.CurrentState == StateMachine.State.emIdle)
                    {
                        m.Result = (IntPtr)1;
                        SaturnAPI.Terminate();
                        Close();
                        GlobalData.customerWindow.Close();
                    }
                    else
                    {
                        m.Result = (IntPtr)9;
                    }
                    break;
                default:
                    int retVal = m.WParam.ToInt32() + m.LParam.ToInt32();
                    m.Result = (IntPtr)retVal;
                    break;
            }
            WinAPI.ReplyMessage(m.Result);
            Utilities.Log.Info("Result: " + m.Result.ToInt32());
        }

        private void AxOp_OutputCompleteEventHandler(object sender, _IOPOSCATEvents_OutputCompleteEventEvent e)
        {
            Utilities.Log.Info("-> OutputComplete !!!"
                            + "\r\n\t-> RequestedData=" + PaymentAPI.AxOp.RequestedData
                            + "\r\n\t-> ResponseData=" + PaymentAPI.AxOp.ResponseData);

            PaymentAPI.IsCompleteEventReceived = true;
            PaymentAPI.LastTimeEventOccur = DateTime.Now;
        }

        private void AxOp_ErrorEventHandler(object sender, _IOPOSCATEvents_ErrorEventEvent e)
        {
            Utilities.Log.Error("-> ErrorEvent !!!"
                            + "\r\n\t-> RequestedData=" + PaymentAPI.AxOp.RequestedData
                            + "\r\n\t-> ResponseData=" + PaymentAPI.AxOp.ResponseData
                            + "\r\n\t-> ResponseCode=" + PaymentAPI.AxOp.ResponseCode
                            + "\r\n\t-> ErrorCode=" + PaymentAPI.AxOp.ErrorCode
                            + "\r\n\t-> Errorcodedetail=" + PaymentAPI.AxOp.Errorcodedetail
                            + "\r\n\t-> ErrorCodeExtended=" + PaymentAPI.AxOp.ErrorCodeExtended
                            + "\r\n\t-> ResultCode=" + PaymentAPI.AxOp.ResultCode
                            + "\r\n\t-> ResultCodeExtended=" + PaymentAPI.AxOp.ResultCodeExtended
                            + "\r\n\t-> Message=" + PaymentAPI.AxOp.Message);

            PaymentAPI.IsCompleteEventReceived = false;
            PaymentAPI.LastTimeEventOccur = DateTime.Now;
        }

        private void PopupBtnOKClick(object sender, RoutedEventArgs e)
        {
            GlobalData.IsPopupNotificationOpen = false;
            PopupNotification.IsOpen = GlobalData.IsPopupNotificationOpen;
            MessageNotification.Text = "";
        }
    }
}