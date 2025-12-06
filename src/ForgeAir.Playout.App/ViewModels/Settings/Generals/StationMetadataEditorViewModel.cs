using Caliburn.Micro;
using ForgeAir.Core.Events;
using ForgeAir.Core.Services.Managers;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Playout.App.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;


namespace ForgeAir.Playout.App.ViewModels.Settings.Generals
{
    public class StationMetadataEditorViewModel : Screen, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public int StationId { get; }
        private readonly string NameTag;
        public string NewName { get; set; }
        public string NewSlogan { get; set; }
        public string NewEmail { get; set; }
        public string NewWebsite { get; set; }
        public string NewTelephone { get; set; }
        private string _newLogoFilePath;
        public string NewLogoFilePath { get => _newLogoFilePath; set { _newLogoFilePath = value; OnPropertyChanged(nameof(NewLogoFilePath)); } }

        private Bitmap _logo;
        public Bitmap Logo { get => _logo; set { _logo = value; OnPropertyChanged(nameof(Logo)); } }
        public string NewGenre { get; set; }
        public string NewPI { get; set; }
        
        private IStorageProvider openFileDialog;
        private readonly StationManager _stationManager;
        private readonly StationInformationChangedEvent _stationInformationChanged;
        public StationMetadataEditorViewModel(IWindowManager manager, IServiceProvider serviceProvider, Station station, IDbContextFactory<ForgeAirDbContext> dbContextFactory, StationInformationChangedEvent stationInformationChanged, IStorageProvider storageProvider)
        {
            openFileDialog = storageProvider;

            StationId = station.Id;
            _stationInformationChanged = stationInformationChanged;
            NameTag = station.NameTag;
            NewName = station.Name;
            NewSlogan = station.Slogan;
            NewEmail = station.Email;
            NewWebsite = station.Website;
            NewTelephone = station.Telephone;
            NewLogoFilePath = station.LogoFilePath;
            if (NewLogoFilePath == null || NewLogoFilePath == "")
            {
                //Logo = ImageHelper.LoadBitmap(Core.Properties.Resources.ImageResources.StationDefaultImage);
            }
            else
            {
                Logo = new Bitmap(NewLogoFilePath);

            }
            NewGenre = station.Genre;
            NewPI = station.RdsPI.ToString("X4");

            _stationManager = new StationManager(dbContextFactory);
        }
        public async void Cancel()
        {
           await TryCloseAsync();
        }
        public async void FMPIGen_call()
        {
            // todo: think about FMPIGen intergration

            MessageBoxManager
                .GetMessageBoxStandard("FMPIGen", "FMPIGen is not available yet.",
                    ButtonEnum.Ok);
        }
        public async void ChangeLogo()
        {
            var files = await openFileDialog.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Audio Files")
                    {
                        Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp" }
                    },
                    FilePickerFileTypes.All
                }
            });
            
            if (files.Count == 0 || files.FirstOrDefault()==null)
            {
                return;
            }
            else
            {
                NewLogoFilePath = files.FirstOrDefault().Path.LocalPath;
                Logo = new Bitmap(NewLogoFilePath);
            }
        }
        public async void RemoveLogo()
        {
            NewLogoFilePath = null;
            //Logo = ImageHelper.LoadBitmap(Core.Properties.Resources.ImageResources.StationDefaultImage);
        }
        private void OnPropertyChanged(string propertyName) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
        public async void Apply()
        {
            ushort pi = 0xffff;
            try
            {
                pi = Convert.ToUInt16(String.Concat(NewPI.Where(c => !Char.IsWhiteSpace(c))), 16);
            }
            catch
            { 
                MessageBoxManager
                    .GetMessageBoxStandard("", "Invalid PI Code",
                        ButtonEnum.Ok);                
                if (pi == 0xffff) return;
            }
            await _stationManager.Update(
                StationId,
                NewName,
                NewSlogan,
                NewEmail,
                NewWebsite,
                NewTelephone,
                NewLogoFilePath,
                NewGenre,
                pi
            );
            _stationInformationChanged.RaiseStationUpdated(new Station() { Name = NewName, Slogan = NewSlogan, Email = NewEmail, Website = NewWebsite, Telephone = NewTelephone, LogoFilePath = NewLogoFilePath, Genre = NewGenre, RdsPI = pi, NameTag = NameTag });
            Cancel();   
        }
    }

}
