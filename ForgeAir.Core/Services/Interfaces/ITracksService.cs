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
        Task<IEnumerable<Database.Models.Track>> GetTracks(int skip, int take);
        int GetTrackCount();
        Task<Database.Models.Track> GetTrack(int id);
    }
}
