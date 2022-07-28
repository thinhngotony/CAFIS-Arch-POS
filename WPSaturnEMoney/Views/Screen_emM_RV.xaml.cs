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

namespace WPSaturnEMoney.Views
{
    /// <summary>
    /// Interaction logic for Screen_emM_RV.xaml
    /// </summary>
    public partial class Screen_emM_RV : UserControl
    {
        public Screen_emM_RV()
        {
            InitializeComponent();
        }
        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            if (!(btn1.Content is null))
            {
                GlobalData.ServiceName = Utilities.GetServiceName(btn1.Content.ToString());
                Utilities.Log.Info($"Press button [{btn1.Content}]");
            }
        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            if (!(btn2.Content is null))
            {
                GlobalData.ServiceName = Utilities.GetServiceName(btn2.Content.ToString());
                Utilities.Log.Info($"Press button [{btn2.Content}]");
            }
        }

        private void btn3_Click(object sender, RoutedEventArgs e)
        {
            if (!(btn3.Content is null))
            {
                GlobalData.ServiceName = Utilities.GetServiceName(btn3.Content.ToString());
                Utilities.Log.Info($"Press button [{btn3.Content}]");
            }
        }

        private void btn4_Click(object sender, RoutedEventArgs e)
        {
            if (!(btn4.Content is null))
            {
                GlobalData.ServiceName = Utilities.GetServiceName(btn4.Content.ToString());
                Utilities.Log.Info($"Press button [{btn4.Content}]");
            }
        }

        private void btn5_Click(object sender, RoutedEventArgs e)
        {
            if (!(btn5.Content is null))
            {
                GlobalData.ServiceName = Utilities.GetServiceName(btn5.Content.ToString());
                Utilities.Log.Info($"Press button [{btn5.Content}]");
            }
        }

        private void btn6_Click(object sender, RoutedEventArgs e)
        {
            if (!(btn6.Content is null))
            {
                GlobalData.ServiceName = Utilities.GetServiceName(btn6.Content.ToString());
                Utilities.Log.Info($"Press button [{btn6.Content}]");
            }
        }
    }
}
