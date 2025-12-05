using ForgeAir.Core.DTO;
using ForgeAir.Core.Models;
using ForgeAir.Core.Models.Enums;
using ForgeAir.Core.Services.Database;
using ForgeAir.Core.Services.Database.Interfaces;
using ForgeAir.Core.Services.TrackSelector.Enums;
using ForgeAir.Core.Services.TrackSelector.Interfaces;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.TrackSelector
{
    public class ClockTrackSelector : ITrackSelector
    {
        private readonly ITracksService _trackRepository;
        public TrackSelectionMode SelectionMode => TrackSelectionMode.Clock;

        public ClockTrackSelector(ITracksService trackRepository)
        {
            _trackRepository = trackRepository;
        }

        public async Task<TrackDTO?> GetBestTrackAsync(ClockItem item, DateTime time, TrackType trackType = TrackType.None)
        {

            switch (item.Type)
            {
                case ClockItemType.TrackFromCategory:
                    var category = item.Parameter;
                    var tracks = await _trackRepository.GetByCategory(new CategoryDTO() { Name = category });

                    if (tracks == null || !tracks.Any())
                        return null;

                    Random random = new Random();

                    int trackCount = tracks.Count;
                    if (trackCount == 0)
                    {
                        return null;
                    }
                    int trackId = random.Next(1, trackCount);

                    var track = tracks[trackId]; // access by list index, not by ID match


                    // Optional: later plug in ML.NET scoring here

                    return track;
                    break;

                case ClockItemType.TrackFromArtist:

                    var artist = item.Parameter;
                    var tracksfromDb = await _trackRepository.GetByArtist(new ArtistDTO() { Name = artist });

                    if (tracksfromDb == null || !tracksfromDb.Any())
                        return null;

                    Random randomTrack = new Random();

                    int trackcount = tracksfromDb.Count;
                    if (trackcount == 0)
                    {
                        return null;
                    }
                    int trackid = randomTrack.Next(1, trackcount);

                    var besttrack = tracksfromDb[trackid]; // access by list index, not by ID match

                    return besttrack;
                    break;

                default:
                    return null;

                    // Optional: later plug in ML.NET scoring here


            }


        }
    }

}
