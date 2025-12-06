using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Caliburn.Micro;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Services.Tags;
using ForgeAir.Core.Tracks;
using ForgeAir.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace ForgeAir.Playout.App.ViewModels.Settings.TrackManagement.Importing
{
    public class ImportFxViewModel : Screen
    {
        private string fxName;
        private string fxColor;
        private string fxFilename;
        private FxDTO fx;
        public string FileBox
        {
            get => _fileBox;
            set
            {
                _fileBox = value;
                NotifyOfPropertyChange(() => FileBox);
            } 
        }
        private string _fileBox;        private IStorageProvider openFileDialog;
        private readonly IWindowManager _windowManager;
        private readonly Importer importer;
        public ImportFxViewModel(IServiceProvider provider, IWindowManager windowManager, IStorageProvider storageProvider)
        {
            openFileDialog = storageProvider;
            importer = new Importer(provider.GetRequiredService<IDbContextFactory<ForgeAirDbContext>>());

        }

        public void Cancel()
        {
            TryCloseAsync();
        }
        public async void FileSelect()
        {
            var files = await openFileDialog.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Audio Files")
                    {
                        Patterns = new[] { "*.mp3", "*.wav", "*.flac", "*.ogg", "*.m4a", "*.fla", "*.wma", "*.opus" }
                    },
                    FilePickerFileTypes.All
                }
            });
            
            if (files.Count == 0 || files.FirstOrDefault()==null)
            {
                return;
            }
            else { FileBox = files.FirstOrDefault().Path.LocalPath; NotifyOfPropertyChange(() => FileBox); }
        }
        public void Add()
        {

            if (fxFilename != null && File.Exists(fxFilename))
            {
                var tagReader = new TagService(new TrackDTO() { FilePath = fxFilename });
                fx = new FxDTO
                {
                    FilePath = fxFilename,
                    Duration = tagReader.AudioDuration,
                    DateAdded = DateTime.Now,
                    DateModified = DateTime.Now,
                    Status = Database.Models.Enums.TrackStatus.Enabled,
                    Color = fxColor,
                    Name = fxName,
                };
                if (fx == null) return;
                importer.AddFx(fx);

                return;
            }

        }

    }
}
