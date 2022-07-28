using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Timers;

namespace Vjp.Skinapp_IF_Test.Common
{
    public sealed class WinAPI
    {
        public delegate bool EnumDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("User32.dll", EntryPoint = "RegisterWindowMessage")]
        public static extern int RegisterWindowMessage(string lpString);

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern int FindWindow(String lpClassName, String lpWindowName);

        [DllImport("User32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(int hWnd, uint Msg, int wParam, int lParam);

        [DllImport("User32.dll", EntryPoint = "ReplyMessage")]
        public static extern bool ReplyMessage(IntPtr lResult);

        [DllImport("User32.dll", EntryPoint = "ShowWindow")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("User32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        public const int SW_HIDE = 0;
        public const int SW_SHOWNORMAL = 1;
        public const int SW_SHOWMAXIMIZED = 3;
        public const int SW_RESTORE = 9;
        public const int SW_SHOWDEFAULT = 10;

        public const int WM_COPYDATA = 0x4A;
        public const int WM_USER = 0x400;
        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_LBUTTONUP = 0x202;
        public const int MK_LBUTTON = 0x0001;

        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_TOPMOST = 0x0008;
        
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
        }

        public static void BringWindowTop()
        {
            IntPtr hwnd = FindWindowByCaption(IntPtr.Zero, GlobalData.AppName);
            ShowWindow(hwnd, SW_RESTORE);
        }

        public static bool IsWindowTopMost(IntPtr hWnd)
        {
            int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            return (exStyle & WS_EX_TOPMOST) == WS_EX_TOPMOST;
        }
    }
}
