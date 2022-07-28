using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPSaturnEMoney.State;

namespace WPSaturnEMoney.Views
{
    public partial class Screen_emMaintenanceMenuFromError : UserControl
    {
        public Screen_emMaintenanceMenuFromError()
        {
            InitializeComponent();
        }

        private void btnCheckConnection_Click(object sender, RoutedEventArgs e)
        {
            Session.ClickedButton_emMT_MENU = "check_connection";
        }

        private void btnReprintReceipt_Click(object sender, RoutedEventArgs e)
        {
            Session.ClickedButton_emMT_MENU = "reprint_receipt";
        }
    }
}
