using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Models;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database.Models;
using ForgeAir.Database.Models.Enums;

namespace ForgeAir.Core.Services.Importers.Interfaces
{
    public interface ITrackImporter
    {
        Task<Dictionary<ImportTrackStatusEnum, ImportTrackErrorsEnum>> CreateTrackAsync(TrackImportModel trackImport);
        Task<Dictionary<ImportTrackStatusEnum, ImportTrackErrorsEnum>> CreateNetStreamTrack(TrackImportModel stream);
    }
}
