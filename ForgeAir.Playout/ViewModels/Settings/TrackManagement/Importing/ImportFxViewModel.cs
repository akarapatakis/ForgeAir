using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Services.Tags;
using ForgeAir.Core.Tracks;
using ForgeAir.Database;
using Microsoft.Identity.Client;
using Microsoft.Win32;

namespace ForgeAir.Playout.ViewModels.Settings.TrackManagement.Importing
{
    public class ImportFxViewModel : Screen
    {
        private string fxName;
        private string fxColor;
        private string fxFilename;
        private FxDTO fx;
        private string fileDirBox;
        private OpenFileDialog openFileDialog;
        private ForgeAirDbContext _context;
        private readonly IWindowManager _windowManager;
        private readonly Importer importer = new();
        public ImportFxViewModel(IWindowManager windowManager)
        {
            openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Multiselect = false;
            openFileDialog.CheckPathExists = true;
        }

        public void Cancel()
        {
            TryCloseAsync();
        }
        public void FileSelect()
        {
            if (openFileDialog.ShowDialog() == false)
            {
                return;
            }
            else { fxFilename = openFileDialog.FileName; fileDirBox = openFileDialog.FileName; }
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
