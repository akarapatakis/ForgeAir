using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.Database.Interfaces
{
    public interface ITracksService
    {
        Task<IEnumerable<ForgeAir.Database.Models.Track>> GetTracks(int skip, int take);
        int GetTrackCount();
        Task<ForgeAir.Database.Models.Track> GetTrack(int id);
    }
}
