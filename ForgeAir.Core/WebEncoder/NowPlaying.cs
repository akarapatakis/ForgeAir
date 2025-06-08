using ForgeAir.Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.WebEncoder
{
    public class NowPlaying
    {
        public NowPlaying() { }

        public Task<String> CreateString() {
            if (AudioPlayerShared.Instance.currentTrack != null)
            {
                return Task.FromResult($"{WebEncoderNowPlaying.Instance.PreText}{AudioPlayerShared.Instance.currentTrack.DisplayArtists} - {AudioPlayerShared.Instance.currentTrack.Title}{WebEncoderNowPlaying.Instance.PostText}");
            }
            else
            {
                return Task.FromResult(WebEncoderNowPlaying.Instance.noTrackPlaying);
            }
        }
        
    }
}
