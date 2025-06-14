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
using ForgeAir.Core.Services;
using ForgeAir.Core.Shared;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using MahApps.Metro.Controls;
using SharpDX.DXGI;

namespace ForgeAir.Playout.UserControls
{
    /// <summary>
    /// Interaction logic for FXSelectorControl.xaml
    /// </summary>
    public partial class FXSelectorControl : UserControl
    {

        private readonly ForgeAirDbContextFactory factory;
        public List<FX> FXTiles { get; set; } = new List<FX>();
        public Repository<FX> repository;
        public FXSelectorControl()
        {

            factory = new ForgeAirDbContextFactory();
            repository = new Repository<FX>(factory);

            InitializeComponent();
            this.DataContext = this;
            DatabaseSharedData.Instance.dbModified += RefreshFX;

            _FXSelectorControl();
        }
        public async void RefreshFX(object sender, EventArgs e)
        {
            FXTiles.Clear();
            foreach (var tile in (await Task.Run(() => repository.GetAll(Core.Tracks.Enums.ModelTypesEnum.FX))))
            {
                FXTiles.Add(tile);
            }
        }
        public async void _FXSelectorControl()
        {

            foreach (var tile in (await Task.Run(() => repository.GetAll(Core.Tracks.Enums.ModelTypesEnum.FX))))
            {
                FXTiles.Add(tile);
            }
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Tile clickedTile && clickedTile.DataContext is FX fxTile)
            {
                Task.Run(() => AudioPlayerShared.Instance.audioPlayer.PlayFX(fxTile));

            }
        }
    }
}
