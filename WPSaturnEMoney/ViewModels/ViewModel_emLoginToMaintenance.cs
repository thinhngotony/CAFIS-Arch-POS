using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPSaturnEMoney.ViewModels
{
    class ViewModel_emLoginToMaintenance : BaseViewModel
    {
        public string TitleColor { get; set; }
        public string BodyTextColor { get; set; }
        public string BorderLineColor { get; set; }
        public string NumberColor { get; set; }
        public string ButtonBorderColor { get; set; }
        public string NumPadButtonColor { get; set; }
        public string NumPadButtonLabelColor { get; set; }
        public string ButtonPressedColor { get; set; }
        public string ButtonPressedLabelColor { get; set; }
        public string ClearButtonColor { get; set; }
        public string ClearButtonLabelColor { get; set; }
        public string InputCodeError { get; set; }
        public string Message { get; set; }
        public override void UpdateView()
        {
        }
    }
}
