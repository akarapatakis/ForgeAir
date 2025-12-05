using ForgeAir.Playout.Bootstrappers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Playout.Instances
{
    /*
        this is a complete mess and for some fucking reason it doesn't want to work through DI and i
        am doing stupid and bad workaround. 

        i mean it works but it's a complete mess.
     
        todo: ditch this and do it properly!
     */
    public class StationsInstance
    {
        public ObservableCollection<StationBootstrapper> Stations { get; } = new();

        private static StationsInstance _instance;
        public static StationsInstance Instance => _instance ?? (_instance = new StationsInstance());
    }
}
