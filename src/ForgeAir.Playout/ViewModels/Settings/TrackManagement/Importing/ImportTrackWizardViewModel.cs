using Caliburn.Micro;
using ForgeAir.Core.Services.Database.Interfaces;
using ForgeAir.Core.Services.Database.RepositoryServices;
using ForgeAir.Playout.Models;
using ForgeAir.Playout.ViewModels.Helpers;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ForgeAir.Playout.ViewModels.Settings.TrackManagement.Importing
{
    public class ImportTrackWizardViewModel
    {
        private IList<TileModel> _dataList;
        private readonly IWindowManager _windowManager;
        private readonly IServiceProvider _provider;
        private readonly ISearchService _searchService;
        public ImportTrackWizardViewModel(ISearchService searchService, IServiceProvider provider, IWindowManager windowManager)
        {
            _provider = provider;
            _windowManager = windowManager;
            _searchService = searchService;
            DataList = GetCardDataList();

        }
        internal ObservableCollection<TileModel> GetCardDataList()
        {
            return new ObservableCollection<TileModel>
            {
                new TileModel
                {
                    Title = "Import file",
                    Icon = PackIconRemixIconKind.FileMusicFill,
                    Color = new SolidColorBrush(Colors.Orange),
                    Command = new RelayCommand(() =>
                    {
                        _windowManager.ShowDialogAsync(new ImportTrackFileViewModel(_provider, _windowManager));

                    }),
                },
                new TileModel
                {
                    Title = "Import from Folder",
                    Icon = PackIconRemixIconKind.FolderMusicFill,
                    Color = new SolidColorBrush(Colors.SandyBrown),
                    Command = new RelayCommand(() =>
                    {
                        _windowManager.ShowDialogAsync(new ImportDirectoryViewModel(_searchService, _provider, _windowManager));

                    }),
                },
                new TileModel
                {
                    Title = "Import from List",
                    Icon = PackIconRemixIconKind.PlayList2Fill,
                   Color = new SolidColorBrush(Colors.BlueViolet),
                   Command = new RelayCommand(() =>
                   {
                       _windowManager.ShowDialogAsync(new ImportM3UListViewModel(_provider, _windowManager));
                   }),
                },
            };
        }
        public IList<TileModel> DataList { get => _dataList; set => _dataList = value; }

    }
}
