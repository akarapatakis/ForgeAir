using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.TrackSelector.Interfaces;

namespace ForgeAir.Core.Services.TrackSelector
{
    public class MLTrackSelector : ITrackSelector
    {
        public Task<TrackDTO> GetBestTrackAsync(ClockItem item, DateTime time)
        {
            throw new NotImplementedException();
        }
    }
}
