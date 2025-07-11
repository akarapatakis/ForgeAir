using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.Database;
using ForgeAir.Core.Services.TrackSelector.Interfaces;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database;
using ForgeAir.Database.Models;

namespace ForgeAir.Core.Services.TrackSelector
{
    public class RandomTrackSelector : ITrackSelector
    {
        private readonly RepositoryService<TrackDTO> trackRepository;
        private readonly Random random = new();


        public RandomTrackSelector(RepositoryService<TrackDTO> _trackRepository)
        {
            trackRepository = _trackRepository;
        }

        public async Task<TrackDTO> GetBestTrackAsync(ClockItem item, DateTime time)
        {
            int trackCount = await trackRepository.GetCountOf(ModelTypesEnum.Track);
            if (trackCount == 0)
            {
                return null;
            }
            int trackId = random.Next(1, trackCount);
            var track = await trackRepository.GetSpecificById(ModelTypesEnum.Track, trackId);

            return track;
        }
    }
}
