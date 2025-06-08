using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Database.Models;

namespace ForgeAir.Core.AudienceRequests.Models
{
    public class TrackRequest
    {
        public string phoneNumber { get; set; }
        public string name { get; set; }

        public Track track { get; set; }

    }
}
