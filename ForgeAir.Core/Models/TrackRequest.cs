using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Database.Models;

namespace ForgeAir.Core.Models
{
    public class TrackRequest
    {
        // we dont use required modifier as it will break the trackrequestbuilder
        public SMSRequest SourceRequest { get; set; }
        public Track RequestedTrack { get; set; }

    }
}
