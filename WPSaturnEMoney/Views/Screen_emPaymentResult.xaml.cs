using System.Windows;
using System.Windows.Controls;

namespace WPSaturnEMoney.Views
{
    /// <summary>
    /// Interaction logic for Screen_emInit.xaml
    /// </summary>
    public partial class Screen_emPaymentResult : UserControl
    {
        public Screen_emPaymentResult()
        {
            InitializeComponent();
        }

        private void BtnClick(object sender, RoutedEventArgs e)
        {
            Common.Utilities.Log.Info($"Press button [{BtnOK.Content}]");
        }
    }
}
