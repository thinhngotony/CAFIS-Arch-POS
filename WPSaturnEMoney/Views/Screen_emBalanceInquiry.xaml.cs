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
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;
using WPSaturnEMoney.State;

namespace WPSaturnEMoney.Views
{
    /// <summary>
    /// Interaction logic for Screen_emZ.xaml
    /// </summary>
    public partial class Screen_emBalanceInquiry : UserControl
    {
        public Screen_emBalanceInquiry()
        {
            InitializeComponent();
            InitBalanceInquiryButtons();
        }
        private void BtnClick(object sender, RoutedEventArgs e)
        {
            Button btn = (sender as Button);
            if (!(btn.Content is null))
            {
                GlobalData.ServiceName = Utilities.GetServiceName(btn.Content.ToString());
                Session.ScreenState.SetPreState(StateMachine.State.emBalanceInquiry);
                Session.ScreenState.NextState = StateMachine.State.emProcessOperation;
                Session.TimerCount = 0;
                Utilities.Log.Info($"Press button [{btn.Content}]");
            }
        }

        private void InitBalanceInquiryButtons()
        {
            int columns = 0;
            int rows = 0;
            int columnSpace = 0;
            int rowSpace = 0;
            FileStruct.Config configData = GlobalData.BasicConfig;
            System.Collections.Generic.List<string> balanceInquiryButtons = new System.Collections.Generic.List<string>();
            foreach (var service in configData.btn_order_ReadValue)
            {
                string brandName = Utilities.GetBrandName(service);
                if (GlobalData.ServiceNameCorrect.Contains(service) && !string.IsNullOrEmpty(brandName))
                {
                    balanceInquiryButtons.Add(brandName);
                }
            }
            switch (balanceInquiryButtons.Count)
            {
                case 1:
                    columns = 1;
                    rows = 1;
                    GridButtons.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    GridButtons.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    GridButtons.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    break;
                case 2:
                    columns = 2;
                    rows = 1;
                    columnSpace = 80;
                    GridButtons.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    GridButtons.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    GridButtons.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                    GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(columnSpace) });
                    GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                    GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    break;
                case 3:
                case 4:
                    columns = 2;
                    rows = 2;
                    columnSpace = 60;
                    rowSpace = 40;
                    GridButtons.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    for (int i = 0; i < rows; i++)
                    {
                        if (i != 0)
                        {
                            GridButtons.RowDefinitions.Add(new RowDefinition { Height = new GridLength(rowSpace) });
                        }
                        GridButtons.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                    }
                    GridButtons.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    for (int i = 0; i < columns; i++)
                    {
                        if (i != 0)
                        {
                            GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(columnSpace) });
                        }
                        GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                    }
                    GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    break;
                case 5:
                case 6:
                    columns = 2;
                    rows = 3;
                    columnSpace = 40;
                    rowSpace = 12;
                    GridButtons.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Pixel) });
                    for (int i = 0; i < rows; i++)
                    {
                        if (i != 0)
                        {
                            GridButtons.RowDefinitions.Add(new RowDefinition { Height = new GridLength(rowSpace) });
                        }
                        GridButtons.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    }
                    GridButtons.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Pixel) });
                    GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Pixel) });
                    for (int i = 0; i < columns; i++)
                    {
                        if (i != 0)
                        {
                            GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(columnSpace) });
                        }
                        GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    }
                    GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Pixel) });
                    break;
                case int n when 7 <= n && n <= 9:
                    columns = 3;
                    rows = 3;
                    columnSpace = 10;
                    rowSpace = 12;
                    GridButtons.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Pixel) });
                    for (int i = 0; i < rows; i++)
                    {
                        if (i != 0)
                        {
                            GridButtons.RowDefinitions.Add(new RowDefinition { Height = new GridLength(rowSpace) });
                        }
                        GridButtons.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    }
                    GridButtons.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Pixel) });
                    GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Pixel) });
                    for (int i = 0; i < columns; i++)
                    {
                        if (i != 0)
                        {
                            GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(columnSpace) });
                        }
                        GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    }
                    GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Pixel) });
                    break;
                case int n when 10 <= n && n <= 12:
                    columns = 3;
                    rows = 4;
                    columnSpace = 10;
                    rowSpace = 4;
                    GridButtons.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Pixel) });
                    for (int i = 0; i < rows; i++)
                    {
                        if (i != 0)
                        {
                            GridButtons.RowDefinitions.Add(new RowDefinition { Height = new GridLength(rowSpace) });
                        }
                        GridButtons.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    }
                    GridButtons.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Pixel) });
                    GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Pixel) });
                    for (int i = 0; i < columns; i++)
                    {
                        if (i != 0)
                        {
                            GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(columnSpace) });
                        }
                        GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    }
                    GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Pixel) });
                    break;
                default:
                    break;
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Button btn = new Button();
                    if (i * columns + j < balanceInquiryButtons.Count)
                    {
                        btn.FontSize = 44;
                        btn.Content = balanceInquiryButtons[i * columns + j];
                        btn.Style = FindResource("BrandButtonStyle") as Style;
                        btn.Click += BtnClick;
                    }
                    else btn.Visibility = Visibility.Hidden;

                    if (i == 0) Grid.SetRow(btn, i + 1);
                    else Grid.SetRow(btn, 2 * i + 1);

                    if (j == 0) Grid.SetColumn(btn, j + 1);
                    else Grid.SetColumn(btn, 2 * j + 1);

                    GridButtons.Children.Add(btn);
                }
            }
        }
    }
}
