using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;
using WPSaturnEMoney.State;

namespace WPSaturnEMoney.Views
{
    public partial class Screen_emProcessOperation : UserControl
    {
        public Screen_emProcessOperation()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GuideImage.Source = new BitmapImage(new Uri(GlobalData.AppPath + GlobalData.SaturnDeviceImagePath, UriKind.Absolute));
        }
    }
}
