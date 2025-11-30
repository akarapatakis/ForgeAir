using ForgeAir.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.MetadataExports.Formats
{
    public interface IFormat
    {
        public void WriteNowPlaying(NowPlayingModel nP);
    }
}
