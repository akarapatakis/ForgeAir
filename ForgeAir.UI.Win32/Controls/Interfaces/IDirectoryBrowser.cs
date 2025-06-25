using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ForgeAir.UI.Win32.Controls.Interfaces
{
    public interface IDirectoryBrowser
    {
        string Title { get; set; }
        Environment.SpecialFolder RootDirectory { get; set; }
         string SelectedPath { get; }
        bool ShowDialog();
    }
}
