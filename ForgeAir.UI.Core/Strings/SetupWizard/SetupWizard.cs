using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ForgeAir.UI.Core.Strings.SetupWizard
{
    public class SetupWizard
    {
        public ResourceDictionary returnDict(string lang)
        {
            ResourceDictionary dict = new ResourceDictionary();
            dict.Source = new Uri("/Strings/SetupWizard/" + lang + ".xaml", UriKind.Relative);
            return dict;
        }
    }
}
