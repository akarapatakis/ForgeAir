using Caliburn.Micro;
using ForgeAir.Core.CustomCollections;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Helpers;
using ForgeAir.Core.Helpers.Interfaces;
using ForgeAir.Core.Services.AudioPlayout;
using ForgeAir.Core.Services.AudioPlayout.DSP.VST;
using ForgeAir.Core.Services.AudioPlayout.DSP.VST.Interfaces;
using ForgeAir.Core.Services.AudioPlayout.Interfaces;
using ForgeAir.Core.Services.AudioPlayout.Players;
using ForgeAir.Core.Services.AudioPlayout.Players.Interfaces;
using ForgeAir.Core.Services.Database;
using ForgeAir.Core.Services.Importers;
using ForgeAir.Core.Services.Importers.Interfaces;
using ForgeAir.Core.Services.Scheduler;
using ForgeAir.Core.Services.Scheduler.Interfaces;
using ForgeAir.Core.Services.TrackSelector;
using ForgeAir.Core.Services.TrackSelector.Interfaces;
using ForgeAir.Core.Services.Weather;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Playout.Services;
using ForgeAir.Playout.UserControls.ViewModels;
using ForgeAir.Playout.ViewModels;
using ForgeAir.Playout.ViewModels.PlayoutWindows;
using ForgeAir.Playout.ViewModels.Settings;
using ForgeAir.Playout.ViewModels.Settings.TrackManagement.Importing;
using ForgeAir.Playout.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ML;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ForgeAir.Playout.Bootstrappers
{

    public class StationBootstrapper
    {
        public string Tag { get; private set; }
        public IServiceProvider Services { get; private set; }
        private readonly IServiceProvider _globalProvider;


        public StationBootstrapper(string iniPath, IServiceProvider globalProvider)
        {
            _globalProvider = globalProvider;

            var config = new ConfigurationManager(iniPath);
            Tag = config.Get("General", "Tag");
            var dbName = config.Get("Database", "DatabaseName");
            var dbUser = config.Get("Database", "User");
            var dbHost = config.Get("Database", "Host");
            var dbPort = config.Get("Database", "Port");


            var services = new ServiceCollection();

            services.AddSingleton<IConfigurationManager>(_ => config);
            try
            {
                services.AddDbContext<ForgeAirDbContext>(options =>
                {
                    options.UseMySql(
                        $"Server={dbHost};Port={dbPort};Database={dbName};User={dbUser};Password={config.Get("Database", "Password")};",
                        new MySqlServerVersion(new Version(9, 1, 0)));
                });
                services.AddDbContextFactory<ForgeAirDbContext>(options =>
                {
                    options.UseMySql(
                        $"Server={dbHost};Port={dbPort};Database={dbName};User={dbUser};Password={config.Get("Database", "Password")};",
                        new MySqlServerVersion(new Version(9, 1, 0)));
                });
            }
            catch (DbException ex)
            {
                if (CreateDatabase(dbHost, dbPort, dbName, dbUser, config.Get("Database", "Password")))
                {
                    services.AddDbContext<ForgeAirDbContext>(options =>
                    {
                        options.UseMySql(
                            $"Server={dbHost};Port={dbPort};Database={dbName};User={dbUser};Password={config.Get("Database", "Password")};",
                            new MySqlServerVersion(new Version(9, 1, 0)));
                    });
                    services.AddDbContextFactory<ForgeAirDbContext>(options =>
                    {
                        options.UseMySql(
                            $"Server={dbHost};Port={dbPort};Database={dbName};User={dbUser};Password={config.Get("Database", "Password")};",
                            new MySqlServerVersion(new Version(9, 1, 0)));
                    });
                }
                else
                {
                    MessageBox.Show("Not able to access/create database!. \nPlease check your database configuration", "Error", MessageBoxButton.OK, icon: MessageBoxImage.Error);
                }
            }
            services.AddSingleton<IWindowManager, WindowManager>();
            services.AddSingleton<ISchedulerService, SchedulerService>();
            services.AddTransient<IAudioService, AudioPlayerService>();
            services.AddSingleton<RepositoryService<ArtistDTO>>();
            services.AddSingleton<RepositoryService<CategoryDTO>>();
            services.AddSingleton<ObservableCollection<TrackDTO>>();
            services.AddSingleton<ObservableCollection<LinkedListQueueItem>>();
            services.AddSingleton<LinkedListQueue<LinkedListQueueItem>>();
            services.AddSingleton<LinkedListQueue<TrackDTO>>();
            services.AddTransient<ShellViewModel>();
            services.AddTransient<ActivityCenterViewModel>();
            services.AddSingleton<IWeatherService, WeatherService>();
            services.AddTransient<CategoryManipulatorViewModel>();
            services.AddSingleton<RepositoryService<Track>>();
            services.AddSingleton<RepositoryService<TrackDTO>>();
            services.AddSingleton<RepositoryService<Artist>>();
            services.AddSingleton<RepositoryService<Category>>();
            services.AddSingleton<RepositoryService<ArtistTrack>>();
            services.AddSingleton<ObservableCollection<CategoryDTO>>();
            services.AddSingleton<ObservableCollection<Category>>();
            services.AddSingleton<ObservableCollection<ArtistTrackDTO>>();
            services.AddSingleton<ObservableCollection<ArtistTrack>>();
            services.AddTransient<ShellView>();
            services.AddTransient<AboutViewModel>();
            services.AddSingleton<TrackQueueViewModel>();
            services.AddSingleton<Core.Helpers.Interfaces.IEventAggregator, SimpleEventAggregator>();
            services.AddSingleton<IWindowManager, WindowManager>();
            services.AddTransient<ImportingProcessViewModel>();
            services.AddTransient<ImportDirectoryViewModel>();
            services.AddSingleton<CrashReporterService>();
            services.AddTransient<TrackSelectorViewModel>();
            services.AddTransient<PlayoutViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<ActivityCenterViewModel>();
            services.AddSingleton<IWeatherService, WeatherService>();
            services.AddTransient<CategoryManipulatorViewModel>();
            services.AddSingleton<RepositoryService<FX>>();
            services.AddSingleton<Core.Helpers.Interfaces.IEventAggregator, SimpleEventAggregator>();
            services.AddSingleton<IVSTService, BassVSTService>();
            services.AddSingleton<ITrackImporter, TrackImporter>();
            services.AddSingleton<ITrackSelector, RandomTrackSelector>(); //todo: make this configurable
            services.AddSingleton<IQueueService, QueueService>();
            services.AddSingleton<TrackStateUpdater>();
            services.AddSingleton<QueueStateUpdater>();
            services.AddSingleton<IPlayerFactory, PlayerFactory>();
            services.AddSingleton<IPlayer>(sp =>
            {
                var factory = sp.GetRequiredService<IPlayerFactory>();
                return factory.CreatePlayer();
            });

            services.AddSingleton<IConfigurationManager>(provider =>
            {
                var configFile = iniPath;
                return new ConfigurationManager(configFile);
            });


            Services = services.BuildServiceProvider();
        }

        private void InitializeDatabase()
        {
            var context = Services.GetRequiredService<ForgeAirDbContext>();
            context.Database.Migrate();

            if (!context.Database.EnsureCreated())
            {
                if (context.Stations.Any()) return;
                context.Stations.Add(new Station() { Name = "My Radio Station!", Slogan = "Powered by ForgeAir", Website = "www.example.com", Genre = "None Assigned", Email = "example@example.com", Id = 0, NameTag = "default_station" });
                context.SaveChanges();
                context.ChangeTracker.Clear();
            }
            context.ChangeTracker.Clear();
            context = null;
            return;

        }
        public async Task Initialize()
        {
            var dbFactory = Services.GetRequiredService<IDbContextFactory<ForgeAirDbContext>>();
            dbFactory.CreateDbContext();
            InitializeDatabase();
            Services.GetRequiredService<IPlayerFactory>().CreatePlayer();
            var weather = Services.GetRequiredService<IWeatherService>();
            if (_globalProvider != null)
            {
                weather.CurrentWeather = await Task.Run(() => weather.GetWeather(_globalProvider.GetRequiredService<IConfigurationManager>().Get("Weather", "City") + " " + _globalProvider.GetRequiredService<IConfigurationManager>().Get("Weather", "Country")));
            }
            return;
        }

        public async Task ShowShellViewAsync()
        {
            var windowManager = Services.GetRequiredService<IWindowManager>();
            var shellViewModel = Services.GetRequiredService<ShellViewModel>();
            var window = System.Windows.Application.Current.Windows
            .OfType<System.Windows.Window>()
            .FirstOrDefault(w => w.DataContext is ShellViewModel);

             if (window == null)
             {
                await windowManager.ShowWindowAsync(shellViewModel);
                return;
             }

            else if (!window.IsVisible)
            {
               window.Show();
               window.Activate();

            }
        }

        private bool CreateDatabase(string server, string port, string dbName, string user, string password)
        {
            var options = $"Server={server};Port={port};Database=mysql;User={user};Password={password};";
            try
            {
                using (var connection = new MySqlConnection(options))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = $"CREATE DATABASE IF NOT EXISTS `{dbName}`;";
                        command.ExecuteNonQuery();
                        return true;
                    }

                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
