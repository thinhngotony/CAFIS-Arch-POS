using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;
using Microsoft.PointOfService;

namespace WPSaturnEMoney.State
{
    class State_emMT_F : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emMT_F)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emMT_F;
                Session.MainViewModel.LoadScreen_emMT_F();

                // Create and print new receipt from unfinished progressing data (ConvReceipt),
                // if convert receipt error, notify to the upper level and printing will be stopped.
                if (!Session.IsPrintRetryPreState_emMT_F)
                {
                    Utilities.Log.Info($"Get ConvertReceipt, PreCurrentServiceSV: {GlobalData.PreCurrentServiceSV}");
                    var receiptData = Utilities.ConvertReceipt();
                    // Utilities.PrintConvertedReceipt(receiptData);
                    if (GlobalData.IsConvertReceiptError)
                    {
                        Utilities.Log.Error("▲ Convert Receipt Error!");
                    }
                }

                GlobalData.Data_ToPosDat.service = "EMONEY";
                GlobalData.Data_ToPosDat.sequence = GlobalData.Data_FromPosDat.sequence;
                GlobalData.Data_ToPosDat.last_operator = GlobalData.LastOperator;
                GlobalData.Data_ToPosDat.result = "1"; // payment successful
                if (Session.IsPrintRetryPreState_emMT_F)
                {
                    // Utilities.PrintReceiptOperation();

                    GlobalData.Data_ToPosDat.SettledAmount = GlobalData.SettledAmount.ToString();
                    GlobalData.Data_ToPosDat.CurrentService = GlobalData.CurrentService;
                    GlobalData.Data_ToPosDat.statementID = GlobalData.statementID;
                    Session.IsPrintRetryPreState_emMT_F = false;

                    Utilities.UpdateTotalInfo(GlobalData.CurrentService, GlobalData.SettledAmount, Utilities.TotalRecordType.SS);
                }
                else
                {
                    GlobalData.Data_ToPosDat.SettledAmount = GlobalData.PreSettledAmountSV.ToString();
                    GlobalData.Data_ToPosDat.CurrentService = GlobalData.PreCurrentServiceSV;
                    GlobalData.Data_ToPosDat.statementID = GlobalData.PreStatementIDSV;

                    Utilities.UpdateTotalInfo(GlobalData.PreCurrentServiceSV, GlobalData.PreSettledAmountSV, Utilities.TotalRecordType.NASS);
                }
                if (!GlobalData.IsPrintError)
                {
                    await WinAPI.EndTransaction();
                }
                if (GlobalData.MPErrorCode == ErrorCode.Extended && GlobalData.MPErrorCodeExtended == 201)
                {
                    FileStruct.MsgMode msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "B0022");
                    //GlobalData.ViewModelProperties.ListHeaderTitle = msgMode.header_title2;
                    GlobalData.ViewModelProperties.Message = msgMode.msg2;
                    //GlobalData.CustomerViewModelProperties.ListHeaderTitle = msgMode.header_title3;
                    GlobalData.CustomerViewModelProperties.Message = msgMode.msg3;
                    Session.MainViewModel.LoadScreen_emM_EMSG();
                }
                else if (GlobalData.MPErrorCode == ErrorCode.Extended && GlobalData.MPErrorCodeExtended == 203)
                {
                    FileStruct.MsgMode msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "B0021");
                    //GlobalData.ViewModelProperties.ListHeaderTitle = msgMode.header_title2;
                    GlobalData.ViewModelProperties.Message = msgMode.msg2;
                    //GlobalData.CustomerViewModelProperties.ListHeaderTitle = msgMode.header_title3;
                    GlobalData.CustomerViewModelProperties.Message = msgMode.msg3;
                    Session.MainViewModel.LoadScreen_emM_EMSG();
                }
                else
                {
                    FileStruct.MsgMode msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "B0023");
                    //GlobalData.ViewModelProperties.ListHeaderTitle = msgMode.header_title2;
                    GlobalData.ViewModelProperties.Message = msgMode.msg2;
                    //GlobalData.CustomerViewModelProperties.ListHeaderTitle = msgMode.header_title3;
                    GlobalData.CustomerViewModelProperties.Message = msgMode.msg3;
                    Session.MainViewModel.LoadScreen_emM_EMSG();
                }
                GlobalData.IsPrintError = false;
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
