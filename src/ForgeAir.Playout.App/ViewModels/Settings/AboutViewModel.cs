using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Playout.ViewModels.Settings
{
    public class AboutViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        public AboutViewModel(IWindowManager manager) { }




        protected void OnPropertyChanged(string propName) =>
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

    }
}
