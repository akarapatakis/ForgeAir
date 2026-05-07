using Caliburn.Micro;
using ForgeAir.Core.Services.Database.Interfaces;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Playout.Models;
using ForgeAir.Playout.ViewModels.Helpers;
using ForgeAir.Playout.Views.Settings.TrackManagement.Importing;
using MahApps.Metro.IconPacks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ForgeAir.Playout.ViewModels.Settings.Ads
{
    public class AdPackWizardViewModel
    {
        private IList<TileModel> _dataList;
        private readonly IWindowManager _windowManager;
        private readonly IServiceProvider _provider;
        private readonly ISearchService _searchService;
        private readonly Repository<AdPack> _repository;
        private readonly IDbContextFactory<ForgeAirDbContext> _dbContextFactory;
        public AdPackWizardViewModel(ISearchService searchService, IServiceProvider provider, IWindowManager windowManager, Repository<AdPack> repository, IDbContextFactory<ForgeAirDbContext> dbContextFactory)
        {
            _provider = provider;
            _windowManager = windowManager;
            _repository = repository;
            _dbContextFactory = dbContextFactory;
            _searchService = searchService;
            DataList = GetCardDataList();

        }
        internal ObservableCollection<TileModel> GetCardDataList()
        {
            return new ObservableCollection<TileModel>
            {
                new TileModel
                {
                    Title = "Create Ad Pack",
                    Icon = PackIconRemixIconKind.Wallet3Fill,
                    Color = new SolidColorBrush(Colors.Green),
                    Command = new RelayCommand(() =>
                    {
                        _windowManager.ShowDialogAsync(new EditAdPackViewModel(_dbContextFactory, _windowManager, new Core.Services.Database.RepositoryServices.TrackService(_dbContextFactory)));

                    }),
                },
                new TileModel
                {
                    Title = "Show Ad Packs",
                    Icon = PackIconRemixIconKind.ListCheck2,
                    Color = new SolidColorBrush(Colors.BlueViolet),
                    Command = new RelayCommand(() =>
                    {
                        _windowManager.ShowDialogAsync(new AdPackListViewModel(_repository, _dbContextFactory, _windowManager));

                    }),
                },
            };
        }
        public IList<TileModel> DataList { get => _dataList; set => _dataList = value; }

    }
}
