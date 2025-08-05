using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.Importers;
using ForgeAir.Core.Services.Importers.Interfaces;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database;
using HandyControl.Data;
using Microsoft.Extensions.DependencyInjection;

namespace ForgeAir.Playout.ViewModels.Settings.TrackManagement.Importing
{
    public class ImportingProcessViewModel : Screen
    {
        public ITrackImporter trackImporter;
        public ObservableCollection<TrackImportModel> Imports { get; }
        private readonly IServiceProvider _provider;

        public bool ImportFinished { get; set; } = false;

        public ImportingProcessViewModel(IServiceProvider provider, ICollection<TrackImportModel> tracksCollection)
        {
            Imports = new ObservableCollection<TrackImportModel>(tracksCollection);
            _provider = provider;
            trackImporter = _provider.GetRequiredService<ITrackImporter>();
        }
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            await Add();
        }
        public void Done()
        {
            TryCloseAsync(true);
            return;
        }
        public async Task Add()
        {
            foreach (var track in Imports)
            {
                track.Title = track.FilePath;
                Dictionary<ImportTrackStatusEnum, ImportTrackErrorsEnum> trackResult;
                if (track.TrackType == Database.Models.Enums.TrackType.Rebroadcast)
                {
                    trackResult = await Task.Run(() => trackImporter.CreateNetStreamTrack(track));

                }
                else
                {
                    trackResult = await Task.Run(() => trackImporter.CreateTrackAsync(track));

                }
                track.ImportStatus = trackResult.First().Key;
                track.ImportErrors = trackResult.First().Value;

            }
            bool allSameStatus = Imports.Any() && Imports.All(t => t.ImportStatus == Imports.First().ImportStatus);
            

            await Task.Delay(2000);
            ClearImportsSafe();
            ImportFinished = true;

            if (allSameStatus)
            {
                Done();
            }

        }
        public void ClearImportsSafe()
        {
            if (Application.Current.Dispatcher.CheckAccess())
                Imports.Clear();
            else
                Application.Current.Dispatcher.Invoke(() => Imports.Clear());
        }
        public async Task AddStream()
        {
            foreach (var track in Imports)
            {
                await Task.Run(() => trackImporter.CreateNetStreamTrack(track));
            }
            ImportFinished = true;

        }
    }
}
