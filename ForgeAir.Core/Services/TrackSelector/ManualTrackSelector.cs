using ForgeAir.Core.DTO;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.TrackSelector.Enums;
using ForgeAir.Core.Services.TrackSelector.Interfaces;
using ForgeAir.Database.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.TrackSelector
{
    public class ManualTrackSelector : ITrackSelector
    {
        public TrackSelectionMode SelectionMode => TrackSelectionMode.Manual;

        public Task<TrackDTO> GetBestTrackAsync(ClockItem item, DateTime time, TrackType trackType = TrackType.None)
        {
            return null;
        }
    }
}
