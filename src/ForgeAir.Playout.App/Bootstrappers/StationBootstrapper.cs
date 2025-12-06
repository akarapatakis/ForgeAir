using Caliburn.Micro;
using ForgeAir.Core.CustomCollections;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Events;
using ForgeAir.Core.Helpers;
using ForgeAir.Core.Helpers.Interfaces;
using ForgeAir.Core.Jobs;
using ForgeAir.Core.MetadataExports;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.AudioPlayout;
using ForgeAir.Core.Services.AudioPlayout.DSP.VST;
using ForgeAir.Core.Services.AudioPlayout.DSP.VST.Interfaces;
using ForgeAir.Core.Services.AudioPlayout.Interfaces;
using ForgeAir.Core.Services.AudioPlayout.Players;
using ForgeAir.Core.Services.AudioPlayout.Players.Interfaces;
using ForgeAir.Core.Services.Database;
using ForgeAir.Core.Services.Database.Interfaces;
using ForgeAir.Core.Services.Database.RepositoryServices;
using ForgeAir.Core.Services.Importers;
using ForgeAir.Core.Services.Importers.Interfaces;
using ForgeAir.Core.Services.Scheduler;
using ForgeAir.Core.Services.Scheduler.Interfaces;
using ForgeAir.Core.Services.StreamingClient;
using ForgeAir.Core.Services.StreamingClient.Interfaces;
using ForgeAir.Core.Services.TrackSelector;
using ForgeAir.Core.Services.TrackSelector.Interfaces;
using ForgeAir.Core.Services.Weather;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Playout.App.Helpers;
using ForgeAir.Playout.App.Services;
using ForgeAir.Playout.App.UserControls;
using ForgeAir.Playout.App.UserControls.ViewModels;
using ForgeAir.Playout.App.ViewModels;
using ForgeAir.Playout.App.ViewModels.PlayoutWindows;
using ForgeAir.Playout.App.ViewModels.Settings;
using ForgeAir.Playout.App.ViewModels.Settings.Generals;
using ForgeAir.Playout.App.ViewModels.Settings.TrackManagement.Importing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using MySqlConnector;
using Quartz;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ForgeAir.Playout.UserControls.ViewModels;
using ForgeAir.Playout.ViewModels;
using ForgeAir.Playout.ViewModels.PlayoutWindows;
using ForgeAir.Playout.ViewModels.Settings;
using ForgeAir.Playout.ViewModels.Settings.TrackManagement.Importing;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;


namespace ForgeAir.Playout.App.Bootstrappers
{

    public class StationBootstrapper
    {
        // for station selector viewmodel - dont wanna deal with making new models as the station selector pulls data from here anyways
        public string Tag { get; private set; }
        public IImage LogoPath { get; private set; }
        public string DisplayName { get; private set; }

        public IServiceProvider Services { get; private set; }
        private readonly IServiceProvider _globalProvider;
        private readonly ISchedulerFactory _schedulerFactory;
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
                    options.UseLazyLoadingProxies();

                    options.UseMySql(
                        $"Server={dbHost};Port={dbPort};Database={dbName};User={dbUser};Password={config.Get("Database", "Password")};",
                        new MySqlServerVersion(new Version(9, 1, 0)));
                });
                services.AddDbContextFactory<ForgeAirDbContext>(options =>
                {
                    options.UseLazyLoadingProxies();

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
                        options.UseLazyLoadingProxies();

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
                    MessageBoxManager
                        .GetMessageBoxStandard("Database Error","Not able to access/create database!. \nPlease check your database configuration",
                            ButtonEnum.Ok, Icon.Warning).ShowAsync();
                }
            }
            services.AddSingleton<ISearchService, SearchService>();
            services.AddSingleton<ITracksService, TrackService>();
            services.AddSingleton<StationMetadataEditorViewModel>();
            services.AddSingleton<IWindowManager, WindowManager>();
            services.AddSingleton<ISchedulerService, SchedulerService>();
            services.AddSingleton<Repository<ArtistDTO>>();
            services.AddSingleton<Repository<CategoryDTO>>();
            services.AddSingleton<ObservableCollection<TrackDTO>>();
            services.AddSingleton<ObservableCollection<LinkedListQueueItem>>();
            services.AddSingleton<LinkedListQueue<LinkedListQueueItem>>();
            services.AddSingleton<LinkedListQueue<TrackDTO>>();
            services.AddTransient<ShellViewModel>();
            services.AddTransient<ActivityCenterViewModel>();
            services.AddSingleton<IWeatherService, WeatherService>();
            services.AddTransient<CategoryManipulatorViewModel>();
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<ITracksService, TrackService>();
            services.AddSingleton<ObservableCollection<CategoryDTO>>();
            services.AddSingleton<ObservableCollection<Category>>();
            services.AddSingleton<ObservableCollection<ArtistTrackDTO>>();
            services.AddSingleton<ObservableCollection<ArtistTrack>>();
            services.AddSingleton<Caliburn.Micro.IEventAggregator, Caliburn.Micro.EventAggregator>();
            services.AddSingleton<QueueUpdatedEvent>();
            services.AddSingleton<TrackChangedEvent>();
            services.AddSingleton<StationInformationChangedEvent>();
            services.AddTransient<TrackLibraryViewModel>();
            //services.AddSingleton(globalProvider.GetRequiredService<ILogger>());
            services.AddLogging(logging =>
            {
                logging.AddConsole(); 
                logging.SetMinimumLevel(LogLevel.Debug);
            });
           //todo: migrate shellview to avalonia
            //services.AddTransient<ShellView>();
            services.AddTransient<AboutViewModel>();
            services.AddSingleton<TrackQueueViewModel>();
            services.AddSingleton<OnAirViewModel>();
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
            services.AddTransient<Core.Helpers.Interfaces.IEventAggregator, SimpleEventAggregator>();
            services.AddTransient<IVSTService, BassVSTService>();
            services.AddSingleton<ITrackImporter, TrackImporter>();
            services.AddTransient<ClockTrackSelector>();
            services.AddTransient<RandomTrackSelector>();
            services.AddTransient<ManualTrackSelector>();
            services.AddSingleton<TrackSelectorFactory>();
            services.AddSingleton<TrackSelectorService>();
            services.AddTransient<IQueueService, QueueService>();
           // services.AddSingleton<TrackChangedEvent>();
            services.AddTransient<VUMeter>();
            services.AddTransient<TimeAnnouncementJob>();

            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory(); // important!
                q.ScheduleJob<TimeAnnouncementJob>(
                    trigger => trigger
                        .WithIdentity("TimeAnnouncementTrigger", "Jingles")
                        .WithCronSchedule("0 0 * * * ?"), //  top-of-hour
                    job => job
                        .WithIdentity("TimeAnnouncementJob", "Jingles")
                );
            });
            services.AddQuartzHostedService(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });

            //todo: migrate these controls
            //services.AddTransient<StereoVUMeterControl>();
            //services.AddTransient<WaveformCanvasControl>();

            services.AddTransient<StreamingEncoder>(s =>
            {
                var config = s.GetRequiredService<IConfigurationManager>();

                return new StreamingEncoder
                {
                    Autoconnect = config.GetBool("Streaming", "Autoconnect", true),
                    Bitrate = config.GetInt("Streaming", "Bitrate", 128),
                    DisplayName = config.Get("Streaming", "DisplayName", "ForgeAir Stream"),
                    Format = Enum.TryParse(config.Get("Streaming", "Format"), true, out Core.Models.Enums.StreamingEncoderFormat format)
                        ? format
                        : Core.Models.Enums.StreamingEncoderFormat.MP3,
                    Genre = config.Get("Streaming", "Genre"),
                    Mountpoint = config.Get("Streaming", "Mountpoint"),
                    Password = config.Get("Streaming", "Password"),
                    Protocol = Enum.TryParse(config.Get("Streaming", "Protocol"), true, out Core.Models.Enums.StreamingEncoderProtocol protocol)
                        ? protocol
                        : Core.Models.Enums.StreamingEncoderProtocol.Icecast,
                    PullDataFromStationInfo = config.GetBool("Streaming", "UseStationMetadata", false),
                    ServerPort = config.Get("Streaming", "Port"),
                    ServerURL = config.Get("Streaming", "Server"),
                };
            });


            services.AddTransient<StreamingClientService>();
            services.AddSingleton<IAudioService, AudioPlayerService>();
            services.AddSingleton<IPlayerFactory, PlayerFactory>();
            services.AddSingleton<IPlayer>(sp =>
            {
                var config = sp.GetRequiredService<IConfigurationManager>();
                if (config.Get("MainOutput", "AudioEngine") == "Bass" && !SystemHelper.BassExists())
                {
                    MessageBoxManager
                        .GetMessageBoxStandard("BASS Initialization failed","BASS libraries not found.\nInstall them or switch the audio engine to NAudio",
                            ButtonEnum.Ok, Icon.Warning).ShowAsync();
                    Environment.Exit(1);
                }
                var factory = sp.GetRequiredService<IPlayerFactory>();
                return factory.CreatePlayer(Core.AudioEngine.Enums.DeviceTypeEnum.Main); // todo: make this configurable/loop through all device types
            });

            services.AddSingleton<IConfigurationManager>(provider =>
            {
                var configFile = iniPath;
                return new ConfigurationManager(configFile);
            });

            services.AddSingleton<StationPaths>(provider =>
            {
                return new StationPaths() { ExportsPath = new FileInfo(iniPath).Directory.FullName + "\\Metadata" };
            });
            services.AddSingleton<NowPlayingModel>();
            services.AddSingleton<MetadataExporter>();

            //todo: remove after test
            services.AddTransient<TestViewModel>();

            Services = services.BuildServiceProvider();
        }

        private void InitializeDatabase()
        {
            var context = Services.GetRequiredService<ForgeAirDbContext>();
            try
            {
                context.Database.Migrate();

                var station = context.Stations.Where(x => x.NameTag == Tag).FirstOrDefault(); // retrieve station info based on tag
                if (station != null)
                {
                    DisplayName = station.Name;
                    if (station.LogoFilePath == null)
                    {
                        LogoPath = ImageHelper.ByteArrayToImage(Core.Properties.Resources.ImageResources.StationDefaultImage);
                    }
                    else
                    {
                        LogoPath = ImageHelper.LoadBitmap(station.LogoFilePath);
                    }
                }
                else if (station == null)
                {
                     MessageBoxManager
                        .GetMessageBoxStandard("Database Error","Station '{Tag}' was not found in the database.\nPlease check ForgeAir's configuration and try again.",
                            ButtonEnum.Ok, Icon.Warning).ShowAsync();
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Access denied", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBoxManager
                        .GetMessageBoxStandard("Database Error","The database server refused the connection. \nPlease check ForgeAir or database server's configuration and try again.",
                            ButtonEnum.Ok, Icon.Warning).ShowAsync();
                    Environment.Exit(1);
                }
            }

            if (!context.Database.EnsureCreated())
            {
                if (context.Stations.Any()) return;
                context.Stations.Add(new Station() { Name = "My Radio Station!", Slogan = "Powered by ForgeAir", Website = "www.example.com", Genre = "None Assigned", Email = "example@example.com", Id = 0, NameTag = Tag });
                context.SaveChanges();
            }

            context.ChangeTracker.Clear();
            context = null;
            return;

        }

        private void UpdateStationInfo(Station station)
        {
            DisplayName = station.Name;
            if (station.LogoFilePath == null)
            {
                LogoPath = ImageHelper.ByteArrayToImage(Core.Properties.Resources.ImageResources.StationDefaultImage);
            }
            else
            {
                LogoPath = ImageHelper.LoadBitmap(station.LogoFilePath);
            }
        }
        public async Task Initialize()
        {
            Services.GetRequiredService<StationInformationChangedEvent>().StationUpdated += UpdateStationInfo;

            var dbFactory = Services.GetRequiredService<IDbContextFactory<ForgeAirDbContext>>();
            dbFactory.CreateDbContext();
            InitializeDatabase();

            var weather = Services.GetRequiredService<IWeatherService>();
            var metadata = Services.GetRequiredService<MetadataExporter>();
            if (_globalProvider != null)
            {
                weather.CurrentWeather = await Task.Run(() => weather.GetWeather(_globalProvider.GetRequiredService<IConfigurationManager>().Get("Weather", "City") + " " + _globalProvider.GetRequiredService<IConfigurationManager>().Get("Weather", "Country")));
            }
            var schedulerFactory = Services.GetRequiredService<ISchedulerFactory>();
            var scheduler = await schedulerFactory.GetScheduler();
            await scheduler.Start();
            return;
        }

        public async Task ShowShellViewAsync()
        {
            var windowManager = Services.GetRequiredService<IWindowManager>();
            var shellViewModel = Services.GetRequiredService<ShellViewModel>();
            var window = WindowHelper.GetShellView();

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
