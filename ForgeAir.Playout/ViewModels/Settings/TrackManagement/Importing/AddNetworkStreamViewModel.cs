using Caliburn.Micro;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.Importers;
using ForgeAir.Core.Services.Importers.Interfaces;
using HandyControl.Controls;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;

namespace ForgeAir.Playout.ViewModels.Settings.TrackManagement.Importing
{
    public class AddNetworkStreamViewModel : Screen
    {
        private readonly IWindowManager _windowManager;
        private readonly IServiceProvider _provider;

        private string _url;
        public string Url
        {
            get => _url;
            set => Set(ref _url, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        public AddNetworkStreamViewModel(IServiceProvider provider, IWindowManager windowManager) { 
            _provider = provider;
            _windowManager = windowManager;
        }
        public async void Add()
        {
            var status = await Ping(Url);

            switch (status)
            {
                case IPStatus.Success:
                    await AddToDatabase(Url, Name);
                    TryCloseAsync(true); // Closes the dialog with DialogResult = true
                    break;

                case IPStatus.TimedOut:
                    System.Windows.MessageBox.Show("A timeout occurred while reaching the server", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;

                case IPStatus.BadDestination:
                    System.Windows.MessageBox.Show("Bad URL.\nPlease check the entered URL", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;

                case IPStatus.Unknown:
                default:
                    System.Windows.MessageBox.Show("An unknown error occurred.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }

        public void Cancel()
        {
            TryCloseAsync(true);
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

                var completed = await Task.WhenAny(connectTask, timeoutTask);
                if (completed == timeoutTask)
                    return IPStatus.TimedOut;

                return client.Connected ? IPStatus.Success : IPStatus.Unknown;
            }
            catch (Exception)
            {
                return IPStatus.Unknown;
            }
        }

        private async Task AddToDatabase(string url, string name)
        {
            var processVM = new ImportingProcessViewModel(_provider, new ObservableCollection<TrackImportModel>() { new TrackImportModel(url, name, Database.Models.Enums.TrackType.Rebroadcast, TimeSpan.Zero, null) });
            await _windowManager.ShowDialogAsync(processVM);
        }
    }
}
