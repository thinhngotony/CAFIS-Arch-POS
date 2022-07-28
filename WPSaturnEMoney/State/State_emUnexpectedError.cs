using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.ViewModels;

namespace WPSaturnEMoney.State
{
    class State_emUnexpectedError : IState
    {
        public async Task<IState> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            if (Session.ScreenState.CurrentState != StateMachine.State.emUnexpectedError)
            {
                Session.ScreenState.CurrentState = StateMachine.State.emUnexpectedError;
                ViewModel_emMessage viewModel_emMessage = new ViewModel_emMessage
                {
                    ListHeaderTitle = new List<string> { "電子マネー", "" },
                    HeaderFooterColor = "#FFFFFF",
                    HeaderTitleColor = "#828282",
                    SignalBarColor = "#FF2C00",
                    BackgroundColor = "#E1E1E1",
                    BodyTextColor = "#254474",
                    Message = "アプリケーションエラーが発生しました。",
                    BtnVisibility = "Visible", // "Visible";
                    BtnContent = "終了",
                    ButtonBorderColor = "#254474",
                    NormalButtonColor = "#EDEDED",
                    NormalButtonLabelColor = "#254474",
                    BtnCommand = new Command(Utilities.ExitApp)
                };
                Session.MainViewModel.CurrentViewModel = viewModel_emMessage;

                ViewModel_emMessage customerViewModel_emMessage = new ViewModel_emMessage
                {
                    ListHeaderTitle = new List<string> { "電子マネー", "" },
                    HeaderFooterColor = "#FFFFFF",
                    HeaderTitleColor = "#828282",
                    SignalBarColor = "#FF2C00",
                    BackgroundColor = "#E1E1E1",
                    BodyTextColor = "#254474",
                    Message = "アプリケーションエラーが発生しました。",
                };
                Session.MainViewModel.CurrentCustomerViewModel = customerViewModel_emMessage;
            }

            return Session.ScreenState.GoToNextState(this);
        }
    }
}
