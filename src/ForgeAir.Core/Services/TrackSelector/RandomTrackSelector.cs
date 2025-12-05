using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Events;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.Database;
using ForgeAir.Core.Services.Database.Interfaces;
using ForgeAir.Core.Services.TrackSelector.Enums;
using ForgeAir.Core.Services.TrackSelector.Interfaces;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Database.Models.Enums;

namespace ForgeAir.Core.Services.TrackSelector
{
    public class RandomTrackSelector : ITrackSelector
    {
        private readonly ITracksService trackRepository;
        private readonly Random random = new();

        
        public RandomTrackSelector(ITracksService _trackRepository)
        {
            trackRepository = _trackRepository;
        }

        public TrackSelectionMode SelectionMode => TrackSelectionMode.Random;

        public async Task<TrackDTO> GetBestTrackAsync(ClockItem item, DateTime time, TrackType trackType = TrackType.None)
        {
            var validTracks = await trackRepository.GetByConditionAsync(
                ModelTypesEnum.Track,
                t => (trackType == TrackType.None || t.TrackType == trackType)
                     && t.TrackStatus == TrackStatus.Enabled);

            // fallback if none of this type
            if (!validTracks.Any())
            {
                if (trackType != TrackType.None)
                    return await GetBestTrackAsync(item, time, TrackType.None);
                return null;
            }

            // Pick a random existing track
            var count = validTracks.Count();
            var randomIndex = random.Next(count);
            var selectedTrack = validTracks.ElementAt(randomIndex);

            // Validate file exists
            if (!File.Exists(selectedTrack.FilePath))
                return null;

            return selectedTrack;
        }

    }
}

