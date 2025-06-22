using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.Shared;
using ForgeAir.Playout.Views;
using ForgeAir.Playout.Views.VST;

namespace ForgeAir.Playout.Shared
{
    class SharedWindowInstances
    {
        public StudioWindow studioWindow {  get; set; }
        public VSTPluginEditor VSTeditor { get; set; }
        public SettingsWindow settingsWindow { get; set; }
        public ExtraWindow extraWindow { get; set; }


        private static SharedWindowInstances? instance;
        public static SharedWindowInstances Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SharedWindowInstances();
                }
                return instance;
            }
        }
    }
}
