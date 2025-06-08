using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DXGI;

namespace ForgeAir.Core.VideoEngine
{
    public class VideoSubtitles
    {

        public string subtitles;
        public VideoSubtitles(string subtitlePath) { 
            

        }

        public async Task<Dictionary<string, TimeSpan>> renderSubtitle()
        {
            throw new NotImplementedException();
        }
    }
}
