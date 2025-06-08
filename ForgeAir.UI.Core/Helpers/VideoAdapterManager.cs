using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ForgeAir.UI.Core.Helpers
{
    public class VideoAdapterManager
    {
        public List<string> getDisplayAdapters()
        {
            List<string> adapters = new List<string>();

            var screens = Screen.AllScreens;
            for (int i = 0; i < screens.Length; i++)
            {
                var screen = screens[i];
                string displayName = $"{i}: {screen.DeviceName} - {screen.Bounds.Width}x{screen.Bounds.Height}" +
                    (screen.Primary ? " (Primary)" : "");
                adapters.Add(displayName);
            }
            return adapters;
        } 
    }
}
