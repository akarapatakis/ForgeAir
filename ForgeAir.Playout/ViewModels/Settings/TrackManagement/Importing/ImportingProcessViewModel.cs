using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.Importers;
using ForgeAir.Core.Services.Importers.Interfaces;
using ForgeAir.Database;

namespace ForgeAir.Playout.ViewModels.Settings.TrackManagement.Importing
{
    public class ImportingProcessViewModel : Screen
    {
        private readonly ForgeAirDbContextFactory factory = new();
        public ITrackImporter trackImporter;
        private readonly ICollection<TrackImportModel> imports;
        public ImportingProcessViewModel(ICollection<TrackImportModel> tracksCollection) {
            imports = tracksCollection;
            trackImporter = new TrackImporter();
        }

        public async Task Add()
        {
            foreach (var track in imports)
            {
               track.ImportStatus = await Task.Run(() => trackImporter.createTrackAsync(track));
            }
        }
        public async Task AddStream()
        {
            foreach (var track in imports)
            {
                await Task.Run(() => trackImporter.createNetStreamTrack(track));
            }
        }
    }
}
