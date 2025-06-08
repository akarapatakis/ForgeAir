using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace ForgeAir.Core.Services.Managers
{
    public class TrackManager
    {
        private readonly Track track;

        public TrackManager(Track inTrack) { 
            track = inTrack;
        }
        public async Task Delete()
        {
            using (var _context = new ForgeAirDbContext()) {
                 _context.Tracks.Remove(track);
                await _context.SaveChangesAsync();

            }
            return;

        }
    }
}
