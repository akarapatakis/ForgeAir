using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ForgeAir.Playout.App.Services
{
    public class LocalizationService : INotifyPropertyChanged
    {
        public static LocalizationService Instance { get; } = new LocalizationService();

        //public string Title => ForgeAir.UI.Core.Langs.;
        //public string StatusReady => Strings.Status_Ready;
        //public string Welcome => Strings.WelcomeMessage;
        // Add more...

        public void Refresh()
        {
            // Call this after language change to update bindings
            OnPropertyChanged("");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
