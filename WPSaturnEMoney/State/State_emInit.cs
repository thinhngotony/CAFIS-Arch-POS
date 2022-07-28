using System;
using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Models;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

namespace WPSaturnEMoney.State
{
    class State_emInit : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            string message = "";
            bool isFileOK = false;
            bool isBrandNameOK = false;
            bool is_btn_order_SubtractValue_OK = false;
            // bool isHealthCheckOK = false;
            if (Session.ScreenState.CurrentState != StateMachine.State.emInit)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emInit;

                string msgCode = "";
                ViewModel_emMessage viewModel_emMessage = new ViewModel_emMessage
                {
                    ListHeaderTitle = new List<string> { "電子マネー", "" },
                    Message = "しばらくお待ち下さい。",
                    SignalBarColor = "#BDBDBD",
                    HeaderFooterColor = "#FFFFFF",
                    HeaderTitleColor = "#828282",
                    BackgroundColor = "#E1E1E1",
                    BodyTextColor = "#254474",
                    ButtonBorderColor = "#254474",
                    NormalButtonColor = "#EDEDED",
                    NormalButtonLabelColor = "#254474",
                };
                Session.MainViewModel.CurrentViewModel = viewModel_emMessage;

                ViewModel_emMessage customerViewModel_emMessage = new ViewModel_emMessage
                {
                    ListHeaderTitle = new List<string> { "電子マネー", "" },
                    Message = "電子マネー決済の準備中です。\nしばらくお待ちください。",
                    SignalBarColor = "#BDBDBD",
                    HeaderFooterColor = "#FFFFFF",
                    HeaderTitleColor = "#828282",
                    BackgroundColor = "#E1E1E1",
                    BodyTextColor = "#254474",
                    ButtonBorderColor = "#254474",
                    NormalButtonColor = "#EDEDED",
                    NormalButtonLabelColor = "#254474",
                };
                Session.MainViewModel.CurrentCustomerViewModel = customerViewModel_emMessage;
                await Task.Delay(1000);

                foreach (string file in GlobalData.ListConfigFiles)
                {
                    switch (file)
                    {
                        case "Config.json":
                            Utilities.Log.Info(file + " file start checking!");
                            isFileOK = FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.ConfigPath, ref GlobalData.BasicConfig);
                            if (!isFileOK)
                            {
                                message = file + " が存在しません。";
                                Utilities.Log.Error("▲ " + file + " file is not exist!");
                            }
                            else if (FileStruct.IsNullorEmpty(GlobalData.BasicConfig))
                            {
                                isFileOK = false;
                                message = file + " の内容に問題があります。";
                                Utilities.Log.Error("▲ " + file + ": setting is not correct.");
                            }
                            else if (FileStruct.IsNullorEmpty(GlobalData.BasicConfig.color))
                            {
                                isFileOK = false;
                                message = file + " の内容に問題があります。";
                                Utilities.Log.Error("▲ " + file + " >> color: setting is not correct.");
                            }
                            else if (FileStruct.IsNullorEmpty(GlobalData.BasicConfig.color.common))
                            {
                                isFileOK = false;
                                message = file + " の内容に問題があります。";
                                Utilities.Log.Error("▲ " + file + " >> color >> common: setting is not correct.");
                            }
                            else if (FileStruct.IsNullorEmpty(GlobalData.BasicConfig.color.customer))
                            {
                                isFileOK = false;
                                message = file + " の内容に問題があります。";
                                Utilities.Log.Error("▲ " + file + " >> color >> customer: setting is not correct.");
                            }
                            else if (FileStruct.IsNullorEmpty(GlobalData.BasicConfig.color.maintenance))
                            {
                                isFileOK = false;
                                message = file + " の内容に問題があります。";
                                Utilities.Log.Error("▲ " + file + " >> color >> maintenance: setting is not correct.");
                            }
                            else if (FileStruct.IsNullorEmpty(GlobalData.BasicConfig.screen_msg))
                            {
                                isFileOK = false;
                                message = file + " の内容に問題があります。";
                                Utilities.Log.Error("▲ " + file + " >> screen_msg: setting is not correct.");
                            }
                            else
                            {
                                Utilities.Log.Info(file + " done checking and no error found!");
                            }
                            break;
                        case "Msg_Brand.json":
                            Utilities.Log.Info(file + " file start checking!");
                            isFileOK = FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.MsgBrandPath, ref GlobalData.MsgBrandConfig);
                            if (!isFileOK)
                            {
                                message = file + " が存在しません。";
                                Utilities.Log.Error("▲ " + file + " file is not exist!");
                                break;
                            }
                            else if (GlobalData.MsgBrandConfig.Count == 0)
                            {
                                isFileOK = false;
                                message = file + " の内容に問題があります。";
                                Utilities.Log.Error("▲ " + file + " file is empty!");
                            }
                            else
                            {
                                int brandsLg = GlobalData.MsgBrandConfig.Count;
                                for (int i = 0; i < brandsLg; i++)
                                {
                                    if (string.IsNullOrEmpty(GlobalData.MsgBrandConfig[i].brand_code))
                                    {
                                        isFileOK = false;
                                        Utilities.Log.Error("▲ " + file + " >> " + (i + 1) + "(th) brand_code: setting is not correct.");
                                        break;
                                    }
                                    int lg = GlobalData.MsgBrandConfig[i].message.Count;
                                    for (int j = 0; j < lg; j++)
                                    {
                                        if (FileStruct.IsNullorEmpty(GlobalData.MsgBrandConfig[i].message[j]))
                                        {
                                            isFileOK = false;
                                            Utilities.Log.Error("▲ " + file + " >> " + GlobalData.MsgBrandConfig[i].brand_code +
                                                " brand_code >> " + (j + 1) + "(th) message: setting is not correct.");
                                            break;
                                        }
                                    }
                                    if (!isFileOK) break;
                                }
                                if (!isFileOK) message = file + " の内容に問題があります。";
                                else Utilities.Log.Info(file + " done checking and no error found!");
                            }
                            break;
                        case "Msg_Common.json":
                            Utilities.Log.Info(file + " file start checking!");
                            isFileOK = FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.MsgCommonPath, ref GlobalData.MsgCommonConfig);
                            if (!isFileOK)
                            {
                                message = file + " が存在しません。";
                                Utilities.Log.Error("▲ " + file + " file is not exist!");
                                break;
                            }
                            else if (GlobalData.MsgCommonConfig.Count == 0)
                            {
                                isFileOK = false;
                                message = file + " の内容に問題があります。";
                                Utilities.Log.Error("▲ " + file + " file is empty!");
                            }
                            else
                            {
                                int msgCommonLg = GlobalData.MsgCommonConfig.Count;
                                for (int i = 0; i < msgCommonLg; i++)
                                {
                                    if (FileStruct.IsNullorEmpty(GlobalData.MsgCommonConfig[i]))
                                    {
                                        isFileOK = false;
                                        message = file + " の内容に問題があります。";
                                        Utilities.Log.Error("▲ " + file + " >> " + (i + 1) + "(th) message: setting is not correct.");
                                        break;
                                    }
                                }
                                if (isFileOK) Utilities.Log.Info(file + " done checking and no error found!");
                            }
                            break;
                        case "Msg_Condition.json":
                            Utilities.Log.Info(file + " file start checking!");
                            isFileOK = FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.MsgConditionPath, ref GlobalData.MsgConditionConfig);
                            if (!isFileOK)
                            {
                                message = file + " が存在しません。";
                                Utilities.Log.Error("▲ " + file + " file is not exist!");
                                break;
                            }
                            else if (GlobalData.MsgConditionConfig.Count == 0)
                            {
                                isFileOK = false;
                                message = file + " の内容に問題があります。";
                                Utilities.Log.Error("▲ " + file + " file is empty!");
                            }
                            else
                            {
                                int msgConditionLg = GlobalData.MsgConditionConfig.Count;
                                for (int i = 0; i < msgConditionLg; i++)
                                {
                                    if (FileStruct.IsNullorEmpty(GlobalData.MsgConditionConfig[i]))
                                    {
                                        isFileOK = false;
                                        message = file + " の内容に問題があります。";
                                        Utilities.Log.Error("▲ " + file + " >> " + (i + 1) + "(th) message: setting is not correct.");
                                        break;
                                    }
                                }
                                if (isFileOK) Utilities.Log.Info(file + " done checking and no error found!");
                            }
                            break;
                        case "Msg_HealthCheck1.json":
                            Utilities.Log.Info(file + " file start checking!");
                            isFileOK = FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.MsgHealthCheck1Path, ref GlobalData.MsgHealthCheck1Config);
                            if (!isFileOK)
                            {
                                message = file + " が存在しません。";
                                Utilities.Log.Error("▲ " + file + " file is not exist!");
                                break;
                            }
                            else if (GlobalData.MsgHealthCheck1Config.Count == 0)
                            {
                                isFileOK = false;
                                message = file + " の内容に問題があります。";
                                Utilities.Log.Error("▲ " + file + " file is empty!");
                            }
                            else
                            {
                                int msgHealthCheck1Lg = GlobalData.MsgHealthCheck1Config.Count;
                                for (int i = 0; i < msgHealthCheck1Lg; i++)
                                {
                                    if (FileStruct.IsNullorEmpty(GlobalData.MsgHealthCheck1Config[i]))
                                    {
                                        isFileOK = false;
                                        message = file + " の内容に問題があります。";
                                        Utilities.Log.Error("▲ " + file + " >> " + (i + 1) + "(th) message: setting is not correct.");
                                        break;
                                    }
                                }
                                if (isFileOK) Utilities.Log.Info(file + " done checking and no error found!");
                            }
                            break;
                        case "Msg_HealthCheck2.json":
                            Utilities.Log.Info(file + " file start checking!");
                            isFileOK = FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.MsgHealthCheck2Path, ref GlobalData.MsgHealthCheck2Config);
                            if (!isFileOK)
                            {
                                message = file + " が存在しません。";
                                Utilities.Log.Error("▲ " + file + " file is not exist!");
                                break;
                            }
                            else if (GlobalData.MsgHealthCheck2Config.Count == 0)
                            {
                                isFileOK = false;
                                message = file + " の内容に問題があります。";
                                Utilities.Log.Error("▲ " + file + " file is empty!");
                            }
                            else
                            {
                                int msgHealthCheck2Lg = GlobalData.MsgHealthCheck2Config.Count;
                                for (int i = 0; i < msgHealthCheck2Lg; i++)
                                {
                                    if (FileStruct.IsNullorEmpty(GlobalData.MsgHealthCheck2Config[i]))
                                    {
                                        isFileOK = false;
                                        message = file + " の内容に問題があります。";
                                        Utilities.Log.Error("▲ " + file + " >> " + (i + 1) + "(th) message: setting is not correct.");
                                        break;
                                    }
                                }
                                if (isFileOK) Utilities.Log.Info(file + " done checking and no error found!");
                            }
                            break;
                        case "Msg_HealthCheck3.json":
                            Utilities.Log.Info(file + " file start checking!");
                            isFileOK = FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.MsgHealthCheck3Path, ref GlobalData.MsgHealthCheck3Config);
                            if (!isFileOK)
                            {
                                message = file + " が存在しません。";
                                Utilities.Log.Error("▲ " + file + " file is not exist!");
                                break;
                            }
                            else if (GlobalData.MsgHealthCheck3Config.Count == 0)
                            {
                                isFileOK = false;
                                message = file + " の内容に問題があります。";
                                Utilities.Log.Error("▲ " + file + " file is empty!");
                            }
                            else
                            {
                                int msgHealthCheck3Lg = GlobalData.MsgHealthCheck3Config.Count;
                                for (int i = 0; i < msgHealthCheck3Lg; i++)
                                {
                                    if (FileStruct.IsNullorEmpty(GlobalData.MsgHealthCheck3Config[i]))
                                    {
                                        isFileOK = false;
                                        message = file + " の内容に問題があります。";
                                        Utilities.Log.Error("▲ " + file + " >> " + (i + 1) + "(th) message: setting is not correct.");
                                        break;
                                    }
                                }
                                if (isFileOK) Utilities.Log.Info(file + " done checking and no error found!");
                            }
                            break;
                        case "Msg_Mode.json":
                            Utilities.Log.Info(file + " file start checking!");
                            isFileOK = FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.MsgModePath, ref GlobalData.MsgModeConfig);
                            if (!isFileOK)
                            {
                                message = file + " が存在しません。";
                                Utilities.Log.Error("▲ " + file + " file is not exist!");
                            }
                            else if (GlobalData.MsgModeConfig.Count == 0)
                            {
                                isFileOK = false;
                                message = file + " の内容に問題があります。";
                                Utilities.Log.Error("▲ " + file + " file is empty!");
                            }
                            else
                            {
                                int lg = GlobalData.MsgModeConfig.Count;
                                for (int i = 0; i < lg; i++)
                                {
                                    if (string.IsNullOrEmpty(GlobalData.MsgModeConfig[i].msg_code))
                                    {
                                        isFileOK = false;
                                        message = file + " の内容に問題があります。";
                                        Utilities.Log.Error("▲ " + file + " >> " + (i + 1) + "(th) message >> msg_code: setting is not correct.");
                                        break;
                                    }
                                    if (string.IsNullOrEmpty(GlobalData.MsgModeConfig[i].type))
                                    {
                                        isFileOK = false;
                                        message = file + " の内容に問題があります。";
                                        Utilities.Log.Error("▲ " + file + " >> " + (i + 1) + "(th) message >> type: setting is not correct.");
                                        break;
                                    }
                                }
                                if (isFileOK) Utilities.Log.Info(file + " done checking and no error found!");
                            }
                            break;
                        case "PosIFConfig.json":
                            Utilities.Log.Info(file + " file start checking!");
                            isFileOK = FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.PosIFConfigPath, ref GlobalData.PosIFConfig);
                            if (!isFileOK)
                            {
                                message = file + " が存在しません。";
                                Utilities.Log.Error("▲ " + file + " file is not exist!");
                            }
                            else if (FileStruct.IsNullorEmpty(GlobalData.PosIFConfig))
                            {
                                message = file + " の内容に問題があります。";
                                isFileOK = false;
                                Utilities.Log.Error("▲ " + file + ": setting is not correct.");
                            }
                            else if (GlobalData.PosIFConfig.pos_btn_loc.Length != 2)
                            {
                                message = file + " の内容に問題があります。";
                                isFileOK = false;
                                Utilities.Log.Error("▲ " + file + " >> pos_btn_loc: setting is not correct.");
                            }
                            else
                            {
                                Utilities.Log.Info(file + " done checking and no error found!");
                            }
                            break;
                        case "ServiceId.json":
                            Utilities.Log.Info(file + " file start checking!");
                            isFileOK = FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.ServiceIdPath, ref GlobalData.ServiceIdConfig);
                            if (!isFileOK)
                            {
                                message = file + " が存在しません。";
                                Utilities.Log.Error("▲ " + file + " file is not exist!");
                            }
                            else if (GlobalData.ServiceIdConfig.Count == 0)
                            {
                                isFileOK = false;
                                message = file + " の内容に問題があります。";
                                Utilities.Log.Error("▲ " + file + " file is empty!");
                            }
                            else
                            {
                                int serviceIdLg = GlobalData.ServiceIdConfig.Count;
                                for (int i = 0; i < serviceIdLg; i++)
                                {
                                    if (FileStruct.IsNullorEmpty(GlobalData.ServiceIdConfig[i]))
                                    {
                                        isFileOK = false;
                                        message = file + " の内容に問題があります。";
                                        string tempService = "";
                                        if (string.IsNullOrEmpty(GlobalData.ServiceIdConfig[i].brand_code)) tempService = (i + 1) + "(th)";
                                        else tempService = GlobalData.ServiceIdConfig[i].brand_code;
                                        Utilities.Log.Error("▲ " + file + " >> " + tempService + " service: setting is not correct.");
                                        break;
                                    }
                                }
                                if (isFileOK) Utilities.Log.Info(file + " done checking and no error found!");
                            }
                            break;
                        default:
                            break;
                    }
                    if (!isFileOK)
                    {
                        break;
                    }
                }
                if (isFileOK)
                {
                    isBrandNameOK = true;
                    Utilities.Log.Info("Start checking brand_name in Config.json file!");
                    if (GlobalData.BasicConfig.brand_name.Length == 0)
                    {
                        isBrandNameOK = false;
                        Utilities.Log.Error("▲ Config.json >> brand_name: list of services is empty.");
                    }
                    else
                    {
                        int cnt = 0;
                        foreach (var service in GlobalData.BasicConfig.brand_name)
                        {
                            string brandName = Utilities.GetBrandName(service);
                            if (GlobalData.ServiceNameCorrect.Contains(service) && !string.IsNullOrEmpty(brandName))
                            {
                                cnt++;
                                break;
                            }
                            /*if (!GlobalData.ServiceNameCorrect.Contains(key))
                            {
                                isBrandNameOK = false;
                                Utilities.Log.Error("▲ Config.json >> brand_name: " + key + " is not correct.");
                                break;
                            }
                            if (string.IsNullOrEmpty(GlobalData.BasicConfig.brand_name[key]))
                            {
                                isBrandNameOK = false;
                                Utilities.Log.Error("▲ Config.json >> brand_name: value of " + key + " is null or empty.");
                                break;
                            }*/
                        }
                        if (cnt == 0) Utilities.Log.Error("▲ Config.json >> brand_name: there are no services that match the services of system and brand_code in ServiceId.json.");
                    }
                    if (!isBrandNameOK)
                    {
                        msgCode = "A0001";
                        GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                        {
                            BtnVisibility = "Visible",
                            BtnContent = "終了",
                            BtnCommand = new Command(Utilities.ExitApp)
                        };
                    }
                    else
                    {
                        Utilities.Log.Info("Done checking brand_name in Config.json file and no error found!");
                        is_btn_order_SubtractValue_OK = true;
                        Utilities.Log.Info("Start checking btn_order_SubtractValue in Config.json file!");
                        if (GlobalData.BasicConfig.btn_order_SubtractValue.Length == 0)
                        {
                            is_btn_order_SubtractValue_OK = false;
                            Utilities.Log.Error("▲ Config.json >> btn_order_SubtractValue: list of services is empty.");
                        }
                        else
                        {
                            int cnt = 0;
                            foreach (var service in GlobalData.BasicConfig.btn_order_SubtractValue)
                            {
                                string brandName = Utilities.GetBrandName(service);
                                if (GlobalData.ServiceNameCorrect.Contains(service) && !string.IsNullOrEmpty(brandName))
                                {
                                    cnt++;
                                    break;
                                }
                                /*if (!GlobalData.ServiceNameCorrect.Contains(service))
                                {
                                    is_btn_order_SubtractValue_OK = false;
                                    Utilities.Log.Error("▲ Config.json >> btn_order_SubtractValue: " + service + " is not correct.");
                                    break;
                                }*/
                                if (cnt == 0) Utilities.Log.Error("▲ Config.json >> btn_order_SubtractValue: there are no services that match the services of system and brand_code in ServiceId.json.");
                            }
                        }
                        if (!is_btn_order_SubtractValue_OK)
                        {
                            msgCode = "A0002";
                            GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                            {
                                BtnVisibility = "Visible",
                                BtnContent = "終了",
                                BtnCommand = new Command(Utilities.ExitApp)
                            };
                        }
                        else
                        {
                            Utilities.Log.Info("Done checking btn_order_SubtractValue in Config.json file and no error found!");
                            /*// Health Check
                            Utilities.Log.Info("Health Check is started!");
                            FileStruct.SaturnAPIResponse result = await SaturnAPI.HealthCheck();
                            if (result == null)
                            {
                                msgCode = "A0003";
                                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                {
                                    BtnVisibility = "Visible",
                                    BtnContent = "終了",
                                    BtnCommand = new Command(Utilities.ExitApp)
                                };

                                Utilities.Log.Error("Health Check is done, timeout when waiting to receive the response!");
                            }
                            else if (result.controlInfo.procResult != "0")
                            {
                                msgCode = "A0004";
                                FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                {
                                    Message = msgMode.msg1
                                                    .Replace("[エラーコード詳細1]", $"{result.controlInfo.errorCodeDetail1}")
                                                    .Replace("[エラーコード詳細2]", $"{result.controlInfo.errorCodeDetail2}"),
                                    BtnVisibility = "Visible",
                                    BtnContent = "終了",
                                    BtnCommand = new Command(Utilities.ExitApp)
                                };

                                Utilities.Log.Error("Health Check is done, response is abnormal, procResult = " + result.controlInfo.procResult + "!");
                            }
                            else if (result.bizInfo.posWaitingStatus != "1")
                            {
                                msgCode = "A0005";
                                FileStruct.MsgMode msgMode = FileStruct.Find(GlobalData.MsgModeConfig, m => m.msg_code == msgCode, msgCode);
                                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                {
                                    Message = msgMode.msg1.Replace("[Arch状態]", $"{result.bizInfo.posWaitingStatus}"),
                                    BtnVisibility = "Visible",
                                    BtnContent = "終了",
                                    BtnCommand = new Command(Utilities.ExitApp)
                                };

                                Utilities.Log.Error("Health Check is done, Arch status is not Idle, posWaitingStatus = " + result.bizInfo.posWaitingStatus + "!");
                            }
                            else*/
                            {
                                // isHealthCheckOK = true;
                                msgCode = "A0006";
                                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties
                                {
                                    BtnVisibility = "Visible",
                                    BtnContent = "OK",
                                    BtnCommand = new Command(WinAPI.HideWindow)
                                };
                                // Utilities.Log.Info("Health Check is done and no error found!");
                            }
                        }
                    }
                }
                
                if (GlobalData.BasicConfig.customer_display_flag == "0")
                {
                    IntPtr hwnd = WinAPI.FindWindowByCaption(IntPtr.Zero, GlobalData.CustomerWindowName);
                    WinAPI.ShowWindow(hwnd, WinAPI.SW_HIDE);
                }
                if (!isFileOK)
                {
                    viewModel_emMessage = new ViewModel_emMessage
                    {
                        ListHeaderTitle = new List<string> { "電子マネー", "" },
                        Message = message,
                        SignalBarColor = "#FF2C00",
                        HeaderFooterColor = "#FFFFFF",
                        HeaderTitleColor = "#828282",
                        BackgroundColor = "#E1E1E1",
                        BodyTextColor = "#254474",
                        ButtonBorderColor = "#254474",
                        NormalButtonColor = "#EDEDED",
                        NormalButtonLabelColor = "#254474",
                        BtnVisibility = "Visible",
                        BtnContent = "終了",
                        BtnCommand = new Command(Utilities.ExitApp),
                    };
                    Session.MainViewModel.CurrentViewModel = viewModel_emMessage;

                    customerViewModel_emMessage = new ViewModel_emMessage
                    {
                        ListHeaderTitle = new List<string> { "電子マネー", "" },
                        Message = "設定内容に問題があるため\nアプリを終了します。",
                        SignalBarColor = "#FF2C00",
                        HeaderFooterColor = "#FFFFFF",
                        HeaderTitleColor = "#828282",
                        BackgroundColor = "#E1E1E1",
                        BodyTextColor = "#254474",
                        ButtonBorderColor = "#254474",
                        NormalButtonColor = "#EDEDED",
                        NormalButtonLabelColor = "#254474",
                    };
                    Session.MainViewModel.CurrentCustomerViewModel = customerViewModel_emMessage;
                }
                else
                    Session.MainViewModel.LoadScreen_emMessage(msgCode);
                if (isFileOK && isBrandNameOK && is_btn_order_SubtractValue_OK) // && isHealthCheckOK)
                {
                    GlobalData.CustomMessageID = WinAPI.RegisterWindowMessage(GlobalData.PosIFConfig.msg_name);

                    bool readPrinterOK = FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.PosPrinterPath, ref GlobalData.PrinterData)
                                         && !FileStruct.IsNullorEmpty(GlobalData.PrinterData);
                    bool loginDataOK = FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.LoginPath, ref GlobalData.LoginData);

                    Utilities.InitDailyTotal();

                    GlobalData.IsPopupNotificationOpen = false;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GlobalData.mainWindow.PopupNotification.IsOpen = GlobalData.IsPopupNotificationOpen;
                        GlobalData.mainWindow.MessageNotification.Text = "";
                        GlobalData.mainWindow.MessageNotification.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GlobalData.BasicConfig.color.customer.txt));
                        GlobalData.mainWindow.PopupBtnOK.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GlobalData.BasicConfig.color.customer.normal_btn));
                        GlobalData.mainWindow.PopupBtnOK.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GlobalData.BasicConfig.color.customer.normal_btn_label));
                    }, System.Windows.Threading.DispatcherPriority.ContextIdle);

                    // Delete old log file (exceed the log_keep_days config) at startup
                    Utilities.DeleteOldLog();

                    await WinAPI.HideWindowAfterTimeout(5000);
                    Session.ScreenState.NextState = StateMachine.State.emIdle;
                }
                
            }
            return Session.ScreenState.GoToNextState(this);
        }
    }
}
