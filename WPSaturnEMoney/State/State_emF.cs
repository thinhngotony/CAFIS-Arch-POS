using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;
using Microsoft.PointOfService;

namespace WPSaturnEMoney.State
{
    class State_emF : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emF)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emF;
                Session.MainViewModel.LoadScreen_emF();

                // Utilities.PlaySound("emf.wav");

                // Utilities.PrintReceiptOperation();
                
                GlobalData.Data_ToPosDat.service = "EMONEY";
                GlobalData.Data_ToPosDat.sequence = GlobalData.Data_FromPosDat.sequence;
                GlobalData.Data_ToPosDat.last_operator = GlobalData.LastOperator;
                GlobalData.Data_ToPosDat.result = "1"; // payment successful
                GlobalData.Data_ToPosDat.SettledAmount = GlobalData.SettledAmount.ToString();
                GlobalData.Data_ToPosDat.CurrentService = GlobalData.CurrentService;
                GlobalData.Data_ToPosDat.statementID = GlobalData.statementID;
                GlobalData.CurrentService = "EMONEY"; //FAKE DATA
                Utilities.UpdateTotalInfo(GlobalData.CurrentService, GlobalData.SettledAmount, Utilities.TotalRecordType.SS);

                if(!GlobalData.IsPrintError)
                {
                    await WinAPI.EndTransaction();
                }
                if(Session.MaintenanceMode != "Maintenance")
                {
                    if (GlobalData.MPErrorCode == ErrorCode.Extended && GlobalData.MPErrorCodeExtended == 201)
                    {
                        FileStruct.MsgMode msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "B0002");
                        GlobalData.ViewModelProperties.Message = msgMode.msg1;
                        GlobalData.CustomerViewModelProperties.Message = msgMode.msg3;
                    }
                    else if (GlobalData.MPErrorCode == ErrorCode.Extended && GlobalData.MPErrorCodeExtended == 203)
                    {
                        FileStruct.MsgMode msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "B0001");
                        GlobalData.ViewModelProperties.Message = msgMode.msg1;
                        GlobalData.CustomerViewModelProperties.Message = msgMode.msg3;
                    }
                    else
                    {
                        FileStruct.MsgMode msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "B0003");
                        GlobalData.ViewModelProperties.Message = msgMode.msg1;
                        GlobalData.CustomerViewModelProperties.Message = msgMode.msg3;
                    }
                    Session.ScreenState.NextState = StateMachine.State.emErrorToMaintenance;
                }
                else
                {
                    if (GlobalData.MPErrorCode == ErrorCode.Extended && GlobalData.MPErrorCodeExtended == 201)
                    {
                        FileStruct.MsgMode msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "B0022");
                        GlobalData.ViewModelProperties.Message = msgMode.msg2;
                        GlobalData.CustomerViewModelProperties.Message = msgMode.msg3;
                        Session.MainViewModel.LoadScreen_emM_EMSG();
                    }
                    else if (GlobalData.MPErrorCode == ErrorCode.Extended && GlobalData.MPErrorCodeExtended == 203)
                    {
                        FileStruct.MsgMode msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "B0021");
                        GlobalData.ViewModelProperties.Message = msgMode.msg2;
                        GlobalData.CustomerViewModelProperties.Message = msgMode.msg3;
                        Session.MainViewModel.LoadScreen_emM_EMSG();
                    }
                    else
                    {
                        FileStruct.MsgMode msgMode = GlobalData.MsgModeConfig.FirstOrDefault(m => m.msg_code == "B0023");
                        GlobalData.ViewModelProperties.Message = msgMode.msg2;
                        GlobalData.CustomerViewModelProperties.Message = msgMode.msg3;
                        Session.MainViewModel.LoadScreen_emM_EMSG();
                    }
                    GlobalData.IsPrintError = false;
                }
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
