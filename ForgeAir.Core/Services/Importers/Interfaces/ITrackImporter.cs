using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Database.Models;
using ForgeAir.Database.Models.Enums;

namespace ForgeAir.Core.Services.Importers.Interfaces
{
    public interface ITrackImporter
    {
        Task<Track> createTrackAsync(string filename, TrackType type, TimeSpan crossfadeTime, bool isVideo=false, string? artist = null);
    }
}
