using ForgeAir.Core.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.Interfaces
{
    public interface ITracksService
    {
        Task<IEnumerable<Track>> GetTracks(int skip, int take);
        Task<int> GetTrackCountAsync(string query = "");
        Task<Track> GetTrack(int id);
    }
}
