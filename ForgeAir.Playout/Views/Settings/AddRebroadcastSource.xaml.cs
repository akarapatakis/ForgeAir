using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
using ForgeAir.Core.Services;
using ForgeAir.Core.Shared;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Core.Tracks;
using ForgeAir.Database.Models;
using Microsoft.EntityFrameworkCore;
using ForgeAir.Database;
using System.Net.Http;
using System.Net.Sockets;

namespace ForgeAir.Playout.Views
{
    /// <summary>
    /// Interaction logic for AddRebroadcastSource.xaml
    /// </summary>
    public partial class AddRebroadcastSource : HandyControl.Controls.Window
    {
        ForgeAirDbContext dbContext;
        public AddRebroadcastSource()
        {
            dbContext = new ForgeAirDbContext();
            InitializeComponent();
        }

        private async void addButton_Click(object sender, RoutedEventArgs e)
        {
            string streamurl = urlBox.Text;
            string streamName = nameBox.Text;
            var task = await Task.Run(() => Ping(streamurl));
            if (task == IPStatus.Success)
            {
                await Task.Run(() => addToDB(streamurl, streamName));
            }
            else if (task == IPStatus.TimedOut)
            {
                HandyControl.Controls.MessageBox.Show("A time out occured while reaching the server", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (task == IPStatus.BadDestination)
            {
                HandyControl.Controls.MessageBox.Show("Bad URL.\nPlease check the entered url", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (task == IPStatus.TimedOut)
            {
                HandyControl.Controls.MessageBox.Show("A time out occured while reaching the server", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (task == IPStatus.Unknown)
            {
                HandyControl.Controls.MessageBox.Show("An unknown error occured.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                HandyControl.Controls.MessageBox.Show("An unknown error occured.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private async Task<IPStatus> Ping(string url)
        {
            try
            {
                var uri = new Uri(url);
                string host = uri.Host;
                int port = uri.Port;

                using var client = new TcpClient();
                var connectTask = client.ConnectAsync(host, port);
                var timeoutTask = Task.Delay(5000);

                var completedTask = await Task.WhenAny(connectTask, timeoutTask);

                if (completedTask == timeoutTask)
                    return IPStatus.TimedOut;

                return client.Connected ? IPStatus.Success : IPStatus.Unknown;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task addToDB(string url, string name)
        {
            var importer = new Importer();
            var tagReader = new Core.AudioEngine.TagReader();
            var generalHelpers = new Core.Helpers.GeneralHelpers();
            var track = new Track();

            track.FilePath = url;

            //track.Id = await tracksService.GetCountOf(ModelTypesEnum.Track) + 1;

            track.Title = name;
            track.Album = "";
            track.ISRC = "";
            track.DateAdded = DateTime.UtcNow;
            track.DateModified = DateTime.UtcNow;
            track.Bpm = 0;
            track.Duration = TimeSpan.Zero;
            track.MixPoint = track.Duration;
            track.TrackStatus = Database.Models.Enums.TrackStatus.Enabled;
            track.ReleaseDate = DateTime.UtcNow;
            track.TrackType = Database.Models.Enums.TrackType.Rebroadcast;

            track.Duration = TimeSpan.Zero;
            track.Outro = track.Duration;
            track.Intro = track.Duration;

            importer.AddTrack(track);
            await dbContext.SaveChangesAsync();

            DatabaseSharedData.Instance.RaiseDBModified();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
    
}
