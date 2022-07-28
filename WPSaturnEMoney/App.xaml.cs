using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;
using WPSaturnEMoney.ViewModels;

namespace WPSaturnEMoney
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            GlobalData.AppPath = AppDomain.CurrentDomain.BaseDirectory;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            Utilities.Log.Info("---------- Saturn EMoney App Startup----------");

            if (WinAPI.AppIsRunning())
            {
                //System.Windows.Forms.MessageBox.Show(new System.Windows.Forms.Form { TopMost = true }, "重複するアプリを起動できません。");
                Utilities.Log.Info("重複するアプリを起動できません。");
                Current.Shutdown();
            }

            base.OnStartup(e);
            MainWindowViewModel mainViewModel = new MainWindowViewModel();
            var window = new MainWindow() { DataContext = mainViewModel };
            var customerWindow = new CustomerWindow() { DataContext = mainViewModel };
            GlobalData.mainWindow = window;
            GlobalData.customerWindow = customerWindow;
            window.Show();
            customerWindow.Show();
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            byte[] assemblyData1 = null;
            byte[] assemblyData2 = null;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WPSaturnEMoney.VescaOCX.Interop.OposCAT_VescaCO.dll"))
            {
                assemblyData1 = new byte[stream.Length];
                stream.Read(assemblyData1, 0, assemblyData1.Length);
            }
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WPSaturnEMoney.VescaOCX.AxInterop.OposCAT_VescaCO.dll"))
            {
                assemblyData2 = new byte[stream.Length];
                stream.Read(assemblyData2, 0, assemblyData2.Length);
            }

            return Assembly.Load(assemblyData1, assemblyData2);
        }
    }
}
