using Caliburn.Micro;
using ForgeAir.Core.Events;
using ForgeAir.Core.Services.Managers;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Playout.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ForgeAir.Playout.ViewModels.Settings.Generals
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

        private BitmapImage _logo;
        public BitmapImage Logo { get => _logo; set { _logo = value; OnPropertyChanged(nameof(Logo)); } }
        public string NewGenre { get; set; }
        public string NewPI { get; set; }

        private readonly StationManager _stationManager;
        private readonly StationInformationChangedEvent _stationInformationChanged;
        public StationMetadataEditorViewModel(IWindowManager manager, IServiceProvider serviceProvider, Station station, IDbContextFactory<ForgeAirDbContext> dbContextFactory, StationInformationChangedEvent stationInformationChanged)
        {
            StationId = station.Id;
            _stationInformationChanged = stationInformationChanged;
            NameTag = station.NameTag;
            NewName = station.Name;
            NewSlogan = station.Slogan;
            NewEmail = station.Email;
            NewWebsite = station.Website;
            NewTelephone = station.Telephone;
            NewLogoFilePath = station.LogoFilePath;
            try
            {
                if (NewLogoFilePath == null || NewLogoFilePath == "")
                {
                    Logo = ImageHelper.BitmapToBitmapImage(Core.Properties.Resources.ImageResources.StationDefaultImage);
                }
                else
                {
                    Logo = new BitmapImage(new Uri(NewLogoFilePath));

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't locate or load logo image.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                NewLogoFilePath = "";
                Logo = ImageHelper.BitmapToBitmapImage(Core.Properties.Resources.ImageResources.StationDefaultImage);
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
            MessageBox.Show("FMPIGen is not available yet.");
        }
        public async void ChangeLogo()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckFileExists = true;
            openFileDialog.Title = "Select New Station Logo";
            openFileDialog.DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            openFileDialog.ValidateNames = true;
            if (openFileDialog.ShowDialog() == true)
            {
                NewLogoFilePath = openFileDialog.FileName;
                Logo = new BitmapImage(new Uri(NewLogoFilePath));
            }
        }
        public async void RemoveLogo()
        {
            NewLogoFilePath = null;
            Logo = ImageHelper.BitmapToBitmapImage(Core.Properties.Resources.ImageResources.StationDefaultImage);
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
                MessageBox.Show("Invalid PI code.");
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
