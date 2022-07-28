using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vjp.Skinapp_IF_Test.Common;

namespace Vjp.Skinapp_IF_Test
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            GlobalData.AppPath = AppDomain.CurrentDomain.BaseDirectory;
            if (WinAPI.AppIsRunning())
            {
                MessageBox.Show("この アプリケーション は 複数 起動 でき ませ ん 。");
                return;
            }
            bool readPosIFOK = Utilities.CsFileRead(GlobalData.AppPath, GlobalData.PosIFConfigPath, ref GlobalData.PosIFConfig)
                               && !Utilities.IsNullorEmpty(GlobalData.PosIFConfig)
                               && GlobalData.PosIFConfig.pos_btn_loc.Length == 2;
            if (!readPosIFOK)
            {
                MessageBox.Show("Invalid Config/PosIFConfig.json!", GlobalData.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Utilities.Log.Error("Invalid Config/PosIFConfig.json!");
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
