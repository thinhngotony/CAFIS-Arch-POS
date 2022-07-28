using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WPSaturnEMoney.Models;

namespace WPSaturnEMoney.Views
{
    public partial class CustomResources : ResourceDictionary
    {
        private Brush _preBackgroud;
        private Brush _preForeground;
        private BrushConverter _converter = new BrushConverter();
        public void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button btn = sender as Button;
            _preBackgroud = btn.Background;
            _preForeground = btn.Foreground;
            try
            {
                btn.Background = GlobalData.BasicConfig.color.common.btn_touch is null
                    ? (Brush)_converter.ConvertFromString("#00DFFF")
                    : (Brush)_converter.ConvertFromString(GlobalData.BasicConfig.color.common.btn_touch);
            }
            catch (NullReferenceException)
            {
                btn.Background = (Brush)_converter.ConvertFromString("#00DFFF");
            }
            try
            {
                btn.Foreground = GlobalData.BasicConfig.color.common.btn_touch_label is null
                    ? (Brush)_converter.ConvertFromString("#254474")
                    : (Brush)_converter.ConvertFromString(GlobalData.BasicConfig.color.common.btn_touch_label);
            }
            catch (NullReferenceException)
            {
                btn.Foreground = (Brush)_converter.ConvertFromString("#00DFFF");
            }
        }

        private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Reset button background color if pressed on the button but releases outside
            Button btn = sender as Button;
            /*int mousePosX = (int)e.GetPosition(btn).X;
            int mousePosY = (int)e.GetPosition(btn).Y;
            bool isMouseLeaved = mousePosX < 0 || mousePosX > btn.ActualWidth ||
                                 mousePosY < 0 || mousePosY > btn.ActualHeight;
            if (isMouseLeaved)*/
            {
                btn.Background = _preBackgroud;
                btn.Foreground = _preForeground;
            }
        }
    }
}
