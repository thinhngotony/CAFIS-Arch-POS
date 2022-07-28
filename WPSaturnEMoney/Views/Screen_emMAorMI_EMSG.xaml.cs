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

namespace WPSaturnEMoney.Views
{
    /// <summary>
    /// Interaction logic for Screen_emMAorMI_EMSG.xaml
    /// </summary>
    public partial class Screen_emMAorMI_EMSG : UserControl
    {
        public Screen_emMAorMI_EMSG()
        {
            InitializeComponent();
        }

        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            Common.Utilities.Log.Info($"Press button [{btnEnd.Content}]");
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Common.Utilities.Log.Info($"Press button [{btnOK.Content}]");
        }
    }
}
