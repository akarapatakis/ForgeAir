using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Models;
using ForgeAir.Database;

namespace ForgeAir.Core.Services.TrackSelector.Interfaces
{
    public interface ITrackSelector
    {
        Task<TrackDTO> GetBestTrackAsync(ClockItem item, DateTime time);
    }

}
