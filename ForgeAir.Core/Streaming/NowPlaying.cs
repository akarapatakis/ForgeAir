using ForgeAir.Core.Models;
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

        private readonly AppState _appState;
        public NowPlaying(AppState appState) {

            _appState = appState;
        }

       /*
        public Task<String> CreateString() {
            if (_appState.CurrentTrack != null)
            {
                return Task.FromResult($"{WebEncoderNowPlaying.Instance.PreText}{_appState.CurrentTrack.DisplayArtists} - {_appState.CurrentTrack.Title}{WebEncoderNowPlaying.Instance.PostText}");
            }
            else
            {
                return Task.FromResult(WebEncoderNowPlaying.Instance.noTrackPlaying);
            }
        } */
        
    }
}
