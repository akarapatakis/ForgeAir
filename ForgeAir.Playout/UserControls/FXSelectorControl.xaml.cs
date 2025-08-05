using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Services;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using MahApps.Metro.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ForgeAir.Playout.UserControls
{
    /// <summary>
    /// Interaction logic for FXSelectorControl.xaml
    /// </summary>
    public partial class FXSelectorControl : UserControl
    {

        public List<FxDTO> FXTiles { get; set; } = new List<FxDTO>();
        public Core.Services.Database.RepositoryService<FX> repository;
        public FXSelectorControl(IServiceProvider provider)
        {

            repository = new Core.Services.Database.RepositoryService<FX>(provider.GetRequiredService<IDbContextFactory<ForgeAirDbContext>>());

            InitializeComponent();
            this.DataContext = this;

            _FXSelectorControl();
        }
        public async void RefreshFX(object sender, EventArgs e)
        {
            FXTiles.Clear();
            foreach (var tile in (await Task.Run(() => repository.GetAll(Core.Tracks.Enums.ModelTypesEnum.FX))))
            {
                FXTiles.Add(FxDTO.FromEntity(tile));
            }
        }
        public async void _FXSelectorControl()
        {

            foreach (var tile in (await Task.Run(() => repository.GetAll(Core.Tracks.Enums.ModelTypesEnum.FX))))
            {
                FXTiles.Add(FxDTO.FromEntity(tile));
            }
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Tile clickedTile && clickedTile.DataContext is FX fxTile)
            {
                // Task.Run(() => AudioPlayerShared.Instance.audioPlayer.PlayFX(fxTile));

            }
        }
    }
}
