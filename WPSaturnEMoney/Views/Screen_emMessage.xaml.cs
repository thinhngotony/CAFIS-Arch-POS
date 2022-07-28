using System.Windows;
using System.Windows.Controls;

namespace WPSaturnEMoney.Views
{
    /// <summary>
    /// Interaction logic for Screen_emI.xaml
    /// </summary>
    public partial class Screen_emMessage : UserControl
    {
        public Screen_emMessage()
        {
            InitializeComponent();
        }

        private void BtnClick(object sender, RoutedEventArgs e)
        {
            Button btn = (sender as Button);
            if (!(btn.Content is null))
            {
                Common.Utilities.Log.Info($"Press button [{btn.Content}]");
            }
        }
    }
}
