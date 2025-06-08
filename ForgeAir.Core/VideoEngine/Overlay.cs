using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.VideoEngine
{
    public class Overlay
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Screenshot { get; set; }
        public string Version { get; set; }
        public string OverlayScript { get; set; }
    }

    public class OverlayCollection
    {
        public List<Overlay> Overlay { get; set; }
    }

}
