using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.TrackSelector.Enums;
using ForgeAir.Database;
using ForgeAir.Database.Models.Enums;

namespace ForgeAir.Core.Services.TrackSelector.Interfaces
{
    public interface ITrackSelector
    {
        TrackSelectionMode SelectionMode { get; }
        Task<TrackDTO> GetBestTrackAsync(ClockItem item, DateTime time, TrackType trackType = TrackType.None);
    }

}
