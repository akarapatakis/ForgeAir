using ForgeAir.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Models
{
    public class DynamicJingle
    {
        public bool Enabled { get; set; }
        public TrafficAnnouncement? TrafficAnnouncement { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public DynamicJingleType Type { get; set; }
    }
}