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
using System.Windows.Threading;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.State;

namespace WPSaturnEMoney.Views
{
    /// <summary>
    /// Interaction logic for Screen_emErrorToMaintenance.xaml
    /// </summary>
    public partial class Screen_emErrorToMaintenance : UserControl
    {
        private int _pressLeft = 0;
        private int _pressRight = 0;
        DispatcherTimer timer = new DispatcherTimer();

        public Screen_emErrorToMaintenance()
        {
            InitializeComponent();

            timer.Interval = TimeSpan.FromSeconds(30);
            timer.Tick += timer_Tick;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            // Reset press status if not completed within 30 seconds after the first press
            ResetStaffKey();
        }

        private void gridHidden_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int x = (int)e.GetPosition(gridHidden).X;
            if (x < 60)
            {
                _pressLeft++;
            }
            else if (x > 1024 - 60)
            {
                _pressRight++;
            }
            else if (x > 280 && x < 740)
            {
                ResetStaffKey();
            }
            Utilities.Log.Info($"StaffKey: {_pressLeft} {_pressRight}");

            if (_pressLeft + _pressRight == 1)
            {
                timer.Start();
            }
            else if (_pressLeft + _pressRight == 3)
            {
                if (_pressLeft == 0 || _pressRight == 0)
                {
                    ResetStaffKey();
                }
                else
                {
                    timer.Stop();
                    if (Models.GlobalData.IsInPrinterMaintenance)
                    {
                        Models.GlobalData.MPNextStep = Utilities.MPStep.emMT_PW;
                    }
                    else
                    {
                        Session.ScreenState.NextState = StateMachine.State.emLoginToMaintenance;
                    }
                    Utilities.Log.Info("StaffKey success");
                }
            }
        }

        private void ResetStaffKey()
        {
            _pressLeft = 0;
            _pressRight = 0;
            timer.Stop();
            Utilities.Log.Info($"Reset StaffKey: {_pressLeft} {_pressRight}");
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Utilities.Log.Info($"Press button [{btnOK.Content}]");
        }
    }
}
