﻿using System;
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
    /// Interaction logic for Screen_emMT_ZRESJ.xaml
    /// </summary>
    public partial class Screen_emMT_ZRESJ : UserControl
    {
        public Screen_emMT_ZRESJ()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Common.Utilities.Log.Info($"Press button [{btnOK.Content}]");
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            Common.Utilities.Log.Info($"Press button [{btnReturn.Content}]");
        }
    }
}
