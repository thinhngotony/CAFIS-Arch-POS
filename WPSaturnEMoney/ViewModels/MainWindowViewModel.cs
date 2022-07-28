using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPSaturnEMoney.Models;
using WPSaturnEMoney.State;
using WPSaturnEMoney.Common;
using System.IO;
using Microsoft.PointOfService;

namespace WPSaturnEMoney.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public string AppName { get { return GlobalData.AppName; } }
        public string CustomerWindowName { get { return GlobalData.CustomerWindowName; } }
        public string NormalButtonColor { get; set; } = "#EDEDED";
        public string NormalButtonLabelColor { get; set; } = "#254474";
        public string ButtonBorderColor { get; set; } = "#254474";
        public string BodyTextColor { get; set; } = "#254474";

        private static ViewModel_emMessage _viewModel_emMessage = new ViewModel_emMessage();
        public static ViewModel_emPayment _viewModel_emPayment = new ViewModel_emPayment();
        private static ViewModel_emProcessOperation _viewModel_emPaymentOperation = new ViewModel_emProcessOperation();
        private static ViewModel_emF _viewModel_emF = new ViewModel_emF();
        private static ViewModel_emPaymentResult _viewModel_emPaymentResult = new ViewModel_emPaymentResult();
        private static ViewModel_emMAorMI_EMSG _viewModel_emMAorMI_EMSG = new ViewModel_emMAorMI_EMSG();
        private static ViewModel_emCM _viewModel_emCM = new ViewModel_emCM();
        private static ViewModel_emErrorToMaintenance _viewModel_emErrorToMaintenance = new ViewModel_emErrorToMaintenance();
        private static ViewModel_emLoginToMaintenance _viewModel_emMSorMT_PW = new ViewModel_emLoginToMaintenance();
        private static ViewModel_emMT_MSG _viewModel_emMT_MSG = new ViewModel_emMT_MSG();
        private static ViewModel_emMSorMT_TA _viewModel_emMSorMT_TA = new ViewModel_emMSorMT_TA();
        private static ViewModel_emMS_MENU _viewModel_emMS_MENU = new ViewModel_emMS_MENU();
        private static ViewModel_emES_MENU _viewModel_emES_MENU = new ViewModel_emES_MENU();
        private static ViewModel_emBalanceInquiry _viewModel_emZ = new ViewModel_emBalanceInquiry();
        private static ViewModel_emBalanceInquiryResult _viewModel_emZF = new ViewModel_emBalanceInquiryResult();
        private static ViewModel_emMaintenanceMenuFromError _viewModel_emMT_MENU = new ViewModel_emMaintenanceMenuFromError();
        private static ViewModel_emMT_RV _viewModel_emMT_RV = new ViewModel_emMT_RV();
        private static ViewModel_emMT_HV _viewModel_emMT_HV = new ViewModel_emMT_HV();
        private static ViewModel_emMT_TM _viewModel_emMT_TM = new ViewModel_emMT_TM();
        private static ViewModel_emMT_ZRESJ _viewModel_emMT_ZRESJ = new ViewModel_emMT_ZRESJ();
        private static ViewModel_emMT_HRESJ _viewModel_emMT_HRESJ = new ViewModel_emMT_HRESJ();
        private static ViewModel_emMT_EMSG _viewModel_emMT_EMSG = new ViewModel_emMT_EMSG();
        private static ViewModel_emMT_SMSG _viewModel_emMT_SMSG = new ViewModel_emMT_SMSG();
        private static ViewModel_emMT_ZJG _viewModel_emMT_ZJG = new ViewModel_emMT_ZJG();
        private static ViewModel_emMT_HJG _viewModel_emMT_HJG = new ViewModel_emMT_HJG();
        private static ViewModel_emMT_JGC1 _viewModel_emMT_JGC1 = new ViewModel_emMT_JGC1();
        private static ViewModel_emMT_JGC2 _viewModel_emMT_JGC2 = new ViewModel_emMT_JGC2();
        private static ViewModel_emMT_F _viewModel_emMT_F = new ViewModel_emMT_F();
        private static ViewModel_emM_TA _viewModel_emM_TA = new ViewModel_emM_TA();
        private static ViewModel_emMaintenanceMenu _viewModel_emM_MENU = new ViewModel_emMaintenanceMenu();
        private static ViewModel_emM_SMSG _viewModel_emM_SMSG = new ViewModel_emM_SMSG();
        private static ViewModel_emM_EMSG _viewModel_emM_EMSG = new ViewModel_emM_EMSG();
        private static ViewModel_emM_CH _viewModel_emM_CH = new ViewModel_emM_CH();
        private static ViewModel_emM_HRES _viewModel_emM_HRES = new ViewModel_emM_HRES();
        private static ViewModel_emM_RV _viewModel_emM_RV = new ViewModel_emM_RV();
        private static ViewModel_emM_TM _viewModel_emM_TM = new ViewModel_emM_TM();
        private static ViewModel_emM_ZRES _viewModel_emM_ZRES = new ViewModel_emM_ZRES();
        private static ViewModel_emM_ZRESJ _viewModel_emM_ZRESJ = new ViewModel_emM_ZRESJ();
        private static ViewModel_emM_HRESJ _viewModel_emM_HRESJ = new ViewModel_emM_HRESJ();
        private static ViewModel_emM_ZJG _viewModel_emM_ZJG = new ViewModel_emM_ZJG();
        private static ViewModel_emM_HJG _viewModel_emM_HJG = new ViewModel_emM_HJG();
        private static ViewModel_emM_JGC1 _viewModel_emM_JGC1 = new ViewModel_emM_JGC1();
        private static ViewModel_emM_JGC2 _viewModel_emM_JGC2 = new ViewModel_emM_JGC2();
        private static ViewModel_emM_TTLC _viewModel_emM_TTLC = new ViewModel_emM_TTLC();
        private static ViewModel_emM_F _viewModel_emM_F = new ViewModel_emM_F();

        // Signal type for signal bar color display on screen
        private enum Signal
        {
            Void,
            Normal,
            Complete,
            Caution,
            Error
        }

        /*private static BaseViewModel[] _customerScreenList = new BaseViewModel[]
        {
            _viewModel_emMessage, _viewModel_emPayment, _viewModel_emPaymentOperation, _viewModel_emF, _viewModel_emPaymentResult, 
            _viewModel_emCM, _viewModel_emZ, _viewModel_emZF, _viewModel_emErrorToMaintenance
        };*/

        private static string[] ErrorScreen = new string[]
        {
            "A0001", "A0002", "A0003", "A0004", "A0005", "B0001", "B0002", "B0003", "B0011", "B0012", "B0013", "D0002", "D0003", "D0004", "D0005", "D0006",
            "E0004", "E0005", "E0006", "E0009", "E0010", "E0011", "E0014", "F0001", "F0006", "F0007", "F0008", "F0011", "F0012"
        };
        private static string[] CautionScreen = new string[]
        {
            "E0003", "E0012", "E0013", "E0019", "F0005", "F0013", "F0015", "F0019", "F0020"
        };
        private static string[] CompleteScreen = new string[]
        {
            "D0008", "E0008", "E0018"
        };
        private static string[] VoidScreen = new string[]
        {
            "A0006", "D0001"
        };
        private static string[] NormalScreen = new string[]
        {
            "E0001", "E0002", "E0007", "E0015", "E0020", "E0021", "E0022", "F0003", "F0004", "F0009", "F0014", "F0021", "F0022", "F0023"
        };

        public MainWindowViewModel()
        {
            // Error message setting
            bool isMsgCommonOK = FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.Msg_Common_OldPath, ref GlobalData.MsgCommonOldConfig);
            /*bool isMsgSuicaOK = FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.Msg_SuicaPath, ref GlobalData.MsgSuicaConfig);
            bool isMsgiDOK = FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.Msg_iDPath, ref GlobalData.MsgiDConfig);
            bool isMsgWAONOK = FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.Msg_WAONPath, ref GlobalData.MsgWAONConfig);

            // Convert receipt setting
            bool isConvReceiptOK = FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.ConvReceiptPath, ref GlobalData.ConvReceiptConfig);

            // Check if reading file error
            if (!isMsgCommonOK || !isMsgSuicaOK || !isMsgiDOK || !isMsgWAONOK || !isConvReceiptOK)
            {
                if (!isMsgCommonOK)
                {
                    Utilities.Log.Error("▲ Cannot read Msg_Common_Old.json");
                    GlobalData.MsgCommonOldConfig.application = new Dictionary<string, FileStruct.msgDetail> {
                        { "AE8", new FileStruct.msgDetail { type = "1", msg = "[%code%] 電子マネーの設定が不備です。\n電子マネー機能は利用できません。" } }
                    };
                }
                if (!isMsgSuicaOK) Utilities.Log.Error("▲ Cannot read Msg_Suica.json");
                if (!isMsgiDOK) Utilities.Log.Error("▲ Cannot read Msg_iD.json");
                if (!isMsgWAONOK) Utilities.Log.Error("▲ Cannot read Msg_WAON.json");
                if (!isConvReceiptOK) Utilities.Log.Error("▲ Cannot read ConvReceipt.json");
                GlobalData.ApplicationError = "AE8";
                GlobalData.ApplicationErrorMsg = Utilities.GetErrorTypeAndMsg("application", GlobalData.ApplicationError).Item2;
            }

            // Read total.json if exist
            if (File.Exists(GlobalData.AppPath + GlobalData.TotalPath))
            {
                FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.TotalPath, ref GlobalData.TotalInfo);
            }

            // Read and check app setting
            FileStruct.SettingReadAndCheck();*/

            Session.MainViewModel = this;
            Session.ScreenState = new StateMachine() { CurrentState = StateMachine.State.InitApp };
            Session.ScreenState.AsyncStart(new State_emInit());
        }

        // CurrentViewModel that is currently bound to the ContentControl
        private BaseViewModel _currentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged("CurrentViewModel");
            }
        }

        // CurrentCustomerViewModel that is currently bound to the ContentControl
        private BaseViewModel _currentCustomerViewModel;
        public BaseViewModel CurrentCustomerViewModel
        {
            get => _currentCustomerViewModel;
            set
            {
                _currentCustomerViewModel = value;
                OnPropertyChanged("CurrentCustomerViewModel");
            }
        }

        private string GetHeaderFooterColor()
        {
            return GlobalData.BasicConfig.color.common.base_color;
        }
        private string GetHeaderTitleColor()
        {
            return GlobalData.BasicConfig.color.common.title;
        }
        private string GetSignalBarColor(Signal signal)
        {
            switch (signal)
            {
                case Signal.Normal:
                    return GlobalData.BasicConfig.color.common.signal_normal;
                case Signal.Complete:
                    return GlobalData.BasicConfig.color.common.signal_complete;
                case Signal.Caution:
                    return GlobalData.BasicConfig.color.common.signal_caution;
                case Signal.Error:
                    return GlobalData.BasicConfig.color.common.signal_calling;
                default:
                    return GlobalData.BasicConfig.color.common.signal_void;
            }
        }
        private string GetBackgroundColor(bool? isForcedCustomer = null)
        {
            if (isForcedCustomer != null)
                return !(bool)isForcedCustomer
                    ? GlobalData.BasicConfig.color.maintenance.bg
                    : GlobalData.BasicConfig.color.customer.bg;
            else
                return Session.MaintenanceMode == "Maintenance" || Session.MaintenanceMode == "MaintenanceFromError"
                    ? GlobalData.BasicConfig.color.maintenance.bg
                    : GlobalData.BasicConfig.color.customer.bg;
        }
        private string GetTitleColor(bool? isForcedCustomer = null)
        {
            if (isForcedCustomer != null)
                return !(bool)isForcedCustomer
                    ? GlobalData.BasicConfig.color.maintenance.title
                    : GlobalData.BasicConfig.color.customer.title;
            else
                return Session.MaintenanceMode == "Maintenance" || Session.MaintenanceMode == "MaintenanceFromError"
                    ? GlobalData.BasicConfig.color.maintenance.title
                    : GlobalData.BasicConfig.color.customer.title;
        }
        private string GetBodyTextColor(bool? isForcedCustomer = null)
        {
            if (isForcedCustomer != null)
                return !(bool)isForcedCustomer
                    ? GlobalData.BasicConfig.color.maintenance.txt
                    : GlobalData.BasicConfig.color.customer.txt;
            else
                return Session.MaintenanceMode == "Maintenance" || Session.MaintenanceMode == "MaintenanceFromError"
                    ? GlobalData.BasicConfig.color.maintenance.txt
                    : GlobalData.BasicConfig.color.customer.txt;
        }
        private string GetNumberColor(bool? isForcedCustomer = null)
        {
            if (isForcedCustomer != null)
                return !(bool)isForcedCustomer
                    ? GlobalData.BasicConfig.color.maintenance.amount
                    : GlobalData.BasicConfig.color.customer.amount;
            else
                return Session.MaintenanceMode == "Maintenance" || Session.MaintenanceMode == "MaintenanceFromError"
                    ? GlobalData.BasicConfig.color.maintenance.amount
                    : GlobalData.BasicConfig.color.customer.amount;
        }
        private string GetBorderLineColor(bool? isForcedCustomer = null)
        {
            if (isForcedCustomer != null)
                return !(bool)isForcedCustomer
                    ? GlobalData.BasicConfig.color.maintenance.borderline
                    : GlobalData.BasicConfig.color.customer.borderline;
            else
                return Session.MaintenanceMode == "Maintenance" || Session.MaintenanceMode == "MaintenanceFromError"
                    ? GlobalData.BasicConfig.color.maintenance.borderline
                    : GlobalData.BasicConfig.color.customer.borderline;
        }
        private string GetBtnBorderColor(bool? isForcedCustomer = null)
        {
            if (isForcedCustomer != null)
                return !(bool)isForcedCustomer
                    ? GlobalData.BasicConfig.color.maintenance.btn_line
                    : GlobalData.BasicConfig.color.customer.btn_line;
            else
                return Session.MaintenanceMode == "Maintenance" || Session.MaintenanceMode == "MaintenanceFromError"
                    ? GlobalData.BasicConfig.color.maintenance.btn_line
                    : GlobalData.BasicConfig.color.customer.btn_line;
        }
        private string GetBrandBtnColor(bool? isForcedCustomer = null)
        {
            if (isForcedCustomer != null)
                return !(bool)isForcedCustomer
                    ? GlobalData.BasicConfig.color.maintenance.brand_btn
                    : GlobalData.BasicConfig.color.customer.brand_btn;
            else
                return Session.MaintenanceMode == "Maintenance" || Session.MaintenanceMode == "MaintenanceFromError"
                    ? GlobalData.BasicConfig.color.maintenance.brand_btn
                    : GlobalData.BasicConfig.color.customer.brand_btn;
        }
        private string GetBrandBtnLabelColor(bool? isForcedCustomer = null)
        {
            if (isForcedCustomer != null)
                return !(bool)isForcedCustomer
                    ? GlobalData.BasicConfig.color.maintenance.brand_btn_label
                    : GlobalData.BasicConfig.color.customer.brand_btn_label;
            else
                return Session.MaintenanceMode == "Maintenance" || Session.MaintenanceMode == "MaintenanceFromError"
                    ? GlobalData.BasicConfig.color.maintenance.brand_btn_label
                    : GlobalData.BasicConfig.color.customer.brand_btn_label;
        }
        private string GetNormalBtnColor(bool? isForcedCustomer = null)
        {
            if (isForcedCustomer != null)
                return !(bool)isForcedCustomer
                    ? GlobalData.BasicConfig.color.maintenance.normal_btn
                    : GlobalData.BasicConfig.color.customer.normal_btn;
            else
                return Session.MaintenanceMode == "Maintenance" || Session.MaintenanceMode == "MaintenanceFromError"
                    ? GlobalData.BasicConfig.color.maintenance.normal_btn
                    : GlobalData.BasicConfig.color.customer.normal_btn;
        }
        private string GetNormalBtnLabelColor(bool? isForcedCustomer = null)
        {
            if (isForcedCustomer != null)
                return !(bool)isForcedCustomer
                    ? GlobalData.BasicConfig.color.maintenance.normal_btn_label
                    : GlobalData.BasicConfig.color.customer.normal_btn_label;
            else
                return Session.MaintenanceMode == "Maintenance" || Session.MaintenanceMode == "MaintenanceFromError"
                    ? GlobalData.BasicConfig.color.maintenance.normal_btn_label
                    : GlobalData.BasicConfig.color.customer.normal_btn_label;
        }
        private string GetBackBtnColor(bool? isForcedCustomer = null)
        {
            if (isForcedCustomer != null)
                return !(bool)isForcedCustomer
                    ? GlobalData.BasicConfig.color.maintenance.back_btn
                    : GlobalData.BasicConfig.color.customer.back_btn;
            else
                return Session.MaintenanceMode == "Maintenance" || Session.MaintenanceMode == "MaintenanceFromError"
                    ? GlobalData.BasicConfig.color.maintenance.back_btn
                    : GlobalData.BasicConfig.color.customer.back_btn;
        }
        private string GetBackBtnLabelColor(bool? isForcedCustomer = null)
        {
            if (isForcedCustomer != null)
                return !(bool)isForcedCustomer
                    ? GlobalData.BasicConfig.color.maintenance.back_btn_label
                    : GlobalData.BasicConfig.color.customer.back_btn_label;
            else
                return Session.MaintenanceMode == "Maintenance" || Session.MaintenanceMode == "MaintenanceFromError"
                    ? GlobalData.BasicConfig.color.maintenance.back_btn_label
                    : GlobalData.BasicConfig.color.customer.back_btn_label;
        }
        private string GetNumPadBtnColor()
        {
            return GlobalData.BasicConfig.color.maintenance.tenkey_btn;
        }
        private string GetNumPadBtnLabelColor()
        {
            return GlobalData.BasicConfig.color.maintenance.tenkey_btn_label;
        }
        private string GetBtnPressedColor()
        {
            return GlobalData.BasicConfig.color.common.btn_touch;
        }
        private string GetBtnPressedLabelColor()
        {
            return GlobalData.BasicConfig.color.common.btn_touch_label;
        }
        private string GetClearBtnColor()
        {
            return GlobalData.BasicConfig.color.maintenance.clear_btn;
        }
        private string GetClearBtnLabelColor()
        {
            return GlobalData.BasicConfig.color.maintenance.clear_btn_label;
        }
        private string GetAttentionTxtColor()
        {
            return GlobalData.BasicConfig.color.maintenance.txt_attention;
        }
        private string GetAttentionTitle(BaseViewModel viewModel)
        {
            if (viewModel.Equals(_viewModel_emM_JGC1))
                return GlobalData.BasicConfig.screen_msg.m_jgc1_title;
            else if (viewModel.Equals(_viewModel_emM_JGC2))
                return GlobalData.BasicConfig.screen_msg.m_jgc2_title;
            else if (viewModel.Equals(_viewModel_emM_TTLC))
                return GlobalData.BasicConfig.screen_msg.m_ttlc_title;
            else if (viewModel.Equals(_viewModel_emMT_JGC1))
                return GlobalData.BasicConfig.screen_msg.mt_jgc1_title;
            else if (viewModel.Equals(_viewModel_emMT_JGC2))
                return GlobalData.BasicConfig.screen_msg.mt_jgc2_title;
            else
                return "";
        }
        private string GetBodyText(BaseViewModel viewModel)
        {
            if (viewModel.Equals(_viewModel_emM_JGC1))
                return GlobalData.BasicConfig.screen_msg.m_jgc1_body;
            else if (viewModel.Equals(_viewModel_emM_JGC2))
                return GlobalData.BasicConfig.screen_msg.m_jgc2_body;
            else if (viewModel.Equals(_viewModel_emM_TTLC))
                return GlobalData.BasicConfig.screen_msg.m_ttlc_body;
            else if (viewModel.Equals(_viewModel_emMT_JGC1))
                return GlobalData.BasicConfig.screen_msg.mt_jgc1_body;
            else if (viewModel.Equals(_viewModel_emMT_JGC2))
                return GlobalData.BasicConfig.screen_msg.mt_jgc2_body;
            else
                return "";
        }

        private void LoadScreenCommon(BaseViewModel viewModel, Signal signalType)
        {
            // viewModel.HeaderTitle = HeaderTitle;
            viewModel.HeaderFooterColor = GetHeaderFooterColor();
            viewModel.HeaderTitleColor = GetHeaderTitleColor();
            viewModel.SignalBarColor = GetSignalBarColor(signalType);
            viewModel.BackgroundColor = GetBackgroundColor();
        }

        private void GetListHeaderTitleAndMessage(FileStruct.MsgMode msgMode, string brandName)
        {
            string[] separator = { "\t" };
            GlobalData.ViewModelProperties.ListHeaderTitle = msgMode.header_title1.Split(separator, StringSplitOptions.None).ToList();
            GlobalData.ViewModelProperties.ListHeaderTitle[0] = GlobalData.ViewModelProperties.ListHeaderTitle[0].Replace("[ブランド名]", brandName);
            GlobalData.ViewModelProperties.ListHeaderTitle[1] = GlobalData.ViewModelProperties.ListHeaderTitle[1].Replace("[金額]", string.Format("{0:N0}", GlobalData.TotalPayment));
            GlobalData.ViewModelProperties.ListHeaderTitleMaintenance = msgMode.header_title2.Split(separator, StringSplitOptions.None).ToList();
            GlobalData.ViewModelProperties.ListHeaderTitleMaintenance[0] = GlobalData.ViewModelProperties.ListHeaderTitleMaintenance[0].Replace("[ブランド名]", brandName);
            GlobalData.ViewModelProperties.ListHeaderTitleMaintenance[1] = GlobalData.ViewModelProperties.ListHeaderTitleMaintenance[1].Replace("[金額]", string.Format("{0:N0}", GlobalData.TotalPayment));
            GlobalData.ViewModelProperties.ListMessage = msgMode.msg1.Split(separator, StringSplitOptions.None).ToList();

            GlobalData.CustomerViewModelProperties.ListHeaderTitle = msgMode.header_title3.Split(separator, StringSplitOptions.None).ToList();
            GlobalData.CustomerViewModelProperties.ListHeaderTitle[0] = GlobalData.CustomerViewModelProperties.ListHeaderTitle[0].Replace("[ブランド名]", brandName);
            GlobalData.CustomerViewModelProperties.ListHeaderTitle[1] = GlobalData.CustomerViewModelProperties.ListHeaderTitle[1].Replace("[金額]", string.Format("{0:N0}", GlobalData.TotalPayment));
            GlobalData.CustomerViewModelProperties.ListHeaderTitleMaintenance = msgMode.header_title4.Split(separator, StringSplitOptions.None).ToList();
            GlobalData.CustomerViewModelProperties.ListHeaderTitleMaintenance[0] = GlobalData.CustomerViewModelProperties.ListHeaderTitleMaintenance[0].Replace("[ブランド名]", brandName);
            GlobalData.CustomerViewModelProperties.ListHeaderTitleMaintenance[1] = GlobalData.CustomerViewModelProperties.ListHeaderTitleMaintenance[1].Replace("[金額]", string.Format("{0:N0}", GlobalData.TotalPayment));
            GlobalData.CustomerViewModelProperties.ListMessage = msgMode.msg3.Split(separator, StringSplitOptions.None).ToList();
        }

        public void LoadScreen_emMessage(string msgCode = "")
        {
            string brandName = "";
            string signalBarColor = "";
            FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
            if (GlobalData.ServiceName != null && GlobalData.BasicConfig.brand_name.Contains(GlobalData.ServiceName))
                brandName = Utilities.GetBrandName(GlobalData.ServiceName);
            GetListHeaderTitleAndMessage(msgMode, brandName);
            if (ErrorScreen.Contains(msgCode))
            {
                signalBarColor = GetSignalBarColor(Signal.Error);
            }
            else if (CautionScreen.Contains(msgCode))
            {
                signalBarColor = GetSignalBarColor(Signal.Caution);
            }
            else if (CompleteScreen.Contains(msgCode))
            {
                signalBarColor = GetSignalBarColor(Signal.Complete);
            }
            else if (NormalScreen.Contains(msgCode))
            {
                signalBarColor = GetSignalBarColor(Signal.Normal);
            }
            else // if (VoidScreen.Contains(msgCode))
            {
                signalBarColor = GetSignalBarColor(Signal.Void);
            }
            ViewModel_emMessage viewModel_emMessage = new ViewModel_emMessage
            {
                ListHeaderTitle = Session.MaintenanceMode == "Maintenance" || Session.MaintenanceMode == "MaintenanceFromError" ? GlobalData.ViewModelProperties.ListHeaderTitleMaintenance : GlobalData.ViewModelProperties.ListHeaderTitle,
                Message = string.IsNullOrEmpty(GlobalData.ViewModelProperties.Message) ? msgMode.msg1 : GlobalData.ViewModelProperties.Message,
                SignalBarColor = signalBarColor,
                HeaderFooterColor = GetHeaderFooterColor(),
                HeaderTitleColor = GetHeaderTitleColor(),
                BackgroundColor = GetBackgroundColor(),
                BodyTextColor = GetBodyTextColor(),
                ButtonBorderColor = GetBtnBorderColor(),
                NormalButtonColor = GetNormalBtnColor(),
                NormalButtonLabelColor = GetNormalBtnLabelColor(),
                BtnVisibility = GlobalData.ViewModelProperties.BtnVisibility,
                BtnContent = GlobalData.ViewModelProperties.BtnContent,
                BtnCommand = GlobalData.ViewModelProperties.BtnCommand,
                Btn2ndVisibility = GlobalData.ViewModelProperties.Btn2ndVisibility,
                Btn2ndContent = GlobalData.ViewModelProperties.Btn2ndContent,
                Btn2ndCommand = GlobalData.ViewModelProperties.Btn2ndCommand,
            };
            CurrentViewModel = viewModel_emMessage;

            if (GlobalData.BasicConfig.customer_display_flag == "1")
            {
                ViewModel_emMessage customerViewModel_emMessage = new ViewModel_emMessage
                {
                    ListHeaderTitle = Session.MaintenanceMode == "Maintenance" || Session.MaintenanceMode == "MaintenanceFromError" ? GlobalData.CustomerViewModelProperties.ListHeaderTitleMaintenance : GlobalData.CustomerViewModelProperties.ListHeaderTitle,
                    Message = string.IsNullOrEmpty(GlobalData.CustomerViewModelProperties.Message) ? msgMode.msg3 : GlobalData.CustomerViewModelProperties.Message,
                    SignalBarColor = signalBarColor,
                    HeaderFooterColor = GetHeaderFooterColor(),
                    HeaderTitleColor = GetHeaderTitleColor(),
                    BackgroundColor = GetBackgroundColor(true),
                    BodyTextColor = GetBodyTextColor(true),
                    ButtonBorderColor = GetBtnBorderColor(true),
                    NormalButtonColor = GetNormalBtnColor(true),
                    NormalButtonLabelColor = GetNormalBtnLabelColor(true),
                    BtnVisibility = GlobalData.CustomerViewModelProperties.BtnVisibility,
                    BtnContent = GlobalData.CustomerViewModelProperties.BtnContent,
                    BtnCommand = GlobalData.CustomerViewModelProperties.BtnCommand
                };
                CurrentCustomerViewModel = customerViewModel_emMessage;
            }
            Utilities.Log.Info("Load screen emMessage with " + msgCode);
        }
        public void LoadScreen_emPayment(string msgCode = "")
        {
            string brandName = "";
            FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
            if (GlobalData.ServiceName != null && GlobalData.BasicConfig.brand_name.Contains(GlobalData.ServiceName))
                brandName = Utilities.GetBrandName(GlobalData.ServiceName);
            GetListHeaderTitleAndMessage(msgMode, brandName);

            GlobalData.LastOperator = "1"; // customer operation

            ViewModel_emPayment viewModel_emPayment = new ViewModel_emPayment
            {
                ListHeaderTitle = GlobalData.ViewModelProperties.ListHeaderTitle,
                SignalBarColor = GetSignalBarColor(Signal.Normal),
                HeaderFooterColor = GetHeaderFooterColor(),
                HeaderTitleColor = GetHeaderTitleColor(),
                BackgroundColor = GetBackgroundColor(),
                TitleColor = GetTitleColor(),
                ButtonBorderColor = GetBtnBorderColor(),
                BrandButtonColor = GetBrandBtnColor(),
                BrandButtonLabelColor = GetBrandBtnLabelColor(),
                NormalButtonColor = GetNormalBtnColor(),
                NormalButtonLabelColor = GetNormalBtnLabelColor(),
                BackButtonColor = GetBackBtnColor(),
                BackButtonLabelColor = GetBackBtnLabelColor(),
                TotalPayment = string.Format("{0:N0}", GlobalData.TotalPayment) + "円",
            };
            CurrentViewModel = viewModel_emPayment;

            if (GlobalData.BasicConfig.customer_display_flag == "1")
            {
                ViewModel_emMessage customerViewModel_emMessage = new ViewModel_emMessage
                {
                    ListHeaderTitle = GlobalData.CustomerViewModelProperties.ListHeaderTitle,
                    Message = msgMode.msg3,
                    SignalBarColor = GetSignalBarColor(Signal.Void),
                    HeaderFooterColor = GetHeaderFooterColor(),
                    HeaderTitleColor = GetHeaderTitleColor(),
                    BackgroundColor = GetBackgroundColor(true),
                    BodyTextColor = GetBodyTextColor(true),
                };
                CurrentCustomerViewModel = customerViewModel_emMessage;
            }
            Utilities.Log.Info("Load screen emPayment with " + msgCode);
        }
        public void LoadScreen_emProcessOperation(string msgCode = "")
        {
            string brandName = "";
            FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
            if (GlobalData.ServiceName != null && GlobalData.BasicConfig.brand_name.Contains(GlobalData.ServiceName))
                brandName = Utilities.GetBrandName(GlobalData.ServiceName);
            GetListHeaderTitleAndMessage(msgMode, brandName);
            if (GlobalData.ViewModelProperties.ListMessage.Count < 2)
            {
                var temp = GlobalData.ViewModelProperties.ListMessage.ToList();
                temp.AddRange(Enumerable.Repeat("", 2 - GlobalData.ViewModelProperties.ListMessage.Count));
                GlobalData.ViewModelProperties.ListMessage = temp;
            }
            if (GlobalData.CustomerViewModelProperties.ListMessage.Count < 2)
            {
                var temp = GlobalData.CustomerViewModelProperties.ListMessage.ToList();
                temp.AddRange(Enumerable.Repeat("", 2 - GlobalData.CustomerViewModelProperties.ListMessage.Count));
                GlobalData.CustomerViewModelProperties.ListMessage = temp;
            }
            ViewModel_emProcessOperation viewModel_emProcessOperation = new ViewModel_emProcessOperation
            {
                SignalBarColor = GetSignalBarColor(Signal.Normal),
                HeaderFooterColor = GetHeaderFooterColor(),
                HeaderTitleColor = GetHeaderTitleColor(),
                BackgroundColor = GetBackgroundColor(),
                ButtonBorderColor = GetBtnBorderColor(),
                BackButtonColor = GetBackBtnColor(),
                BackButtonLabelColor = GetBackBtnLabelColor(),
                BodyTextColor = GetBodyTextColor(),
                ListHeaderTitle = GlobalData.ViewModelProperties.ListHeaderTitle,
                ListMessage = GlobalData.ViewModelProperties.ListMessage,
                BtnVisibility = GlobalData.ViewModelProperties.BtnVisibility,
                BtnContent = GlobalData.ViewModelProperties.BtnContent,
                BtnCommand = GlobalData.ViewModelProperties.BtnCommand,
            };
            CurrentViewModel = viewModel_emProcessOperation;

            if (GlobalData.BasicConfig.customer_display_flag == "1")
            {
                ViewModel_emProcessOperation customerViewModel_emProcessOperation = new ViewModel_emProcessOperation
                {
                    SignalBarColor = GetSignalBarColor(Signal.Normal),
                    HeaderFooterColor = GetHeaderFooterColor(),
                    HeaderTitleColor = GetHeaderTitleColor(),
                    BackgroundColor = GetBackgroundColor(true),
                    ButtonBorderColor = GetBtnBorderColor(true),
                    BackButtonColor = GetBackBtnColor(true),
                    BackButtonLabelColor = GetBackBtnLabelColor(true),
                    BodyTextColor = GetBodyTextColor(true),
                    ListHeaderTitle = GlobalData.CustomerViewModelProperties.ListHeaderTitle,
                    ListMessage = GlobalData.CustomerViewModelProperties.ListMessage,
                };
                CurrentCustomerViewModel = customerViewModel_emProcessOperation;
            }

            Utilities.Log.Info("Load screen emProcessOperation with " + msgCode);
        }
        public void LoadScreen_emPaymentResult(string msgCode = "")
        {
            string brandName = "";
            decimal.TryParse(GlobalData.SaturnAPIResponse.bizInfo.tradeAmount, out decimal paymentSettlementAmount);
            decimal remainingAmount = GlobalData.TotalPayment - paymentSettlementAmount;
            FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
            if (GlobalData.ServiceName != null && GlobalData.BasicConfig.brand_name.Contains(GlobalData.ServiceName))
                brandName = Utilities.GetBrandName(GlobalData.ServiceName);
            GetListHeaderTitleAndMessage(msgMode, brandName);
            if (GlobalData.ViewModelProperties.ListMessage.Count < 2)
            {
                var temp = GlobalData.ViewModelProperties.ListMessage.ToList();
                temp.AddRange(Enumerable.Repeat("", 2 - GlobalData.ViewModelProperties.ListMessage.Count));
                GlobalData.ViewModelProperties.ListMessage = temp;
            }
            if (GlobalData.CustomerViewModelProperties.ListMessage.Count < 2)
            {
                var temp = GlobalData.CustomerViewModelProperties.ListMessage.ToList();
                temp.AddRange(Enumerable.Repeat("", 2 - GlobalData.CustomerViewModelProperties.ListMessage.Count));
                GlobalData.CustomerViewModelProperties.ListMessage = temp;
            }
            ViewModel_emPaymentResult viewModel_emPaymentResult = new ViewModel_emPaymentResult
            {
                ListHeaderTitle = GlobalData.ViewModelProperties.ListHeaderTitle,
                ListMessage = GlobalData.ViewModelProperties.ListMessage,
                SignalBarColor = GetSignalBarColor(Signal.Complete),
                HeaderFooterColor = GetHeaderFooterColor(),
                HeaderTitleColor = GetHeaderTitleColor(),
                BackgroundColor = GetBackgroundColor(),
                ButtonBorderColor = GetBtnBorderColor(),
                NormalButtonColor = GetNormalBtnColor(),
                NormalButtonLabelColor = GetNormalBtnLabelColor(),
                BodyTextColor = GetBodyTextColor(),
                BtnVisibility = GlobalData.ViewModelProperties.BtnVisibility,
                BtnContent = GlobalData.ViewModelProperties.BtnContent,
                PaymentRequestAmount = string.Format("{0:N0}", GlobalData.TotalPayment) + "円",
                PaymentSettlementAmount = string.Format("{0:N0}", paymentSettlementAmount) + "円",
                RemainingAmount = string.Format("{0:N0}", remainingAmount) + "円",
            };
            CurrentViewModel = viewModel_emPaymentResult;

            if (GlobalData.BasicConfig.customer_display_flag == "1")
            {
                ViewModel_emPaymentResult customerViewModel_emStandBy = new ViewModel_emPaymentResult
                {
                    ListHeaderTitle = GlobalData.CustomerViewModelProperties.ListHeaderTitle,
                    ListMessage = GlobalData.CustomerViewModelProperties.ListMessage,
                    SignalBarColor = GetSignalBarColor(Signal.Complete),
                    HeaderFooterColor = GetHeaderFooterColor(),
                    HeaderTitleColor = GetHeaderTitleColor(),
                    BackgroundColor = GetBackgroundColor(true),
                    ButtonBorderColor = GetBtnBorderColor(true),
                    NormalButtonColor = GetNormalBtnColor(true),
                    NormalButtonLabelColor = GetNormalBtnLabelColor(true),
                    BodyTextColor = GetBodyTextColor(true),
                    BtnVisibility = GlobalData.CustomerViewModelProperties.BtnVisibility,
                    BtnContent = GlobalData.CustomerViewModelProperties.BtnContent,
                    PaymentRequestAmount = string.Format("{0:N0}", GlobalData.TotalPayment) + "円",
                    PaymentSettlementAmount = string.Format("{0:N0}", paymentSettlementAmount) + "円",
                    RemainingAmount = string.Format("{0:N0}", remainingAmount) + "円",
                };
                CurrentCustomerViewModel = customerViewModel_emStandBy;
            }
            Utilities.Log.Info("Load screen emPaymentResult!");
        }
        public void LoadScreen_emBalanceInquiry(string msgCode = "")
        {
            string brandName = "";
            FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
            if (GlobalData.ServiceName != null && GlobalData.BasicConfig.brand_name.Contains(GlobalData.ServiceName))
                brandName = Utilities.GetBrandName(GlobalData.ServiceName);
            GetListHeaderTitleAndMessage(msgMode, brandName);

            GlobalData.LastOperator = "1"; // customer operation

            ViewModel_emBalanceInquiry viewModel_emBalanceInquiry = new ViewModel_emBalanceInquiry
            {
                ListHeaderTitle = GlobalData.ViewModelProperties.ListHeaderTitle,
                SignalBarColor = GetSignalBarColor(Signal.Normal),
                HeaderFooterColor = GetHeaderFooterColor(),
                HeaderTitleColor = GetHeaderTitleColor(),
                BackgroundColor = GetBackgroundColor(),
                TitleColor = GetTitleColor(),
                ButtonBorderColor = GetBtnBorderColor(),
                BrandButtonColor = GetBrandBtnColor(),
                BrandButtonLabelColor = GetBrandBtnLabelColor(),
                BackButtonColor = GetBackBtnColor(),
                BackButtonLabelColor = GetBackBtnLabelColor(),
                TotalPayment = string.Format("{0:N0}", GlobalData.TotalPayment) + "円",
            };
            CurrentViewModel = viewModel_emBalanceInquiry;

            if (GlobalData.BasicConfig.customer_display_flag == "1")
            {
                ViewModel_emMessage customerViewModel_emMessage = new ViewModel_emMessage
                {
                    ListHeaderTitle = GlobalData.CustomerViewModelProperties.ListHeaderTitle,
                    Message = msgMode.msg3,
                    SignalBarColor = GetSignalBarColor(Signal.Void),
                    HeaderFooterColor = GetHeaderFooterColor(),
                    HeaderTitleColor = GetHeaderTitleColor(),
                    BackgroundColor = GetBackgroundColor(true),
                    BodyTextColor = GetBodyTextColor(true),
                };
                CurrentCustomerViewModel = customerViewModel_emMessage;
            }
            Utilities.Log.Info("Load screen emBalanceInquiry with " + msgCode);
        }
        public void LoadScreen_emBalanceInquiryResult(string msgCode = "")
        {
            string brandName = "";
            FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
            if (GlobalData.ServiceName != null && GlobalData.BasicConfig.brand_name.Contains(GlobalData.ServiceName))
                brandName = Utilities.GetBrandName(GlobalData.ServiceName);
            GetListHeaderTitleAndMessage(msgMode, brandName);
            ViewModel_emBalanceInquiryResult ViewModel_emBalanceInquiryResult = new ViewModel_emBalanceInquiryResult
            {
                SignalBarColor = GetSignalBarColor(Signal.Normal),
                HeaderFooterColor = GetHeaderFooterColor(),
                HeaderTitleColor = GetHeaderTitleColor(),
                BackgroundColor = GetBackgroundColor(),
                BodyTextColor = GetBodyTextColor(),
                TitleColor = GetTitleColor(),
                ButtonBorderColor = GetBtnBorderColor(),
                NormalButtonColor = GetNormalBtnColor(),
                NormalButtonLabelColor = GetNormalBtnLabelColor(),
                NumberColor = GetNumberColor(),
                BorderLineColor = GetBorderLineColor(),
                BrandName = brandName,
                Balance = string.Format("{0:N0}", GlobalData.Balance) + "円",
                TotalPayment = string.Format("{0:N0}", GlobalData.TotalPayment) + "円",
                Message = msgMode.msg1,
                MessageVisibility = GlobalData.ViewModelProperties.MessageVisibility,
                BtnVisibility = GlobalData.ViewModelProperties.BtnVisibility,
                Btn2ndVisibility = GlobalData.ViewModelProperties.Btn2ndVisibility,
                Btn2ndContent = GlobalData.ViewModelProperties.Btn2ndContent,
                Btn2ndCommand = GlobalData.ViewModelProperties.Btn2ndCommand,
            };
            CurrentViewModel = ViewModel_emBalanceInquiryResult;

            if (GlobalData.BasicConfig.customer_display_flag == "1")
            {
                ViewModel_emBalanceInquiryResult customerViewModel_emBalanceInquiryResult = new ViewModel_emBalanceInquiryResult
                {
                    SignalBarColor = GetSignalBarColor(Signal.Normal),
                    HeaderFooterColor = GetHeaderFooterColor(),
                    HeaderTitleColor = GetHeaderTitleColor(),
                    BackgroundColor = GetBackgroundColor(true),
                    BodyTextColor = GetBodyTextColor(true),
                    TitleColor = GetTitleColor(true),
                    ButtonBorderColor = GetBtnBorderColor(true),
                    NormalButtonColor = GetNormalBtnColor(true),
                    NormalButtonLabelColor = GetNormalBtnLabelColor(true),
                    NumberColor = GetNumberColor(true),
                    BorderLineColor = GetBorderLineColor(true),
                    BrandName = brandName,
                    Balance = string.Format("{0:N0}", GlobalData.Balance) + "円",
                    TotalPayment = string.Format("{0:N0}", GlobalData.TotalPayment) + "円",
                };
                CurrentCustomerViewModel = customerViewModel_emBalanceInquiryResult;
            }
            Utilities.Log.Info("Load screen emBalanceInquiryResult with " + msgCode);
        }
        public void LoadScreen_emErrorToMaintenance(string msgCode = "")
        {
            string brandName = "";
            string signalBarColor = "";
            FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
            if (GlobalData.ServiceName != null && GlobalData.BasicConfig.brand_name.Contains(GlobalData.ServiceName))
                brandName = Utilities.GetBrandName(GlobalData.ServiceName);
            GetListHeaderTitleAndMessage(msgMode, brandName);
            if (ErrorScreen.Contains(msgCode))
            {
                signalBarColor = GetSignalBarColor(Signal.Error);
            }
            else if (CautionScreen.Contains(msgCode))
            {
                signalBarColor = GetSignalBarColor(Signal.Caution);
            }
            else if (CompleteScreen.Contains(msgCode))
            {
                signalBarColor = GetSignalBarColor(Signal.Complete);
            }
            else if (NormalScreen.Contains(msgCode))
            {
                signalBarColor = GetSignalBarColor(Signal.Normal);
            }
            else // if (VoidScreen.Contains(msgCode))
            {
                signalBarColor = GetSignalBarColor(Signal.Void);
            }

            GlobalData.LastOperator = "0"; // "0" clerk operation

            ViewModel_emErrorToMaintenance viewModel_emErrorToMaintenance = new ViewModel_emErrorToMaintenance
            {
                ListHeaderTitle = GlobalData.ViewModelProperties.ListHeaderTitle,
                HeaderFooterColor = GetHeaderFooterColor(),
                HeaderTitleColor = GetHeaderTitleColor(),
                BackgroundColor = GetBackgroundColor(),
                BodyTextColor = GetBodyTextColor(),
                ButtonBorderColor = GetBtnBorderColor(),
                NormalButtonColor = GetNormalBtnColor(),
                NormalButtonLabelColor = GetNormalBtnLabelColor(),
                SignalBarColor = signalBarColor,
                Message = string.IsNullOrEmpty(GlobalData.CustomerViewModelProperties.Message) ? msgMode.msg1 : GlobalData.CustomerViewModelProperties.Message,
                BtnVisibility = GlobalData.ViewModelProperties.BtnVisibility,
                BtnContent = GlobalData.ViewModelProperties.BtnContent,
                BtnCommand = GlobalData.ViewModelProperties.BtnCommand
            };
            CurrentViewModel = viewModel_emErrorToMaintenance;

            if (GlobalData.BasicConfig.customer_display_flag == "1")
            {
                ViewModel_emMessage customerViewModel_emMessage = new ViewModel_emMessage
                {
                    ListHeaderTitle = GlobalData.CustomerViewModelProperties.ListHeaderTitle,
                    Message = string.IsNullOrEmpty(GlobalData.CustomerViewModelProperties.Message) ? msgMode.msg3 : GlobalData.CustomerViewModelProperties.Message,
                    SignalBarColor = signalBarColor,
                    HeaderFooterColor = GetHeaderFooterColor(),
                    HeaderTitleColor = GetHeaderTitleColor(),
                    BackgroundColor = GetBackgroundColor(true),
                    BodyTextColor = GetBodyTextColor(true),
                };
                CurrentCustomerViewModel = customerViewModel_emMessage;
            }
            Utilities.Log.Info("Load screen emErrorToMaintenance with " + msgCode);
        }
        public void LoadScreen_emLoginToMaintenance(string msgCode = "")
        {
            msgCode = "C0001";
            string brandName = "";
            FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
            if (GlobalData.ServiceName != null && GlobalData.BasicConfig.brand_name.Contains(GlobalData.ServiceName))
                brandName = Utilities.GetBrandName(GlobalData.ServiceName);
            GetListHeaderTitleAndMessage(msgMode, brandName);
            ViewModel_emLoginToMaintenance viewModel_emLoginToMaintenance = new ViewModel_emLoginToMaintenance
            {
                ListHeaderTitle = GlobalData.ViewModelProperties.ListHeaderTitleMaintenance,
                SignalBarColor = GetSignalBarColor(Signal.Normal),
                HeaderFooterColor = GetHeaderFooterColor(),
                HeaderTitleColor = GetHeaderTitleColor(),
                BackgroundColor = GetBackgroundColor(false),
                BodyTextColor = GetBodyTextColor(false),
                TitleColor = GetTitleColor(false),
                BorderLineColor = GetBorderLineColor(false),
                NumberColor = GetNumberColor(false),
                ButtonBorderColor = GetBtnBorderColor(false),
                NumPadButtonColor = GetNumPadBtnColor(),
                NumPadButtonLabelColor = GetNumPadBtnLabelColor(),
                ButtonPressedColor = GetBtnPressedColor(),
                ButtonPressedLabelColor = GetBtnPressedLabelColor(),
                ClearButtonColor = GetClearBtnColor(),
                ClearButtonLabelColor = GetClearBtnLabelColor(),
                Message = msgMode.msg2,
            };
            CurrentViewModel = viewModel_emLoginToMaintenance;

            if (GlobalData.BasicConfig.customer_display_flag == "1")
            {
                ViewModel_emMessage customerViewModel_emMessage = new ViewModel_emMessage
                {
                    ListHeaderTitle = GlobalData.CustomerViewModelProperties.ListHeaderTitle,
                    Message = msgMode.msg3,
                    SignalBarColor = GetSignalBarColor(Signal.Error),
                    HeaderFooterColor = GetHeaderFooterColor(),
                    HeaderTitleColor = GetHeaderTitleColor(),
                    BackgroundColor = GetBackgroundColor(true),
                    BodyTextColor = GetBodyTextColor(true),
                };
                CurrentCustomerViewModel = customerViewModel_emMessage;
            }

            Utilities.Log.Info("Load screen emLoginToMaintenance with " + msgCode);
        }
        public void LoadScreen_emMaintenanceMenu(string btnLastMinTransInquiryVisibility, string btnBalanceInquiryVisibility, string btnLastMinTransSuccessFailedVisibility)
        {
            string msgCode = "A0007";
            string brandName = "";
            FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
            if (GlobalData.ServiceName != null && GlobalData.BasicConfig.brand_name.Contains(GlobalData.ServiceName))
                brandName = Utilities.GetBrandName(GlobalData.ServiceName);
            GetListHeaderTitleAndMessage(msgMode, brandName);
            ViewModel_emMaintenanceMenu viewModel_EmMaintenanceMenu = new ViewModel_emMaintenanceMenu
            {
                ListHeaderTitle = GlobalData.ViewModelProperties.ListHeaderTitleMaintenance,
                SignalBarColor = GetSignalBarColor(Signal.Normal),
                HeaderFooterColor = GetHeaderFooterColor(),
                HeaderTitleColor = GetHeaderTitleColor(),
                BackgroundColor = GetBackgroundColor(),
                TitleColor = GetTitleColor(),
                ButtonBorderColor = GetBtnBorderColor(),
                NormalButtonColor = GetNormalBtnColor(),
                NormalButtonLabelColor = GetNormalBtnLabelColor(),
                BackButtonColor = GetBackBtnColor(),
                BackButtonLabelColor = GetBackBtnLabelColor(),
                BtnLastMinTransInquiryVisibility = btnLastMinTransInquiryVisibility,
                BtnBalanceInquiryVisibility = btnBalanceInquiryVisibility,
                BtnLastMinTransSuccessFailedVisibility = btnLastMinTransSuccessFailedVisibility
            };
            CurrentViewModel = viewModel_EmMaintenanceMenu;

            if (GlobalData.BasicConfig.customer_display_flag == "1")
            {
                ViewModel_emMessage customerViewModel_emMessage = new ViewModel_emMessage
                {
                    ListHeaderTitle = GlobalData.CustomerViewModelProperties.ListHeaderTitleMaintenance,
                    Message = msgMode.msg4,
                    SignalBarColor = GetSignalBarColor(Signal.Normal),
                    HeaderFooterColor = GetHeaderFooterColor(),
                    HeaderTitleColor = GetHeaderTitleColor(),
                    BackgroundColor = GetBackgroundColor(true),
                    BodyTextColor = GetBodyTextColor(true),
                };
                CurrentCustomerViewModel = customerViewModel_emMessage;
            }
            Utilities.Log.Info("Load screen emMaintenanceMenu with " + msgCode);
        }
        public void LoadScreen_emMaintenanceMenuFromError(string msgCode = "")
        {
            string brandName = "";
            FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
            if (GlobalData.ServiceName != null && GlobalData.BasicConfig.brand_name.Contains(GlobalData.ServiceName))
                brandName = Utilities.GetBrandName(GlobalData.ServiceName);
            GetListHeaderTitleAndMessage(msgMode, brandName);
            ViewModel_emMaintenanceMenuFromError viewModel_emMaintenanceMenuFromError = new ViewModel_emMaintenanceMenuFromError
            {
                ListHeaderTitle = GlobalData.ViewModelProperties.ListHeaderTitleMaintenance,
                ErrorMessage = string.IsNullOrEmpty(GlobalData.ViewModelProperties.MessageMaintenance) ? msgMode.msg2 : GlobalData.ViewModelProperties.MessageMaintenance,
                SignalBarColor = GetSignalBarColor(Signal.Normal),
                HeaderFooterColor = GetHeaderFooterColor(),
                HeaderTitleColor = GetHeaderTitleColor(),
                BackgroundColor = GetBackgroundColor(),
                BodyTextColor = GetBodyTextColor(),
                TitleColor = GetTitleColor(),
                ButtonBorderColor = GetBtnBorderColor(),
                NormalButtonColor = GetNormalBtnColor(),
                NormalButtonLabelColor = GetNormalBtnLabelColor(),
                BackButtonColor = GetBackBtnColor(),
                BackButtonLabelColor = GetBackBtnLabelColor(),
            };
            CurrentViewModel = viewModel_emMaintenanceMenuFromError;

            if (GlobalData.BasicConfig.customer_display_flag == "1")
            {
                ViewModel_emMessage customerViewModel_emMessage = new ViewModel_emMessage
                {
                    ListHeaderTitle = GlobalData.CustomerViewModelProperties.ListHeaderTitleMaintenance,
                    SignalBarColor = string.IsNullOrEmpty(GlobalData.CustomerViewModelProperties.MessageMaintenance) ? GetSignalBarColor(Signal.Normal) : GetSignalBarColor(Signal.Error),
                    HeaderFooterColor = GetHeaderFooterColor(),
                    HeaderTitleColor = GetHeaderTitleColor(),
                    BackgroundColor = GetBackgroundColor(true),
                    BodyTextColor = GetBodyTextColor(true),
                    Message = string.IsNullOrEmpty(GlobalData.CustomerViewModelProperties.MessageMaintenance) ? msgMode.msg4 : GlobalData.CustomerViewModelProperties.MessageMaintenance,
                };
                CurrentCustomerViewModel = customerViewModel_emMessage;
            }

            Utilities.Log.Info($"Load screen emMaintenanceMenuFromError with {msgCode} message: {viewModel_emMaintenanceMenuFromError.ErrorMessage}");
        }
        public void LoadScreen_emF()
        {
            LoadScreenCommon(_viewModel_emF, Signal.Complete);
            _viewModel_emF.BodyTextColor = GetBodyTextColor();
            CurrentViewModel = _viewModel_emF;
            Utilities.Log.Info("Load screen emF");
        }
        public void LoadScreen_emMAorMI_EMSG()
        {
            // _viewModel_emMAorMI_EMSG.HeaderTitle = HeaderTitle;
            _viewModel_emMAorMI_EMSG.HeaderFooterColor = GetHeaderFooterColor();
            if (_viewModel_emMAorMI_EMSG.HeaderFooterColor is null) _viewModel_emMAorMI_EMSG.HeaderFooterColor = "#FFFFFF";

            _viewModel_emMAorMI_EMSG.HeaderTitleColor = GetHeaderTitleColor();
            if (_viewModel_emMAorMI_EMSG.HeaderTitleColor is null) _viewModel_emMAorMI_EMSG.HeaderTitleColor = "#828282";

            _viewModel_emMAorMI_EMSG.SignalBarColor = GetSignalBarColor(Signal.Caution);
            if (_viewModel_emMAorMI_EMSG.SignalBarColor is null) _viewModel_emMAorMI_EMSG.SignalBarColor = "#FFCB0E";

            _viewModel_emMAorMI_EMSG.BackgroundColor = GetBackgroundColor();
            if (_viewModel_emMAorMI_EMSG.BackgroundColor is null) _viewModel_emMAorMI_EMSG.BackgroundColor = "#CEFFC0";

            _viewModel_emMAorMI_EMSG.BodyTextColor = GetBodyTextColor();
            if (_viewModel_emMAorMI_EMSG.BodyTextColor is null) _viewModel_emMAorMI_EMSG.BodyTextColor = "#254474";

            _viewModel_emMAorMI_EMSG.ButtonBorderColor = GetBtnBorderColor();
            if (_viewModel_emMAorMI_EMSG.ButtonBorderColor is null) _viewModel_emMAorMI_EMSG.ButtonBorderColor = "#254474";

            _viewModel_emMAorMI_EMSG.NormalButtonColor = GetNormalBtnColor();
            if (_viewModel_emMAorMI_EMSG.NormalButtonColor is null) _viewModel_emMAorMI_EMSG.NormalButtonColor = "#FFFFFF";

            _viewModel_emMAorMI_EMSG.NormalButtonLabelColor = GetNormalBtnLabelColor();
            if (_viewModel_emMAorMI_EMSG.NormalButtonLabelColor is null) _viewModel_emMAorMI_EMSG.NormalButtonLabelColor = "#254474";

            StateMachine.State preState = Session.ScreenState.CurrentState;
            if (preState == StateMachine.State.emIdle)
            {
                _viewModel_emMAorMI_EMSG.BtnOKVisibility = "Visible";
                _viewModel_emMAorMI_EMSG.BtnReturnVisibility = "Hidden";
                Utilities.Log.Info("Load screen emMI_EMSG");
            }
            else
            {
                _viewModel_emMAorMI_EMSG.BtnOKVisibility = "Hidden";
                _viewModel_emMAorMI_EMSG.BtnReturnVisibility = "Visible";
                Utilities.Log.Info("Load screen emMA_EMSG");
            }

            _viewModel_emMAorMI_EMSG.ErrorMessage = GlobalData.ApplicationErrorMsg;
            CurrentViewModel = _viewModel_emMAorMI_EMSG;
            Utilities.Log.Info($"Displayed message: {_viewModel_emMAorMI_EMSG.ErrorMessage}");
        }
        public void LoadScreen_emCM()
        {
            LoadScreenCommon(_viewModel_emCM, Signal.Caution);
            _viewModel_emCM.BodyTextColor = GetBodyTextColor();
            _viewModel_emCM.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emCM.NormalButtonColor = GetNormalBtnColor();
            _viewModel_emCM.NormalButtonLabelColor = GetNormalBtnLabelColor();

            if (Session.PreState_emCM == "emTM" && Session.PreState_emTM == "emZ")
            {
                _viewModel_emCM.ButtonLabel = "電子マネー選択に戻る";
                _viewModel_emCM.ButtonLabelSize = 34;
            }
            else
            {
                _viewModel_emCM.ButtonLabel = "支払選択に戻る";
                _viewModel_emCM.ButtonLabelSize = 44;
            }

            _viewModel_emCM.ErrorMessage = GlobalData.ScreenErrorMsg;
            CurrentViewModel = _viewModel_emCM;
            Utilities.Log.Info("Load screen emCM");
            Utilities.Log.Info($"Displayed message: {_viewModel_emCM.ErrorMessage}");
        }
        public void LoadScreen_emMT_MSG()
        {
            LoadScreenCommon(_viewModel_emMT_MSG, Signal.Caution);
            _viewModel_emMT_MSG.BodyTextColor = GetBodyTextColor();
            _viewModel_emMT_MSG.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emMT_MSG.NormalButtonColor = GetNormalBtnColor();
            _viewModel_emMT_MSG.NormalButtonLabelColor = GetNormalBtnLabelColor();

            _viewModel_emMT_MSG.ErrorMessage = GlobalData.ScreenErrorMsg;
            CurrentViewModel = _viewModel_emMT_MSG;
            Utilities.Log.Info("Load screen emMT_MSG");
            Utilities.Log.Info($"Displayed message: {_viewModel_emMT_MSG.ErrorMessage}");
        }
        public void LoadScreen_emMS_MENU()
        {
            // HeaderTitle = "電子マネー メンテナンス 疎通確認";
            _viewModel_emMS_MENU.TitleColor = GetTitleColor();
            _viewModel_emMS_MENU.BodyTextColor = GetBodyTextColor();
            _viewModel_emMS_MENU.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emMS_MENU.NormalButtonColor = GetNormalBtnColor();
            _viewModel_emMS_MENU.NormalButtonLabelColor = GetNormalBtnLabelColor();
            _viewModel_emMS_MENU.BackButtonColor = GetBackBtnColor();
            _viewModel_emMS_MENU.BackButtonLabelColor = GetBackBtnLabelColor();

            if (GlobalData.ScreenErrorMsg == "完了しました。")
            {
                LoadScreenCommon(_viewModel_emMS_MENU, Signal.Complete);
                Utilities.Log.Info("Load screen emMS_MENU_F");
            }
            else
            {
                LoadScreenCommon(_viewModel_emMS_MENU, Signal.Caution);
                Utilities.Log.Info("Load screen emMS_MENU");
            }
            _viewModel_emMS_MENU.ErrorMessage = GlobalData.ScreenErrorMsg;
            CurrentViewModel = _viewModel_emMS_MENU;
            Utilities.Log.Info($"Displayed message: {_viewModel_emMS_MENU.ErrorMessage}");
        }
        public void LoadScreen_emES_MENU()
        {
            LoadScreenCommon(_viewModel_emES_MENU, Signal.Normal);
            _viewModel_emES_MENU.TitleColor = GetTitleColor();
            _viewModel_emES_MENU.BodyTextColor = GetBodyTextColor();
            _viewModel_emES_MENU.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emES_MENU.NormalButtonColor = GetNormalBtnColor();
            _viewModel_emES_MENU.NormalButtonLabelColor = GetNormalBtnLabelColor();
            _viewModel_emES_MENU.BackButtonColor = GetBackBtnColor();
            _viewModel_emES_MENU.BackButtonLabelColor = GetBackBtnLabelColor();

            // _viewModel_emES_MENU.HeaderTitle = GlobalData.ViewModelProperties.ListHeaderTitle;
            string[] Message = GlobalData.ViewModelProperties.Message.Split('\t');
            _viewModel_emES_MENU.Message1 = Message[0];
            _viewModel_emES_MENU.Message2 = Message[1];
            _viewModel_emES_MENU.Message3 = Message[2];
            _viewModel_emES_MENU.Message4 = Message[3];

            CurrentViewModel = _viewModel_emES_MENU;

            if (GlobalData.BasicConfig.customer_display_flag == "1")
            {
                ViewModel_emMessage customerViewModel_emMessage = new ViewModel_emMessage
                {
                    // HeaderTitle = GlobalData.CustomerViewModelProperties.ListHeaderTitle,
                    Message = GlobalData.CustomerViewModelProperties.Message,
                    HeaderFooterColor = GlobalData.BasicConfig.color.common.base_color,
                    HeaderTitleColor = GlobalData.BasicConfig.color.common.title,
                    SignalBarColor = GlobalData.BasicConfig.color.common.signal_calling,
                    BackgroundColor = GlobalData.BasicConfig.color.customer.bg,
                    BodyTextColor = GlobalData.BasicConfig.color.customer.txt
                };
                CurrentCustomerViewModel = customerViewModel_emMessage;
            }

            Utilities.Log.Info("Load screen emES_MENU");
            Utilities.Log.Info($"Displayed message: {GlobalData.ViewModelProperties.Message}");
        }
        public void LoadScreen_emMSorMT_TA()
        {
            // if (Session.ClickedButton_emMT_MENU == "reprint_receipt") HeaderTitle = "電子マネー メンテナンス レシート再印刷";
            // else HeaderTitle = "電子マネー メンテナンス 疎通確認";
            LoadScreenCommon(_viewModel_emMSorMT_TA, Signal.Normal);
            _viewModel_emMSorMT_TA.BodyTextColor = GetBodyTextColor();
            CurrentViewModel = _viewModel_emMSorMT_TA;
        }
        public void LoadScreen_emMT_RV()
        {
            /*var configData = GlobalData.BasicConfig;
            _viewModel_emMT_RV.BtnContent = new string[6];
            _viewModel_emMT_RV.BtnVisibility = new string[6];
            for (int i = 0; i < 6; i++)
            {
                if (configData.brand_name.ContainsKey(configData.btn_order_ReadValue[i]))
                {
                    _viewModel_emMT_RV.BtnContent[i] = configData.brand_name[configData.btn_order_ReadValue[i]];
                    _viewModel_emMT_RV.BtnVisibility[i] = "Visible";
                }
                else
                {
                    _viewModel_emMT_RV.BtnVisibility[i] = "Hidden";
                }
            }*/

            // HeaderTitle = "電子マネー メンテナンス 残高照会";
            LoadScreenCommon(_viewModel_emMT_RV, Signal.Normal);
            _viewModel_emMT_RV.TitleColor = GetTitleColor();
            _viewModel_emMT_RV.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emMT_RV.BrandButtonColor = GetBrandBtnColor();
            _viewModel_emMT_RV.BrandButtonLabelColor = GetBrandBtnLabelColor();
            _viewModel_emMT_RV.BackButtonColor = GetBackBtnColor();
            _viewModel_emMT_RV.BackButtonLabelColor = GetBackBtnLabelColor();

            CurrentViewModel = _viewModel_emMT_RV;
            Utilities.Log.Info("Load screen emMT_RV");
        }
        public void LoadScreen_emMT_HV()
        {
            /*var configData = GlobalData.BasicConfig;
            _viewModel_emMT_HV.BtnContent = new string[6];
            _viewModel_emMT_HV.BtnVisibility = new string[6];
            for (int i = 0; i < 6; i++)
            {
                if (configData.brand_name.ContainsKey(configData.btn_order_CardHistory[i]))
                {
                    _viewModel_emMT_HV.BtnContent[i] = configData.brand_name[configData.btn_order_CardHistory[i]];
                    _viewModel_emMT_HV.BtnVisibility[i] = "Visible";
                }
                else
                {
                    _viewModel_emMT_HV.BtnVisibility[i] = "Hidden";
                }
            }*/

            // HeaderTitle = "電子マネー メンテナンス カード履歴照会";
            LoadScreenCommon(_viewModel_emMT_HV, Signal.Normal);
            _viewModel_emMT_HV.TitleColor = GetTitleColor();
            _viewModel_emMT_HV.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emMT_HV.BrandButtonColor = GetBrandBtnColor();
            _viewModel_emMT_HV.BrandButtonLabelColor = GetBrandBtnLabelColor();
            _viewModel_emMT_HV.BackButtonColor = GetBackBtnColor();
            _viewModel_emMT_HV.BackButtonLabelColor = GetBackBtnLabelColor();

            CurrentViewModel = _viewModel_emMT_HV;
            Utilities.Log.Info("Load screen emMT_HV");
        }
        public void LoadScreen_emMT_TM()
        {
            LoadScreenCommon(_viewModel_emMT_TM, Signal.Normal);
            _viewModel_emMT_TM.BodyTextColor = GetBodyTextColor();
            CurrentViewModel = _viewModel_emMT_TM;
            Utilities.Log.Info("Load screen emMT_TM");
        }
        public void LoadScreen_emMT_ZRESJ()
        {
            LoadScreenCommon(_viewModel_emMT_ZRESJ, Signal.Complete);
            _viewModel_emMT_ZRESJ.TitleColor = GetTitleColor();
            _viewModel_emMT_ZRESJ.BodyTextColor = GetBodyTextColor();
            _viewModel_emMT_ZRESJ.NumberColor = GetNumberColor();
            _viewModel_emMT_ZRESJ.BorderLineColor = GetBorderLineColor();
            _viewModel_emMT_ZRESJ.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emMT_ZRESJ.NormalButtonColor = GetNormalBtnColor();
            _viewModel_emMT_ZRESJ.NormalButtonLabelColor = GetNormalBtnLabelColor();

            if (GlobalData.TransactionErrorType == Utilities.ErrorType.ProcessingNotCompleted &&
                GlobalData.ServiceName == GlobalData.PreCurrentServiceSV)
            {
                _viewModel_emMT_ZRESJ.BtnOKVisibility = "Visible";
                _viewModel_emMT_ZRESJ.BtnReturnVisibility = "Hidden";
                Utilities.Log.Info("Load screen emMT_ZRESJ");
            }
            else
            {
                _viewModel_emMT_ZRESJ.BtnOKVisibility = "Hidden";
                _viewModel_emMT_ZRESJ.BtnReturnVisibility = "Visible";
                Utilities.Log.Info("Load screen emMT_ZRES");
            }

            if (GlobalData.ServiceName != null && GlobalData.BasicConfig.brand_name.Contains(GlobalData.ServiceName))
            {
                _viewModel_emMT_ZRESJ.BrandName = Utilities.GetBrandName(GlobalData.ServiceName);
            }

            _viewModel_emMT_ZRESJ.Balance = string.Format("{0:N0}", GlobalData.Balance) + "円";
            CurrentViewModel = _viewModel_emMT_ZRESJ;
        }
        public void LoadScreen_emMT_HRESJ()
        {
            LoadScreenCommon(_viewModel_emMT_HRESJ, Signal.Complete);
            _viewModel_emMT_HRESJ.TitleColor = GetTitleColor();
            _viewModel_emMT_HRESJ.BodyTextColor = GetBodyTextColor();
            _viewModel_emMT_HRESJ.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emMT_HRESJ.NormalButtonColor = GetNormalBtnColor();
            _viewModel_emMT_HRESJ.NormalButtonLabelColor = GetNormalBtnLabelColor();

            if (GlobalData.TransactionErrorType == Utilities.ErrorType.ProcessingNotCompleted &&
                GlobalData.ServiceName == GlobalData.PreCurrentServiceSV)
            {
                _viewModel_emMT_HRESJ.BtnOKVisibility = "Visible";
                _viewModel_emMT_HRESJ.BtnReturnVisibility = "Hidden";
                Utilities.Log.Info("Load screen emMT_HRESJ");
            }
            else
            {
                _viewModel_emMT_HRESJ.BtnOKVisibility = "Hidden";
                _viewModel_emMT_HRESJ.BtnReturnVisibility = "Visible";
                Utilities.Log.Info("Load screen emMT_HRES");
            }

            CurrentViewModel = _viewModel_emMT_HRESJ;
        }
        public void LoadScreen_emMT_EMSG()
        {
            LoadScreenCommon(_viewModel_emMT_EMSG, Signal.Caution);
            _viewModel_emMT_EMSG.BodyTextColor = GetBodyTextColor();
            _viewModel_emMT_EMSG.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emMT_EMSG.NormalButtonColor = GetNormalBtnColor();
            _viewModel_emMT_EMSG.NormalButtonLabelColor = GetNormalBtnLabelColor();

            _viewModel_emMT_EMSG.ErrorMessage = GlobalData.ScreenSubErrorMsg;
            CurrentViewModel = _viewModel_emMT_EMSG;
            Utilities.Log.Info("Load screen emMT_EMSG");
            Utilities.Log.Info($"Displayed message: {_viewModel_emMT_EMSG.ErrorMessage}");
        }
        public void LoadScreen_emMT_SMSG()
        {
            LoadScreenCommon(_viewModel_emMT_SMSG, Signal.Complete);
            _viewModel_emMT_SMSG.BodyTextColor = GetBodyTextColor();
            _viewModel_emMT_SMSG.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emMT_SMSG.NormalButtonColor = GetNormalBtnColor();
            _viewModel_emMT_SMSG.NormalButtonLabelColor = GetNormalBtnLabelColor();

            CurrentViewModel = _viewModel_emMT_SMSG;
            Utilities.Log.Info("Load screen emMT_SMSG");
        }
        public void LoadScreen_emMT_ZJG()
        {
            LoadScreenCommon(_viewModel_emMT_ZJG, Signal.Normal);
            _viewModel_emMT_ZJG.TitleColor = GetTitleColor();
            _viewModel_emMT_ZJG.BodyTextColor = GetBodyTextColor();
            _viewModel_emMT_ZJG.AttentionTextColor = GetAttentionTxtColor();
            _viewModel_emMT_ZJG.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emMT_ZJG.NormalButtonColor = GetNormalBtnColor();
            _viewModel_emMT_ZJG.NormalButtonLabelColor = GetNormalBtnLabelColor();
            CurrentViewModel = _viewModel_emMT_ZJG;
            Utilities.Log.Info("Load screen emMT_ZJG");
        }
        public void LoadScreen_emMT_HJG()
        {
            LoadScreenCommon(_viewModel_emMT_HJG, Signal.Normal);
            _viewModel_emMT_HJG.TitleColor = GetTitleColor();
            _viewModel_emMT_HJG.BodyTextColor = GetBodyTextColor();
            _viewModel_emMT_HJG.AttentionTextColor = GetAttentionTxtColor();
            _viewModel_emMT_HJG.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emMT_HJG.NormalButtonColor = GetNormalBtnColor();
            _viewModel_emMT_HJG.BackButtonColor = GetBackBtnColor();
            _viewModel_emMT_HJG.BackButtonLabelColor = GetBackBtnLabelColor();
            _viewModel_emMT_HJG.NormalButtonLabelColor = GetNormalBtnLabelColor();
            CurrentViewModel = _viewModel_emMT_HJG;
            Utilities.Log.Info("Load screen emMT_HJG");
        }
        public void LoadScreen_emMT_JGC1()
        {
            LoadScreenCommon(_viewModel_emMT_JGC1, Signal.Normal);
            _viewModel_emMT_JGC1.AttentionTitle = GetAttentionTitle(_viewModel_emMT_JGC1);
            _viewModel_emMT_JGC1.AttentionTitleColor = GetAttentionTxtColor();
            _viewModel_emMT_JGC1.BodyText = GetBodyText(_viewModel_emMT_JGC1);
            _viewModel_emMT_JGC1.BodyTextColor = GetBodyTextColor();
            _viewModel_emMT_JGC1.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emMT_JGC1.NormalButtonColor = GetNormalBtnColor();
            _viewModel_emMT_JGC1.NormalButtonLabelColor = GetNormalBtnLabelColor();
            CurrentViewModel = _viewModel_emMT_JGC1;
            Utilities.Log.Info("Load screen emMT_JGC1");
        }
        public void LoadScreen_emMT_JGC2()
        {
            LoadScreenCommon(_viewModel_emMT_JGC2, Signal.Normal);
            _viewModel_emMT_JGC2.AttentionTitle = GetAttentionTitle(_viewModel_emMT_JGC2);
            _viewModel_emMT_JGC2.AttentionTitleColor = GetAttentionTxtColor();
            _viewModel_emMT_JGC2.BodyText = GetBodyText(_viewModel_emMT_JGC2);
            _viewModel_emMT_JGC2.BodyTextColor = GetBodyTextColor();
            _viewModel_emMT_JGC2.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emMT_JGC2.NormalButtonColor = GetNormalBtnColor();
            _viewModel_emMT_JGC2.NormalButtonLabelColor = GetNormalBtnLabelColor();
            CurrentViewModel = _viewModel_emMT_JGC2;
            Utilities.Log.Info("Load screen emMT_JGC2");
        }
        public void LoadScreen_emMT_F()
        {
            // if (!Session.IsPrintRetryPreState_emMT_F) HeaderTitle = "電子マネー メンテナンス";
            LoadScreenCommon(_viewModel_emMT_F, Signal.Complete);
            _viewModel_emMT_F.BodyTextColor = GetBodyTextColor();
            CurrentViewModel = _viewModel_emMT_F;
            Utilities.Log.Info("Load screen emMT_F");
        }
        public void LoadScreen_emM_TA()
        {
            //if (Session.ClickedButton_emM_MENU == "check_connection") HeaderTitle = "電子マネー メンテナンス 疎通確認";
            //else if (Session.ClickedButton_emM_MENU == "reprint_receipt") HeaderTitle = "電子マネー メンテナンス レシート再印刷";
            //else HeaderTitle = "電子マネー メンテナンス 日計";
            LoadScreenCommon(_viewModel_emM_TA, Signal.Normal);
            _viewModel_emM_TA.BodyTextColor = GetBodyTextColor();
            _viewModel_emM_TA.Message = GlobalData.ScreenProgressMsg;
            CurrentViewModel = _viewModel_emM_TA;

            if (GlobalData.BasicConfig.customer_display_flag == "1")
            {
                ViewModel_emMessage customerViewModel_emMessage = new ViewModel_emMessage
                {
                    // HeaderTitle = GlobalData.CustomerViewModelProperties.ListHeaderTitle,
                    Message = GlobalData.CustomerViewModelProperties.Message,
                    HeaderFooterColor = GlobalData.BasicConfig.color.common.base_color,
                    HeaderTitleColor = GlobalData.BasicConfig.color.common.title,
                    SignalBarColor = GlobalData.BasicConfig.color.common.signal_calling,
                    BackgroundColor = GlobalData.BasicConfig.color.customer.bg,
                    BodyTextColor = GlobalData.BasicConfig.color.customer.txt
                };
                CurrentCustomerViewModel = customerViewModel_emMessage;
            }

            Utilities.Log.Info("Load screen emM_TA");
            Utilities.Log.Info($"Displayed message: {_viewModel_emM_TA.Message}");
        }
        public void LoadScreen_emM_SMSG()
        {
            LoadScreenCommon(_viewModel_emM_SMSG, Signal.Complete);
            _viewModel_emM_SMSG.BodyTextColor = GetBodyTextColor();
            _viewModel_emM_SMSG.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emM_SMSG.NormalButtonColor = GetNormalBtnColor();
            _viewModel_emM_SMSG.NormalButtonLabelColor = GetNormalBtnLabelColor();

            _viewModel_emM_SMSG.BodyText = GlobalData.ScreenResultMsg;
            // _viewModel_emM_SMSG.HeaderTitle = GlobalData.ViewModelProperties.ListHeaderTitle;
            CurrentViewModel = _viewModel_emM_SMSG;
            Utilities.Log.Info("Load screen emM_SMSG");
        }
        public void LoadScreen_emM_EMSG()
        {
            _viewModel_emM_EMSG.HeaderFooterColor = GlobalData.BasicConfig.color.common.base_color;
            _viewModel_emM_EMSG.HeaderTitleColor = GlobalData.BasicConfig.color.common.title;
            _viewModel_emM_EMSG.SignalBarColor = GlobalData.BasicConfig.color.common.signal_caution;
            _viewModel_emM_EMSG.BackgroundColor = GlobalData.BasicConfig.color.maintenance.bg;
            _viewModel_emM_EMSG.BodyTextColor = GetBodyTextColor();
            _viewModel_emM_EMSG.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emM_EMSG.NormalButtonColor = GetNormalBtnColor();
            _viewModel_emM_EMSG.NormalButtonLabelColor = GetNormalBtnLabelColor();
            // _viewModel_emM_EMSG.HeaderTitle = GlobalData.ViewModelProperties.ListHeaderTitle;
            _viewModel_emM_EMSG.ErrorMessage = GlobalData.ScreenMErrorMsg;
            CurrentViewModel = _viewModel_emM_EMSG;

            if (GlobalData.BasicConfig.customer_display_flag == "1")
            {
                ViewModel_emMessage customerViewModel_emMessage = new ViewModel_emMessage
                {
                    // HeaderTitle = GlobalData.CustomerViewModelProperties.ListHeaderTitle,
                    Message = GlobalData.CustomerViewModelProperties.Message,
                    HeaderFooterColor = GlobalData.BasicConfig.color.common.base_color,
                    HeaderTitleColor = GlobalData.BasicConfig.color.common.title,
                    SignalBarColor = GlobalData.BasicConfig.color.common.signal_calling,
                    BackgroundColor = GlobalData.BasicConfig.color.customer.bg,
                    BodyTextColor = GlobalData.BasicConfig.color.customer.txt
                };
                CurrentCustomerViewModel = customerViewModel_emMessage;
            }

            Utilities.Log.Info("Load screen emM_EMSG");
            Utilities.Log.Info($"Displayed message: {_viewModel_emM_EMSG.ErrorMessage}");
        }
        public void LoadScreen_emM_CH()
        {
            /*var configData = GlobalData.BasicConfig;
            _viewModel_emM_CH.BtnContent = new string[6];
            _viewModel_emM_CH.BtnVisibility = new string[6];
            for (int i = 0; i < 6; i++)
            {
                if (configData.brand_name.ContainsKey(configData.btn_order_CardHistory[i]))
                {
                    _viewModel_emM_CH.BtnContent[i] = configData.brand_name[configData.btn_order_CardHistory[i]];
                    _viewModel_emM_CH.BtnVisibility[i] = "Visible";
                }
                else
                {
                    _viewModel_emM_CH.BtnVisibility[i] = "Hidden";
                }
            }*/

            // HeaderTitle = "電子マネー メンテナンス カード履歴照会";
            LoadScreenCommon(_viewModel_emM_CH, Signal.Normal);
            _viewModel_emM_CH.TitleColor = GetTitleColor();
            _viewModel_emM_CH.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emM_CH.BrandButtonColor = GetBrandBtnColor();
            _viewModel_emM_CH.BrandButtonLabelColor = GetBrandBtnLabelColor();
            _viewModel_emM_CH.BackButtonColor = GetBackBtnColor();
            _viewModel_emM_CH.BackButtonLabelColor = GetBackBtnLabelColor();

            CurrentViewModel = _viewModel_emM_CH;
            Utilities.Log.Info("Load screen emM_CH");
        }
        public void LoadScreen_emM_HRES()
        {
            LoadScreenCommon(_viewModel_emM_HRES, Signal.Complete);
            _viewModel_emM_HRES.TitleColor = GetTitleColor();
            _viewModel_emM_HRES.BodyTextColor = GetBodyTextColor();
            _viewModel_emM_HRES.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emM_HRES.NormalButtonColor = GetNormalBtnColor();
            _viewModel_emM_HRES.NormalButtonLabelColor = GetNormalBtnLabelColor();

            CurrentViewModel = _viewModel_emM_HRES;
            Utilities.Log.Info("Load screen emM_HRES");
        }
        public void LoadScreen_emM_RV()
        {
            /*var configData = GlobalData.BasicConfig;
            _viewModel_emM_RV.BtnContent = new string[6];
            _viewModel_emM_RV.BtnVisibility = new string[6];
            for (int i = 0; i < 6; i++)
            {
                if (configData.brand_name.ContainsKey(configData.btn_order_ReadValue[i]))
                {
                    _viewModel_emM_RV.BtnContent[i] = configData.brand_name[configData.btn_order_ReadValue[i]];
                    _viewModel_emM_RV.BtnVisibility[i] = "Visible";
                }
                else
                {
                    _viewModel_emM_RV.BtnVisibility[i] = "Hidden";
                }
            }*/

            // HeaderTitle = "電子マネー メンテナンス 残高照会";
            LoadScreenCommon(_viewModel_emM_RV, Signal.Normal);
            _viewModel_emM_RV.TitleColor = GetTitleColor();
            _viewModel_emM_RV.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emM_RV.BrandButtonColor = GetBrandBtnColor();
            _viewModel_emM_RV.BrandButtonLabelColor = GetBrandBtnLabelColor();
            _viewModel_emM_RV.BackButtonColor = GetBackBtnColor();
            _viewModel_emM_RV.BackButtonLabelColor = GetBackBtnLabelColor();

            CurrentViewModel = _viewModel_emM_RV;
            Utilities.Log.Info("Load screen emM_RV");
        }
        public void LoadScreen_emM_TM()
        {
            LoadScreenCommon(_viewModel_emM_TM, Signal.Normal);
            _viewModel_emM_TM.BodyTextColor = GetBodyTextColor();
            CurrentViewModel = _viewModel_emM_TM;
            Utilities.Log.Info("Load screen emM_TM");
        }
        public void LoadScreen_emM_ZRES()
        {
            LoadScreenCommon(_viewModel_emM_ZRES, Signal.Complete);
            _viewModel_emM_ZRES.TitleColor = GetTitleColor();
            _viewModel_emM_ZRES.BodyTextColor = GetBodyTextColor();
            _viewModel_emM_ZRES.NumberColor = GetNumberColor();
            _viewModel_emM_ZRES.BorderLineColor = GetBorderLineColor();
            _viewModel_emM_ZRES.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emM_ZRES.NormalButtonColor = GetNormalBtnColor();
            _viewModel_emM_ZRES.NormalButtonLabelColor = GetNormalBtnLabelColor();

            if (GlobalData.ServiceName != null && GlobalData.BasicConfig.brand_name.Contains(GlobalData.ServiceName))
            {
                _viewModel_emM_ZRES.BrandName = Utilities.GetBrandName(GlobalData.ServiceName);
            }

            _viewModel_emM_ZRES.Balance = string.Format("{0:N0}", GlobalData.Balance) + "円";
            CurrentViewModel = _viewModel_emM_ZRES;
            Utilities.Log.Info("Load screen emM_ZRES");
        }
        public void LoadScreen_emM_ZRESJ()
        {
            LoadScreenCommon(_viewModel_emM_ZRESJ, Signal.Complete);
            _viewModel_emM_ZRESJ.TitleColor = GetTitleColor();
            _viewModel_emM_ZRESJ.BodyTextColor = GetBodyTextColor();
            _viewModel_emM_ZRESJ.NumberColor = GetNumberColor();
            _viewModel_emM_ZRESJ.BorderLineColor = GetBorderLineColor();
            _viewModel_emM_ZRESJ.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emM_ZRESJ.NormalButtonColor = GetNormalBtnColor();
            _viewModel_emM_ZRESJ.NormalButtonLabelColor = GetNormalBtnLabelColor();

            if(GlobalData.ServiceName != null && GlobalData.BasicConfig.brand_name.Contains(GlobalData.ServiceName))
            {
                _viewModel_emM_ZRESJ.BrandName = Utilities.GetBrandName(GlobalData.ServiceName);
            }

            _viewModel_emM_ZRESJ.Balance = string.Format("{0:N0}", GlobalData.Balance) + "円";
            CurrentViewModel = _viewModel_emM_ZRESJ;
            Utilities.Log.Info("Load screen emM_ZRESJ");
        }
        public void LoadScreen_emM_HRESJ()
        {
            LoadScreenCommon(_viewModel_emM_HRESJ, Signal.Complete);
            _viewModel_emM_HRESJ.TitleColor = GetTitleColor();
            _viewModel_emM_HRESJ.BodyTextColor = GetBodyTextColor();
            _viewModel_emM_HRESJ.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emM_HRESJ.NormalButtonColor = GetNormalBtnColor();
            _viewModel_emM_HRESJ.NormalButtonLabelColor = GetNormalBtnLabelColor();

            CurrentViewModel = _viewModel_emM_HRESJ;
            Utilities.Log.Info("Load screen emM_HRESJ");
        }
        public void LoadScreen_emM_ZJG()
        {
            LoadScreenCommon(_viewModel_emM_ZJG, Signal.Normal);
            _viewModel_emM_ZJG.TitleColor = GetTitleColor();
            _viewModel_emM_ZJG.BodyTextColor = GetBodyTextColor();
            _viewModel_emM_ZJG.AttentionTextColor = GetAttentionTxtColor();
            _viewModel_emM_ZJG.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emM_ZJG.NormalButtonColor = GetNormalBtnColor();
            _viewModel_emM_ZJG.NormalButtonLabelColor = GetNormalBtnLabelColor();
            CurrentViewModel = _viewModel_emM_ZJG;
            Utilities.Log.Info("Load screen emM_ZJG");
        }
        public void LoadScreen_emM_HJG()
        {
            LoadScreenCommon(_viewModel_emM_HJG, Signal.Normal);
            _viewModel_emM_HJG.TitleColor = GetTitleColor();
            _viewModel_emM_HJG.BodyTextColor = GetBodyTextColor();
            _viewModel_emM_HJG.AttentionTextColor = GetAttentionTxtColor();
            _viewModel_emM_HJG.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emM_HJG.NormalButtonColor = GetNormalBtnColor();
            _viewModel_emM_HJG.NormalButtonLabelColor = GetNormalBtnLabelColor();
            CurrentViewModel = _viewModel_emM_HJG;
            Utilities.Log.Info("Load screen emM_HJG");
        }
        public void LoadScreen_emM_JGC1()
        {
            LoadScreenCommon(_viewModel_emM_JGC1, Signal.Normal);
            _viewModel_emM_JGC1.AttentionTitle = GetAttentionTitle(_viewModel_emM_JGC1);
            _viewModel_emM_JGC1.AttentionTitleColor = GetAttentionTxtColor();
            _viewModel_emM_JGC1.BodyText = GetBodyText(_viewModel_emM_JGC1);
            _viewModel_emM_JGC1.BodyTextColor = GetBodyTextColor();
            _viewModel_emM_JGC1.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emM_JGC1.NormalButtonColor = GetNormalBtnColor();
            _viewModel_emM_JGC1.NormalButtonLabelColor = GetNormalBtnLabelColor();
            CurrentViewModel = _viewModel_emM_JGC1;
            Utilities.Log.Info("Load screen emM_JGC1");
        }
        public void LoadScreen_emM_JGC2()
        {
            LoadScreenCommon(_viewModel_emM_JGC2, Signal.Normal);
            _viewModel_emM_JGC2.AttentionTitle = GetAttentionTitle(_viewModel_emM_JGC2);
            _viewModel_emM_JGC2.AttentionTitleColor = GetAttentionTxtColor();
            _viewModel_emM_JGC2.BodyText = GetBodyText(_viewModel_emM_JGC2);
            _viewModel_emM_JGC2.BodyTextColor = GetBodyTextColor();
            _viewModel_emM_JGC2.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emM_JGC2.NormalButtonColor = GetNormalBtnColor();
            _viewModel_emM_JGC2.NormalButtonLabelColor = GetNormalBtnLabelColor();
            CurrentViewModel = _viewModel_emM_JGC2;
            Utilities.Log.Info("Load screen emM_JGC2");
        }
        public void LoadScreen_emM_TTLC()
        {
            // HeaderTitle = "電子マネー メンテナンス 日計";
            LoadScreenCommon(_viewModel_emM_TTLC, Signal.Normal);
            _viewModel_emM_TTLC.AttentionTitle = GetAttentionTitle(_viewModel_emM_TTLC);
            _viewModel_emM_TTLC.AttentionTitleColor = GetAttentionTxtColor();
            _viewModel_emM_TTLC.BodyText = GetBodyText(_viewModel_emM_TTLC);
            _viewModel_emM_TTLC.BodyTextColor = GetBodyTextColor();
            _viewModel_emM_TTLC.ButtonBorderColor = GetBtnBorderColor();
            _viewModel_emM_TTLC.NormalButtonColor = GetNormalBtnColor();
            _viewModel_emM_TTLC.NormalButtonLabelColor = GetNormalBtnLabelColor();
            CurrentViewModel = _viewModel_emM_TTLC;
            Utilities.Log.Info("Load screen emM_TTLC");
        }
        public void LoadScreen_emM_F()
        {
            // HeaderTitle = "電子マネー メンテナンス";
            LoadScreenCommon(_viewModel_emM_F, Signal.Complete);
            _viewModel_emM_F.BodyTextColor = GetBodyTextColor();
            CurrentViewModel = _viewModel_emM_F;
            Utilities.Log.Info("Load screen emM_F");
        }

        public override void UpdateView()
        {
        }
    }
}
