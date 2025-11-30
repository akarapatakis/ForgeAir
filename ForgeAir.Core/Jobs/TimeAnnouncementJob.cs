using ForgeAir.Core.Models;
using ForgeAir.Core.Services.AudioPlayout.Interfaces;
using ForgeAir.Core.Services.TrackSelector;
using ForgeAir.Core.Services.TrackSelector.Interfaces;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Jobs
{
    public class TimeAnnouncementJob : IJob
    {

        private readonly IQueueService _queueService;
        private readonly TrackSelectorService _trackSelector;

        public TimeAnnouncementJob(IQueueService queue, TrackSelectorService trackSelector) {

            _queueService = queue;
            _trackSelector = trackSelector;
        }

        //todo: remove hardcoded paths
        public async Task Execute(IJobExecutionContext context)
        {
            var time = DateTime.Now;
            var jingle = await Task.Run(() => _trackSelector.CurrentSelector.GetBestTrackAsync(null, time, Database.Models.Enums.TrackType.Jingle));
            if (jingle != null){
                _queueService.EnqueueTop(jingle); // always add a jingle after announcement

            }

            switch (time.Hour)
            {
                case 0:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\12.mp3", Id=090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT"  });
                    break;
                case 1:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\1.mp3", Id = 090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT" });
                    break;
                case 2:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\2.mp3", Id = 090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT" });
                    break;
                case 3:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\3.mp3", Id = 090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT" });
                    break;
                case 4:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\4.mp3", Id = 090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT" });
                    break;
                case 5:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\5.mp3", Id = 090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT" });
                    break;
                case 6:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\6.mp3", Id = 090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT" });
                    break;
                case 7:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\7.mp3", Id = 090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT" });
                    break;
                case 8:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\8.mp3", Id = 090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT" });
                    break;
                case 9:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\9.mp3", Id = 090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT" });
                    break;
                case 10:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\10.mp3", Id = 090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT" });
                    break;
                case 11:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\11.mp3", Id = 090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT" });
                    break;
                case 12:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\12.mp3", Id = 090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT" });
                    break;
                case 13:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\1.mp3", Id = 090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT" });
                    break;
                case 14:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\2.mp3", Id = 090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT" });
                    break;
                case 15:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\3.mp3", Id = 090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT" });
                    break;
                case 16:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\4.mp3", Id = 090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT" });
                    break;
                case 17:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\5.mp3", Id = 090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT" });
                    break;
                case 18:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\6.mp3", Id = 090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT" });
                    break;
                case 19:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\7.mp3", Id = 090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT" });
                    break;
                case 20:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\8.mp3", Id = 090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT" });
                    break;
                case 21:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\9.mp3", Id = 090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT" });
                    break;
                case 22:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\10.mp3", Id = 090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT" });
                    break;
                case 23:
                    _queueService.EnqueueTop(new DTO.TrackDTO() { FilePath = "D:\\Ανακοίνωση Ώρας\\11.mp3", Id = 090923, TrackType = Database.Models.Enums.TrackType.Jingle, IsDynamicJingleAsset = true, StartPoint = TimeSpan.Zero, TrackStatus = Database.Models.Enums.TrackStatus.Enabled, Title = "DYNAMIC JINGLE | TIME ANNOUNCEMENT" });
                    break;
            }

            return;
        }
    }
}
