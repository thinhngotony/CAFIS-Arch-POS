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
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;

namespace WPSaturnEMoney.Views
{
    public partial class Screen_emLoginToMaintenance : UserControl
    {
        private Window thisWindow;
        private string[] _ingoreKey = { "Tab", "Alt", "System", "NumLock", "Ctrl"};
        private MouseEventArgs _mouseDown = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
        {
            RoutedEvent = Mouse.PreviewMouseDownEvent
        };

        public Screen_emLoginToMaintenance()
        {
            InitializeComponent();
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            string keyPress = e.Key.ToString();
            switch (keyPress)
            {
                case "D0":
                case "NumPad0":
                    btn_0.RaiseEvent(_mouseDown);
                    break;
                case "D1":
                case "NumPad1":
                    btn_1.RaiseEvent(_mouseDown);
                    break;
                case "D2":
                case "NumPad2":
                    btn_2.RaiseEvent(_mouseDown);
                    break;
                case "D3":
                case "NumPad3":
                    btn_3.RaiseEvent(_mouseDown);
                    break;
                case "D4":
                case "NumPad4":
                    btn_4.RaiseEvent(_mouseDown);
                    break;
                case "D5":
                case "NumPad5":
                    btn_5.RaiseEvent(_mouseDown);
                    break;
                case "D6":
                case "NumPad6":
                    btn_6.RaiseEvent(_mouseDown);
                    break;
                case "D7":
                case "NumPad7":
                    btn_7.RaiseEvent(_mouseDown);
                    break;
                case "D8":
                case "NumPad8":
                    btn_8.RaiseEvent(_mouseDown);
                    break;
                case "D9":
                case "NumPad9":
                    btn_9.RaiseEvent(_mouseDown);
                    break;
                case "Return":
                    btn_Return.RaiseEvent(_mouseDown);
                    break;
                case "Back":
                    btn_Back.RaiseEvent(_mouseDown);
                    break;
                case "Delete":
                    btn_Clear.RaiseEvent(_mouseDown);
                    break;
            }

            if (_ingoreKey.Contains(keyPress))
            {
                e.Handled = true;
            }
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ErrorMsg.Visibility = Visibility.Hidden;
            Button btn = sender as Button;
            string name = btn.Name.ToString();
            if (name == "btn_Return")
            {
                if (GlobalData.LoginData.Contains(Input_ID.Password))
                {
                    if (GlobalData.IsInPrinterMaintenance)
                    {
                        GlobalData.MPNextStep = Utilities.MPStep.emMP_EMSG;
                    }
                    else
                    {
                        Session.IsIDCorrect = true;
                    }
                    Utilities.Log.Info($"Confirm ID: {Input_ID.Password} (login success)");
                }
                else
                {
                    ErrorMsg.Visibility = Visibility.Visible;
                    Utilities.Log.Info($"Confirm ID: {Input_ID.Password} (not exist)");
                }
                Input_ID.Password = string.Empty;
            }
            else if (name == "btn_Back")
            {
                if (Input_ID.Password.Length > 0)
                {
                    Input_ID.Password = Input_ID.Password.Substring(0, Input_ID.Password.Length - 1);
                }
            }
            else if (name == "btn_Clear")
            {
                Input_ID.Password = string.Empty;
            }
            else
            {
                if (Input_ID.Password.Length < 13)
                {
                    Input_ID.Password += btn.Content.ToString();
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            thisWindow = Window.GetWindow(this);
            thisWindow.PreviewKeyDown += UserControl_PreviewKeyDown;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            thisWindow.PreviewKeyDown -= UserControl_PreviewKeyDown;
        }
    }
}
