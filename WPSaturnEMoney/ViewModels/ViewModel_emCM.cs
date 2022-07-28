using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPSaturnEMoney.Common;
using WPSaturnEMoney.State;

namespace WPSaturnEMoney.ViewModels
{
    class ViewModel_emCM : BaseViewModel
    {
        public string BodyTextColor { get; set; }
        public string ErrorMessage { get; set; }
        public string ButtonLabel { get; set; }
        public int ButtonLabelSize { get; set; }
        public string ButtonBorderColor { get; set; }
        public string NormalButtonColor { get; set; }
        public string NormalButtonLabelColor { get; set; }
        public ICommand StopWaitingCommand { get; }

        public ViewModel_emCM()
        {
            StopWaitingCommand = new Command(StopWaiting);
        }

        private void StopWaiting()
        {
            Utilities.Log.Info($"Press button [{ButtonLabel}]");
            Session.TimerCount = 0;
        }

        public override void UpdateView()
        {
        }
    }
}
