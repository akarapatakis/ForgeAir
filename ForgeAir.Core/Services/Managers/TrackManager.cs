using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Database.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace ForgeAir.Core.Services.Managers
{
    public class TrackManager
    {
        private readonly TrackDTO track;

        public TrackManager(TrackDTO _track) { 
            track = _track;
        }
        public async Task Delete()
        {
            using (var _context = new ForgeAirDbContext()) {
                 _context.Tracks.Remove(TrackDTO.ToEntity(track));
                await _context.SaveChangesAsync();
            }
            return;
        }
        public async Task ChangeStatus()
        {
            TrackStatus newStatus = track.TrackStatus == TrackStatus.Enabled ? TrackStatus.Disabled : TrackStatus.Enabled;
            using (var _context = new ForgeAirDbContext())
            {
                track.TrackStatus = newStatus;
                await _context.SaveChangesAsync();
            }
            return;
        }
    }
}
