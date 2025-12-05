using ForgeAir.Core.DTO;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Database.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.Managers
{
    public class TrackManager
    {
        private readonly TrackDTO track;
        private readonly IDbContextFactory<ForgeAirDbContext> _dbContextFactory;

        public TrackManager(IDbContextFactory<ForgeAirDbContext> dbContextFactory, TrackDTO _track) { 
            _dbContextFactory = dbContextFactory;
            track = _track;
        }
        public async Task Delete()
        {
            using (var _context = _dbContextFactory.CreateDbContext())
            {
                _context.Tracks.Remove(TrackDTO.ToEntity(track));
                await _context.SaveChangesAsync();
            }
            return;
        }
        public async Task ChangeStatus()
        {
            TrackStatus newStatus = track.TrackStatus == TrackStatus.Enabled ? TrackStatus.Disabled : TrackStatus.Enabled;
            using (var _context = _dbContextFactory.CreateDbContext())
            {
                track.TrackStatus = newStatus;
                await _context.SaveChangesAsync();
            }
            return;
        }
    }
}
