using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Models.Enums;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.TrackSelector.Interfaces;
using ForgeAir.Core.Services.Database;
using ForgeAir.Core.Tracks.Enums;

namespace ForgeAir.Core.Services.TrackSelector
{
    public class TrackSelectorService : ITrackSelectorService
    {
        private readonly RepositoryService<TrackDTO> _trackRepository;

        public TrackSelectorService(RepositoryService<TrackDTO> trackRepository)
        {
            _trackRepository = trackRepository;
        }

        public async Task<TrackDTO?> GetBestTrackAsync(ClockItem item, DateTime time)
        {

            switch (item.Type)
            {
                case ClockItemType.TrackFromCategory:
                    var category = item.Parameter;
                    var tracks = await _trackRepository.GetTracksByCategoryAsync(new CategoryDTO() { Name = category });

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
                    var tracksfromDb = await _trackRepository.GetTracksByArtistAsync(new ArtistDTO() { Name = artist });

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
