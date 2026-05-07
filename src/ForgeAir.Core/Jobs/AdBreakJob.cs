using ForgeAir.Core.DTO;
using ForgeAir.Core.Services.AudioPlayout.Interfaces;
using ForgeAir.Core.Services.Managers;
using ForgeAir.Database.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Jobs
{
    public class AdBreakJob : IJob
    {
        private readonly IQueueService _queueService;
        private readonly AdPackManager _adPackManager;
        private DateTime _cachedDT;
        private static readonly HashSet<int> _enqueuedAdPackIds = new();
        public AdBreakJob(IQueueService queueService, AdPackManager adPackManager)
        {
            _cachedDT = DateTime.Now;
            _queueService = queueService;
            _adPackManager = adPackManager;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var time = DateTime.Now;
            if (time.Day > _cachedDT.Day) {  // clear if day changes
                _enqueuedAdPackIds.Clear();
            }

            var adPack = await _adPackManager.GetAdPackByTime(time);
            if (adPack != null)
            {
                if (!_enqueuedAdPackIds.Contains(adPack.Id))
                {
                    foreach (var ad in adPack.Ads
                        .OrderByDescending(x => x.PlayOrder))
                    {
                        _queueService.EnqueueTop(TrackDTO.FromEntity(ad.Track));
                    }
                    _enqueuedAdPackIds.Add(adPack.Id);
                }
            }
            _cachedDT = time;
        }
    }
}
