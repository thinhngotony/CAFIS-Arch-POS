using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPSaturnEMoney.State;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.Models;

namespace WPSaturnEMoney.ViewModels
{
    class ViewModel_emMT_MSG : BaseViewModel
    {
        public string BodyTextColor { get; set; }
        public string ButtonBorderColor { get; set; }
        public string NormalButtonColor { get; set; }
        public string NormalButtonLabelColor { get; set; }
        public string ErrorMessage { get; set; }
        public ICommand GoBack_Command { get; }
        public ViewModel_emMT_MSG()
        {
            GoBack_Command = new Command(GoBack);
        }

        private void GoBack()
        {
            Utilities.Log.Info("Press button [戻る]");
            Session.ScreenState.NextState = StateMachine.State.emMaintenanceMenuFromError;
            Session.TimerCount = 0;
        }
        public override void UpdateView()
        {
        }
    }
}
