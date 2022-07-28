using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Diagnostics;
using WPSaturnEMoney.Models;
using WPSaturnEMoney.State;

namespace WPSaturnEMoney.Common
{
    public sealed class WinAPI
    {
        public delegate bool EnumDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("User32.dll", EntryPoint = "RegisterWindowMessage")]
        public static extern int RegisterWindowMessage(string lpString);

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern int FindWindow(String lpClassName, String lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(int hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "ReplyMessage")]
        public static extern bool ReplyMessage(IntPtr lResult);

        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        public const int SW_HIDE = 0;
        public const int SW_SHOWNORMAL = 1;
        public const int SW_SHOWMAXIMIZED = 3;
        public const int SW_RESTORE = 9;
        public const int SW_SHOWDEFAULT = 10;

        public const int WM_CLOSE = 0x0010;
        public const int WM_SHOWWINDOW = 0x0018;
        public const int WM_COPYDATA = 0x4A;
        public const int WM_USER = 0x400;
        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_LBUTTONUP = 0x202;
        public const int MK_LBUTTON = 0x0001;

        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_TOPMOST = 0x0008;

        //COPYDATASTRUCT構造体 
        public struct COPYDATASTRUCT
        {
            public int dwData;　　//送信する32ビット値
            public int cbData;　　//lpDataのバイト数
            public string lpData;　　//送信するデータへのポインタ(0も可能)
        }
        
        public static bool AppIsRunning()
        {
            bool isRunning = false;
            try
            {
                IntPtr hwnd = FindWindowByCaption(IntPtr.Zero, GlobalData.AppName);

                if (hwnd != IntPtr.Zero) // Check running process   
                {
                    isRunning = true;
                    ShowWindow(hwnd, SW_RESTORE); // Activate the window, if process is already running  
                }
            }
            catch (Exception ex)
            {
                Utilities.Log.Error("▲ Check App running throw exception:  " + ex.ToString());
            }
            return isRunning;
        }
    
        public static void HideWindow()
        {
            IntPtr hwnd = FindWindowByCaption(IntPtr.Zero, GlobalData.AppName);
            ShowWindow(hwnd, SW_HIDE);
            Utilities.Log.Info("Hide Window!");
        }

        public static void BringWindowTop()
        {
            IntPtr hwnd = FindWindowByCaption(IntPtr.Zero, GlobalData.AppName);
            ShowWindow(hwnd, SW_RESTORE);
            Utilities.Log.Info("Restore Window!");
        }

        public static bool IsWindowTopMost(IntPtr hWnd)
        {
            int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            return (exStyle & WS_EX_TOPMOST) == WS_EX_TOPMOST;
        }

        public static bool SendMouseEvent()
        {
            FileStruct.CsFileWrite(GlobalData.AppPath, GlobalData.ToPosPath, GlobalData.Data_ToPosDat);
            if (!ClickEvent())
                return false;
            return true;
        }

        /// <summary>
        /// Send click event on specific location of another app.
        /// </summary>
        /// <param name="configInfo">Config information of the other app.</param>
        public static bool ClickEvent()
        {
            var configInfo = GlobalData.PosIFConfig;
            int[] location = configInfo.pos_btn_loc;
            string posProcessName = configInfo.pos_process_name;
            string posWindowName = configInfo.pos_window_name;
            int hWnd = FindWindow(null, posWindowName);
            // Find pos process
            Process[] posProcess = Process.GetProcessesByName(posProcessName);
            if (0 == posProcess.Length)
            {
                Utilities.Log.Error("▲ 相手プロセスが取得できません");
                return false;
            }

            // Find pos window
            if (hWnd == 0)
            {
                Utilities.Log.Error("▲ 相手Windowのハンドルが取得できません");
                return false;
            }

            Utilities.Log.Info("Send mouse button position: x: " + location[0] + "; y: "+ location[1]);
            int mouseDown = PostMessage(hWnd, WM_LBUTTONDOWN, MK_LBUTTON, GetLoc(location[0], location[1]));
            int mouseUp = PostMessage(hWnd, WM_LBUTTONUP, MK_LBUTTON, GetLoc(location[0], location[1]));
            return mouseDown == 1 && mouseUp == 1;
        }
        public static int GetLoc(int x_value, int y_value)
        {
            return x_value + 65536 * y_value;
        }

        public static void ClickEventCommand()
        {
            ClickEvent();
        }

        public async static Task EndTransaction()
        {
            Session.IsStatusOK = false;
            Session.IsTransactionStart = false;
            Session.MaintenanceMode = "";

            // Close printer connection before sending mouse event.
            PrintAPI.CloseConnection();

            // Send mouse click event to the button on regi-app.
            FileStruct.PosIFConfig config = GlobalData.PosIFConfig;
            SendMouseEvent();

            await HideWindowAfterTimeout((int)config.pos_timeout);

            Session.ScreenState.NextState = StateMachine.State.emIdle;
            Utilities.Log.Info("■ End Operation!");
        }

        public async static Task HideWindowAfterTimeout(int timeout)
        {
            // Wait for hiding and unlocking topmost by regi-app.
            bool isHidden = false;
            // IntPtr hwnd = FindWindowByCaption(IntPtr.Zero, GlobalData.AppName);
            while (timeout >= 100)
            {
                await Task.Delay(100);
                timeout -= 100;
                
                // if (!IsWindowTopMost(hwnd))
                if (GlobalData.IsWindowHidden)
                {
                    isHidden = true;
                    break;
                }
            }

            // Auto-hide app if timeout.
            if (!isHidden)
            {
                Utilities.Log.Info("Auto-hide Saturn EMoney App because of timeout.");
                HideWindow();
            }
        }

        public static void ClerkCallNotification(string errorCode)
        {
            // Prepare status information
            var toPosData = new FileStruct.ToPos_ClerkCallNotify
            {
                service = "EMONEY",
                sequence = GlobalData.Data_FromPosDat.sequence,
                last_operator = GlobalData.LastOperator,
                result = "10", // clerk call notification
                SettledAmount = "",
                CurrentService = "",
                statementID = "",
                error_code = errorCode
            };

            // Write information file (ToPos.dat)
            FileStruct.CsFileWrite(GlobalData.AppPath, GlobalData.ToPosPath, toPosData);

            // Send mouse click event
            ClickEvent();
        }
    }
}
