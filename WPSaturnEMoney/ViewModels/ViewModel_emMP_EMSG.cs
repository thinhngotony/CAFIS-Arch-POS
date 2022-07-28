using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPSaturnEMoney.ViewModels
{
    public class ViewModel_emMP_EMSG : BaseViewModel
    {
        public string TitleColor { get; set; }
        public string BodyTextColor { get; set; }
        public string ButtonBorderColor { get; set; }
        public string NormalButtonColor { get; set; }
        public string NormalButtonLabelColor { get; set; }
        public string BackButtonColor { get; set; }
        public string BackButtonLabelColor { get; set; }
        public string ErrorMessage { get; set; }

        public override void UpdateView()
        {
            OnPropertyChanged("TitleColor");
            OnPropertyChanged("BodyTextColor");
            OnPropertyChanged("ButtonBorderColor");
            OnPropertyChanged("NormalButtonColor");
            OnPropertyChanged("NormalButtonLabelColor");
            OnPropertyChanged("BackButtonColor");
            OnPropertyChanged("BackButtonLabelColor");
            OnPropertyChanged("ErrorMessage");
        }
    }
}
