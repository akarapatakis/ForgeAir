using ForgeAir.Core.Services.Managers;
using ForgeAir.Core.Shared;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Playout.Views.Settings;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ForgeAir.Playout.Views
{
    /// <summary>
    /// Interaction logic for TracksList.xaml
    /// </summary>
    public partial class TracksList : HandyControl.Controls.Window
    {
        private Core.Services.Database.RepositoryService<Database.Models.Track> trackDB;
        private readonly ForgeAirDbContextFactory factory = new ForgeAirDbContextFactory();

        private TrackManager trackManager;
        private int trackCount;
        ContextMenu menu;
        public TracksList()
        {
            InitializeComponent();


            trackDB = new Core.Services.Database.RepositoryService<Database.Models.Track>(factory);

            menu = new ContextMenu();
            MenuItem deleteItem = new MenuItem();
            deleteItem.Header = "Delete Track";
            deleteItem.Click += DeleteTrack;

            DatabaseSharedData.Instance.dbModified += refreshList;
            menu.Items.Add(deleteItem);


            _TracksList();

        }

        private void refreshList(object? sender, EventArgs e)
        {
            _TracksList();
        }

        private async void DeleteTrack(object? sender, EventArgs e)
        {

            if (listView.SelectedItem == null)
            {
                return;
            }

            Track track = listView.SelectedItem as Track;

            if (HandyControl.Controls.MessageBox.Show("Do you really want to delete this track?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                trackManager = new TrackManager(track);
                await Task.Run(() => trackManager.Delete());
                DatabaseSharedData.Instance.RaiseDBModified();
            }


        }

        private async void _TracksList()
        {
            Task.Delay(1200);
            listView.ItemsSource = await trackDB.GetAll(Core.Tracks.Enums.ModelTypesEnum.Track);

        }
        private void GenerateColumns(object item)
        {
            // Clear existing columns
            if (listView.View is GridView gridView)
            {
                gridView.Columns.Clear();
            }
            else
            {
                gridView = new GridView();
                listView.View = gridView;
            }

            // Use reflection to get properties dynamically
            foreach (PropertyInfo prop in item.GetType().GetProperties())
            {
                var column = new GridViewColumn
                {
                    Header = prop.Name, // Use property name as column header
                    DisplayMemberBinding = new System.Windows.Data.Binding(prop.Name),
                    Width = 150
                };
                gridView.Columns.Add(column);
            }
        }

        private void LoadTracks()
        {

        }

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TrackEditor trackEditor = new TrackEditor((Track)listView.SelectedItem);
            trackEditor.ShowDialog();
        }

        private void listView_MouseRightButtonUp(object sender, RoutedEventArgs e)
        {
            menu.IsOpen = true;
            menu.Show();
            
        }
    }
}
