// it literally chooses random tracks based on nothing!

using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Pilots
{
    public class RandomPilot
    {


        public async Task<Database.Models.Track?> selectRandomTrack()
        {

            var factory = new ForgeAirDbContextFactory();
            Services.Repository<Database.Models.Track> trackDb = new Services.Repository<Database.Models.Track>(factory);

            Random random = new Random();

            int trackCount = await trackDb.GetCountOf(ModelTypesEnum.Track);
            if (trackCount == 0)
            {
                return null;
            }
            int trackId = random.Next(1, trackCount);
            var track = await trackDb.GetSpecificById(ModelTypesEnum.Track, trackId);
            
            return await Task.FromResult((Database.Models.Track)track);

        }

        public async Task<bool> WillHookOnTrack(Database.Models.Track track)
        {
            if (track.Intro == null) { return false; }
            Random random = new Random();
            int trackId = random.Next(0, 1);
            if (trackId == 0) { return false; }
            else { return true; }
        }

        

        public async Task<Database.Models.Track?> selectRandomSweeper()
        {
            var factory = new ForgeAirDbContextFactory();

            Services.Repository<Database.Models.Track> trackDb = new Services.Repository<Database.Models.Track>(factory);

            Random random = new Random();

            int trackCount = await trackDb.GetCountOf(ModelTypesEnum.Track);
            if (trackCount == 0)
            {
                return null;
            }
            int trackId = random.Next(1, trackCount);
            var track = await trackDb.GetSpecificById(ModelTypesEnum.Track, trackId);

            return await Task.FromResult((Database.Models.Track)track);

        }
    }
}
