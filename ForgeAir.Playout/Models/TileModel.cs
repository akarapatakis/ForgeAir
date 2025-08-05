using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.IconPacks;

namespace ForgeAir.Playout.Models
{
    public class TileModel
    {
        public string Title { get; set; }
        public SolidColorBrush Color { get; set; }
        public PackIconRemixIconKind Icon { get; set; }

        public ICommand Command { get; set; }
    }
}
