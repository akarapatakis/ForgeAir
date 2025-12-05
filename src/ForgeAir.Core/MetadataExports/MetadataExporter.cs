using ForgeAir.Core.DTO;
using ForgeAir.Core.Events;
using ForgeAir.Core.MetadataExports.Enums;
using ForgeAir.Core.MetadataExports.Formats;
using ForgeAir.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.MetadataExports
{
    public class MetadataExporter
    {
        private readonly IServiceProvider serviceProvider;
        private readonly NowPlayingModel nowPlayingModel;
        private readonly IFormat PlainTextOut;

        public MetadataExporter(IServiceProvider provider, TrackChangedEvent TrackChangedEvent)
        {
            TrackChangedEvent.TrackChanged += OnTrackChanged;
            if (TrackChangedEvent.CurrentTrack != null)
            {
                OnTrackChanged(TrackChangedEvent.CurrentTrack);
            }
            nowPlayingModel = provider.GetRequiredService<NowPlayingModel>();
            PlainTextOut = new PlainText(provider.GetRequiredService<StationPaths>());
        }

        private void OnTrackChanged(TrackDTO newTrack)
        {
            if (newTrack == null || newTrack.TrackType != Database.Models.Enums.TrackType.Song) {
                return;
            }
            nowPlayingModel.CurrentTrack = newTrack;

            PlainTextOut.WriteNowPlaying(nowPlayingModel);

        }

    }
}
