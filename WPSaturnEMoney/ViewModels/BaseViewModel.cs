using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPSaturnEMoney.ViewModels
{
    /// <summary>
    /// The 'BaseViewModel' abstract class
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public string HeaderTitle { get; set; }
        public List<string> ListHeaderTitle { get; set; }
        public string HeaderFooterColor { get; set; }
        public string HeaderTitleColor { get; set; }
        public string SignalBarColor { get; set; }
        public string BackgroundColor { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public abstract void UpdateView();
    }
}
