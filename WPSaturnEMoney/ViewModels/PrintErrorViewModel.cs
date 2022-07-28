using Microsoft.PointOfService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;

namespace WPSaturnEMoney.ViewModels
{
    public class PrintErrorViewModel : BaseViewModel
    {
        private static ViewModel_emErrorToMaintenance _viewModel_emErrorToMaintenance = new ViewModel_emErrorToMaintenance();
        private static ViewModel_emLoginToMaintenance _viewModel_emMSorMT_PW = new ViewModel_emLoginToMaintenance();
        public static ViewModel_emMP_EMSG _viewModel_emMP_EMSG = new ViewModel_emMP_EMSG();

        public PrintErrorViewModel()
        {
            if (GlobalData.LastOperator == "0") // clerk operation
            {
                LoadScreen_emMP_EMSG();
            }
            else // customer operation
            {
                GlobalData.MPCurrentStep = Utilities.MPStep.Init;
                GlobalData.MPNextStep = Utilities.MPStep.emMT;
                GlobalData.LastOperator = "0"; // clerk operation from now
                Task taskPrinterMaintenance = Task.Run(async () =>
                {
                    do
                    {
                        if (GlobalData.MPCurrentStep != Utilities.MPStep.emMT && GlobalData.MPNextStep == Utilities.MPStep.emMT)
                        {
                            GlobalData.MPCurrentStep = Utilities.MPStep.emMT;
                            LoadScreen_emErrorToMaintenance();
                        }
                        else if (GlobalData.MPCurrentStep != Utilities.MPStep.emMT_PW && GlobalData.MPNextStep == Utilities.MPStep.emMT_PW)
                        {
                            GlobalData.MPCurrentStep = Utilities.MPStep.emMT_PW;
                            LoadScreen_emMSorMT_PW();
                        }
                        else if (GlobalData.MPCurrentStep != Utilities.MPStep.emMP_EMSG && GlobalData.MPNextStep == Utilities.MPStep.emMP_EMSG)
                        {
                            GlobalData.MPCurrentStep = Utilities.MPStep.emMP_EMSG;
                            LoadScreen_emMP_EMSG();
                        }
                        await Task.Delay(100);
                    }
                    while (GlobalData.IsInPrinterMaintenance);
                });
            }
        }

        private BaseViewModel _printErrorCurrentViewModel;
        public BaseViewModel PrintErrorCurrentViewModel
        {
            get => _printErrorCurrentViewModel;
            set
            {
                _printErrorCurrentViewModel = value;
                this.OnPropertyChanged("PrintErrorCurrentViewModel");
            }
        }
        public void LoadScreen_emErrorToMaintenance()
        {
            _viewModel_emErrorToMaintenance.HeaderTitle = State.Session.MainViewModel.HeaderTitle;
            _viewModel_emErrorToMaintenance.HeaderFooterColor = GlobalData.BasicConfig.color.common.base_color;
            _viewModel_emErrorToMaintenance.HeaderTitleColor = GlobalData.BasicConfig.color.common.title;
            _viewModel_emErrorToMaintenance.SignalBarColor = GlobalData.BasicConfig.color.common.signal_calling;
            _viewModel_emErrorToMaintenance.BackgroundColor = GlobalData.BasicConfig.color.customer.bg;
            _viewModel_emErrorToMaintenance.BodyTextColor = GlobalData.BasicConfig.color.customer.txt;

            GlobalData.LastOperator = "0"; // "0" clerk operation
            PrintErrorCurrentViewModel = _viewModel_emErrorToMaintenance;
            Utilities.Log.Info("Load screen emMT");
        }
        public void LoadScreen_emMSorMT_PW()
        {
            State.Session.MainViewModel.HeaderTitle = "電子マネー メンテナンス";
            _viewModel_emMSorMT_PW.HeaderTitle = State.Session.MainViewModel.HeaderTitle;
            _viewModel_emMSorMT_PW.HeaderFooterColor = GlobalData.BasicConfig.color.common.base_color;
            _viewModel_emMSorMT_PW.HeaderTitleColor = GlobalData.BasicConfig.color.common.title;
            _viewModel_emMSorMT_PW.SignalBarColor = GlobalData.BasicConfig.color.common.signal_normal;
            _viewModel_emMSorMT_PW.BackgroundColor = GlobalData.BasicConfig.color.maintenance.bg;
            _viewModel_emMSorMT_PW.TitleColor = GlobalData.BasicConfig.color.maintenance.title;
            _viewModel_emMSorMT_PW.BodyTextColor = GlobalData.BasicConfig.color.maintenance.txt;
            _viewModel_emMSorMT_PW.BorderLineColor = GlobalData.BasicConfig.color.maintenance.borderline;
            _viewModel_emMSorMT_PW.NumberColor = GlobalData.BasicConfig.color.maintenance.amount;
            _viewModel_emMSorMT_PW.ButtonBorderColor = GlobalData.BasicConfig.color.maintenance.btn_line;
            _viewModel_emMSorMT_PW.NumPadButtonColor = GlobalData.BasicConfig.color.maintenance.tenkey_btn;
            _viewModel_emMSorMT_PW.NumPadButtonLabelColor = GlobalData.BasicConfig.color.maintenance.tenkey_btn_label;
            _viewModel_emMSorMT_PW.ButtonPressedColor = GlobalData.BasicConfig.color.common.btn_touch;
            _viewModel_emMSorMT_PW.ButtonPressedLabelColor = GlobalData.BasicConfig.color.common.btn_touch_label;
            _viewModel_emMSorMT_PW.ClearButtonColor = GlobalData.BasicConfig.color.maintenance.clear_btn;
            _viewModel_emMSorMT_PW.ClearButtonLabelColor = GlobalData.BasicConfig.color.maintenance.clear_btn_label;

            _viewModel_emMSorMT_PW.InputCodeError = Utilities.GetErrorTypeAndMsg("application", "AE6").Item2;
            PrintErrorCurrentViewModel = _viewModel_emMSorMT_PW;
            Utilities.Log.Info("Load screen emMT_PW");
        }
        public void LoadScreen_emMP_EMSG()
        {
            _viewModel_emMP_EMSG.HeaderTitle = State.Session.MainViewModel.HeaderTitle;
            _viewModel_emMP_EMSG.HeaderFooterColor = GlobalData.BasicConfig.color.common.base_color;
            _viewModel_emMP_EMSG.HeaderTitleColor = GlobalData.BasicConfig.color.common.title;
            _viewModel_emMP_EMSG.SignalBarColor = GlobalData.BasicConfig.color.common.signal_caution;
            _viewModel_emMP_EMSG.BackgroundColor = GlobalData.BasicConfig.color.maintenance.bg;
            _viewModel_emMP_EMSG.TitleColor = GlobalData.BasicConfig.color.maintenance.title;
            _viewModel_emMP_EMSG.BodyTextColor = GlobalData.BasicConfig.color.maintenance.txt;
            _viewModel_emMP_EMSG.ButtonBorderColor = GlobalData.BasicConfig.color.maintenance.btn_line;
            _viewModel_emMP_EMSG.NormalButtonColor = GlobalData.BasicConfig.color.maintenance.normal_btn;
            _viewModel_emMP_EMSG.NormalButtonLabelColor = GlobalData.BasicConfig.color.maintenance.normal_btn_label;
            _viewModel_emMP_EMSG.BackButtonColor = GlobalData.BasicConfig.color.maintenance.back_btn;
            _viewModel_emMP_EMSG.BackButtonLabelColor = GlobalData.BasicConfig.color.maintenance.back_btn_label;

            if (GlobalData.MPErrorCode == ErrorCode.Extended && GlobalData.MPErrorCodeExtended == 201)
            {
                _viewModel_emMP_EMSG.ErrorMessage = Utilities.GetErrorTypeAndMsg("application", "AE9").Item2;
            }
            else if (GlobalData.MPErrorCode == ErrorCode.Extended && GlobalData.MPErrorCodeExtended == 203)
            {
                _viewModel_emMP_EMSG.ErrorMessage = Utilities.GetErrorTypeAndMsg("application", "AE10").Item2;
            }
            else
            {
                _viewModel_emMP_EMSG.ErrorMessage = Utilities.GetErrorTypeAndMsg("application", "AE7").Item2;
            }

            PrintErrorCurrentViewModel = _viewModel_emMP_EMSG;
            Utilities.Log.Info("Load screen emMP_EMSG");
            Utilities.Log.Info($"Displayed message: {_viewModel_emMP_EMSG.ErrorMessage}");
        }
        public override void UpdateView()
        {
        }
    }
}
