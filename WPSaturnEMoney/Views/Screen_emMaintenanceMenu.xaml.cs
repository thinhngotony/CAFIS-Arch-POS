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
using WPSaturnEMoney.State;

namespace WPSaturnEMoney.Views
{
    public partial class Screen_emMaintenanceMenu : UserControl
    {
        public Screen_emMaintenanceMenu()
        {
            InitializeComponent();
        }
        private void btnCheckConnection_Click(object sender, RoutedEventArgs e)
        {
            Session.ClickedButton_emM_MENU = "check_connection";
        }

        private void btnReprintReceipt_Click(object sender, RoutedEventArgs e)
        {
            Session.ClickedButton_emM_MENU = "reprint_receipt";
        }

        private void btnDailyTotal_Click(object sender, RoutedEventArgs e)
        {
            //Session.ClickedButton_emM_MENU = "daily_total";
            //Session.ClickedButton_emM_MENU = "daily_reprint";
        }

        private void btnTransactionInquiry_Click(object sender, RoutedEventArgs e)
        {
            Session.ClickedButton_emM_MENU = "transaction_inquiry";
        }

        private void btnBalanceInquiry_Click(object sender, RoutedEventArgs e)
        {
            Session.ClickedButton_emM_MENU = "balance_inquiry";
        }
    }
}
