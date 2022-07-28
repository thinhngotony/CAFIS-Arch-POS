using System.Windows;
using System.Windows.Controls;
using WPSaturnEMoney.Common;

namespace WPSaturnEMoney.Views
{
    /// <summary>
    /// Interaction logic for Screen_emZF.xaml
    /// </summary>
    public partial class Screen_emBalanceInquiryResult : UserControl
    {
        public Screen_emBalanceInquiryResult()
        {
            InitializeComponent();
        }
        private void BtnClick(object sender, RoutedEventArgs e)
        {
            Button btn = (sender as Button);
            if (!(btn.Content is null))
            {
                Utilities.Log.Info($"Press button [{btn.Content}]");
            }
        }
    }
}
