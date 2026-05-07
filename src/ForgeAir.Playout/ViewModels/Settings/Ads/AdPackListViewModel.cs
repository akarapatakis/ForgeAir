using Caliburn.Micro;
using ForgeAir.Core.Services.Database.RepositoryServices;
using ForgeAir.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ForgeAir.Playout.ViewModels.Settings.Ads
{
    public class AdPackListViewModel : Screen
    {
        private readonly Repository<AdPack> _repository;
        private readonly IWindowManager _windowManager;
        private readonly Microsoft.EntityFrameworkCore.IDbContextFactory<ForgeAir.Database.ForgeAirDbContext> db;
        private readonly TrackService trackService;

        public AdPackListViewModel(
            Repository<AdPack> repository,
            IDbContextFactory<ForgeAir.Database.ForgeAirDbContext> dbContextFactory,
            IWindowManager windowManager)
        {
            _repository = repository;
            _windowManager = windowManager;
            db = dbContextFactory;
            trackService = new TrackService(dbContextFactory);
            AdPacks = new ObservableCollection<AdPack>();
        }

        protected override async Task OnActivateAsync(
            System.Threading.CancellationToken cancellationToken)
        {
            await LoadPacks();
        }

        #region Properties

        private ObservableCollection<AdPack> _adPacks;
        public ObservableCollection<AdPack> AdPacks
        {
            get => _adPacks;
            set => Set(ref _adPacks, value);
        }

        private AdPack _selectedAdPack;
        public AdPack SelectedAdPack
        {
            get => _selectedAdPack;
            set => Set(ref _selectedAdPack, value);
        }

        #endregion

        #region Methods

        public async Task LoadPacks()
        {
            try
            {
                var packs = await _repository.GetAllAsync(q =>
                    q.Include(p => p.Ads)
                     .ThenInclude(a => a.Track));

                AdPacks = new ObservableCollection<AdPack>(packs);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task CreatePack()
        {
            await _windowManager.ShowDialogAsync(new EditAdPackViewModel(db, _windowManager, trackService, SelectedAdPack));
            await LoadPacks();
        }

        public async Task EditPack()
        {
            if (SelectedAdPack == null)
                return;

            await _windowManager.ShowDialogAsync(new EditAdPackViewModel(db, _windowManager, trackService, SelectedAdPack));


            await LoadPacks();
        }

        public async Task DeletePack()
        {
            if (SelectedAdPack == null)
                return;

            await _repository.DeleteAsync(SelectedAdPack);
            await LoadPacks();
        }

        #endregion
    }
}
