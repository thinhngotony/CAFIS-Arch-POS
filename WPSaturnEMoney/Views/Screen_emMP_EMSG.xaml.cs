using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;
using WPSaturnEMoney.ViewModels;

namespace WPSaturnEMoney.Views
{
    /// <summary>
    /// Interaction logic for Screen_emMP_EMSG.xaml
    /// </summary>
    public partial class Screen_emMP_EMSG : UserControl
    {
        public Screen_emMP_EMSG()
        {
            InitializeComponent();
        }

        private void btnReprint_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.MPPressReprint = true;
            PrintErrorViewModel._viewModel_emMP_EMSG.ErrorMessage = "お待ちください...";
            PrintErrorViewModel._viewModel_emMP_EMSG.UpdateView();
            Utilities.Log.Info("Press button [再印刷]");
        }

        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            // Utilities.EndPrintReceipt();
            Utilities.Log.Info("Press button [終了]");
        }

        private void TextBlock_OnTargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (PrintErrorViewModel._viewModel_emMP_EMSG.ErrorMessage != "お待ちください...")
            {
                string colorCode = GlobalData.BasicConfig.color.maintenance.normal_btn;
                btnReprint.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorCode));
            }
        }
    }
}
