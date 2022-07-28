using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Models;

namespace WPSaturnEMoney.State
{
    class State_emMaintenanceMenu : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emMaintenanceMenu)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emMaintenanceMenu;

                GlobalData.ViewModelProperties = new FileStruct.ViewModelProperties();
                GlobalData.CustomerViewModelProperties = new FileStruct.ViewModelProperties();
                string lastSettlementBrandCode = "";
                string lastSettlementSeqNo = "";
                string btnLastMinTransInquiryVisibility = "Visible";
                string btnBalanceInquiryVisibility = "Visible";
                string btnLastMinTransSuccessFailedVisibility = "Visible";
                try
                {
                    using (StreamReader reader = new StreamReader(GlobalData.AppPath + GlobalData.LastSettlementBrandCodePath, Encoding.GetEncoding("Shift_JIS")))
                    {
                        lastSettlementBrandCode = reader.ReadToEnd();
                    }
                    using (StreamReader reader = new StreamReader(GlobalData.AppPath + GlobalData.LastSettlementSeqNoPath, Encoding.GetEncoding("Shift_JIS")))
                    {
                        lastSettlementSeqNo = reader.ReadToEnd();
                    }
                }
                catch (Exception)
                {

                }
                if (string.IsNullOrEmpty(lastSettlementBrandCode) || string.IsNullOrEmpty(lastSettlementSeqNo))
                {
                    btnLastMinTransInquiryVisibility = "Hidden";
                    btnBalanceInquiryVisibility = "Hidden";
                    btnLastMinTransSuccessFailedVisibility = "Hidden";
                }

                Session.MainViewModel.LoadScreen_emMaintenanceMenu(btnLastMinTransInquiryVisibility, btnBalanceInquiryVisibility, btnLastMinTransSuccessFailedVisibility);
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
