using ForgeAir.Database;
using ForgeAir.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.Managers
{
    public class AdPackManager
    {
        private readonly IDbContextFactory<ForgeAirDbContext> _dbContextFactory;

        public AdPackManager(IDbContextFactory<ForgeAirDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<AdPack?> GetAdPackByTime(DateTime dateTime)
        {
            using var context = _dbContextFactory.CreateDbContext();

            var nowHour = dateTime.Hour;
            var nowMinute = dateTime.Minute;

            try
            {
                // Pull AdPacks and related Ads into memory first
                var adPacks = await context.AdPacks
                    .Include(p => p.Ads)
                        .ThenInclude(a => a.Track)
                            .ThenInclude(t => t.TrackArtists)
                                .ThenInclude(at => at.Artist)
                    .Include(p => p.Ads)
                        .ThenInclude(a => a.Track)
                            .ThenInclude(t => t.Categories)
                    .ToListAsync();

                return adPacks.FirstOrDefault(p =>
                    p.ScheduledDateTimes.Contains(dateTime)
                    ||
                    (p.RepeatEveryDay &&
                     p.ScheduledDateTimes.Any(s => s.TimeOfDay.Hours == nowHour && s.TimeOfDay.Minutes == nowMinute))
                );
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.InnerException);
                throw;
            }
        }




        public async Task CreateOrUpdate(
            int? id,
            string name,
            ICollection<DateTime> schedTimes,
            bool active,
            bool repeat,
            ICollection<AdPackItem> tracks)
        {
            using var context = _dbContextFactory.CreateDbContext();

            AdPack pack;

            if (id.HasValue && id.Value != 0)
            {
                pack = await context.AdPacks
                    .Include(p => p.Ads)
                    .FirstOrDefaultAsync(p => p.Id == id.Value);

                if (pack == null)
                    throw new Exception("AdPack not found.");

                // Remove old items
                context.AdPackItems.RemoveRange(pack.Ads);

                pack.Name = name;
                pack.IsActive = active;
                pack.RepeatEveryDay = repeat;
                pack.ScheduledDateTimes = schedTimes;
            }
            else
            {
                pack = new AdPack
                {
                    Name = name,
                    IsActive = active,
                    RepeatEveryDay = repeat,
                    ScheduledDateTimes = schedTimes
                };

                await context.AdPacks.AddAsync(pack);
            }

            // IMPORTANT: create fresh entities
            pack.Ads = tracks.Select(t => new AdPackItem
            {
                TrackId = t.TrackId,
                PlayOrder = t.PlayOrder
            }).ToList();

            await context.SaveChangesAsync();
        }

    }
}
