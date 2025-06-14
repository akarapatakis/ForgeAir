using ForgeAir.Core.AudioEngine;
using ForgeAir.Core.Helpers;
using ForgeAir.Core.Shared;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using System;
using System.Collections.Generic;
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

namespace ForgeAir.Playout.UserControls
{
    /// <summary>
    /// Interaction logic for TrackSelectorControl.xaml
    /// </summary>
    public partial class TrackSelectorControl : UserControl
    {
        private Core.Services.Repository<Database.Models.Track> trackDB;
        private Core.AudioEngine.QueueManager queueManager;
        private GeneralHelpers helper;
        private readonly ForgeAirDbContextFactory factory = new ForgeAirDbContextFactory();


        public TrackSelectorControl()
        {

            trackDB = new Core.Services.Repository<Track>(factory);
            InitializeComponent();

            DatabaseSharedData.Instance.dbModified += RefreshListView;

            _TrackSelectorControl();
            queueManager = new Core.AudioEngine.QueueManager();

        }

        private async void _TrackSelectorControl()
        {
            Task.Delay(1200);
            listView.ItemsSource = await trackDB.GetAll(Core.Tracks.Enums.ModelTypesEnum.Track);
        }
        public async void RefreshListView(object sender, EventArgs e)
        {
            var tracks = await trackDB.GetAll(Core.Tracks.Enums.ModelTypesEnum.Track); // Fetch data from DB
            this.Dispatcher.Invoke(() => { listView.ItemsSource = tracks; });
        }
        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listView.SelectedItem != null)
            {
                
                queueManager.AddToQueue(listView.SelectedItem as Database.Models.Track);
                AudioPlayerShared.Instance.RaiseOnQueueChanged();
            }
        }
    }
}
