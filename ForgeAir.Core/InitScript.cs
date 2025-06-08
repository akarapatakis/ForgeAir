using ForgeAir.Core.AudioEngine;
using ForgeAir.Core.AudioEngine.Enums;
using ForgeAir.Core.Exceptions;
using ForgeAir.Core.Helpers;
using ForgeAir.Core.Shared;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ForgeAir.Core
{
    public class InitScript
    {
        private Helpers.ConfigurationManager configuration;
        private GeneralHelpers generalHelper;


        public InitScript()
        {
            configuration = new Helpers.ConfigurationManager("configuration.ini"); // Load config file
            generalHelper = new GeneralHelpers();

            AudioPlayerShared.Instance.deviceManager = new Core.AudioEngine.DeviceManager();
            VSTEffect.Instance.effectManager = new VSTEffectManager();
        }

        public void ExecuteForgeVision()
        {
            if (!File.Exists("configuration.ini"))
            {
                System.Windows.MessageBox.Show("ForgeVision Configuration file missing.");
                Environment.Exit(-1);
                return;
            }

            // DATABASE INITIALIZATION

            Database.Shared.DatabaseConnectionProperties.Instance.dbName = configuration.Get("Database", "DatabaseName");
            Database.Shared.DatabaseConnectionProperties.Instance.password = configuration.Get("Database", "Password");
            Database.Shared.DatabaseConnectionProperties.Instance.serverPort = configuration.GetInt("Database", "Port").ToString();

            using (ForgeAir.Database.ForgeAirDbContext context = new Database.ForgeAirDbContext())
            {
                context.Database.Migrate(); //create or update db

            };

            // VIDEO INITIALIZATION

            VideoOutputShared.Instance.videoResWidth = Double.Parse(configuration.GetInt("Video", "ResolutionW").ToString());
            VideoOutputShared.Instance.videoResHeight = Double.Parse(configuration.GetInt("Video", "ResolutionH").ToString());
            VideoOutputShared.Instance.Enabled = configuration.GetBool("Video", "Enabled");
            VideoOutputShared.Instance.stretchFourToThree = configuration.GetBool("Video", "StretchToWidescreen");
            VideoOutputShared.Instance.useOverlay = configuration.GetBool("Video", "UseOverlay");
            VideoOutputShared.Instance.overlayPath = configuration.Get("Video", "OverlayPath");
            VideoOutputShared.Instance.useLogo = configuration.GetBool("Video", "UseLogo");
            VideoOutputShared.Instance.logoPath = configuration.Get("Video", "LogoPath");
            VideoOutputShared.Instance.showClock = configuration.GetBool("Video", "ShowClock");


            // BEHAVIOUR 
            AudioPlayerShared.Instance.autoStart = configuration.GetBool("General", "PlayAutoAtStart");
            AudioPlayerShared.Instance.trackQueue = new Core.CustomDataTypes.LinkedListQueue<Database.Models.Track>();

           // VideoEngine.VideoFeed.NDIout ndi = new VideoEngine.VideoFeed.NDIout();
          //  Task.Run(() => ndi.sendVideoFeed()); // remove after testing
            // DONE
            Console.WriteLine("ForgeVision initialization complete.");
            return;
        }

        public void ExecuteForgeAir()
        {
            if (!File.Exists("configuration.ini"))
            {
                System.Windows.MessageBox.Show("ForgeAir Configuration file missing.");
                Environment.Exit(-1);
                return;
            }
            // DATABASE INITIALIZATION

            Database.Shared.DatabaseConnectionProperties.Instance.dbName = configuration.Get("Database", "DatabaseName");
            Database.Shared.DatabaseConnectionProperties.Instance.password = configuration.Get("Database", "Password");
            Database.Shared.DatabaseConnectionProperties.Instance.serverPort = configuration.GetInt("Database", "Port").ToString();

            using (ForgeAir.Database.ForgeAirDbContext context = new Database.ForgeAirDbContext())
            {
                context.Database.Migrate(); //create or update db
            }

            // SOUND INITIALIZATION
            OutputDevice mainOutput = OutputDevice.RetreiveOrCreate("mainOutDevice");

            mainOutput.deviceIndex = configuration.GetInt("Audio", "MainOutDevice", 0);
            mainOutput.sampleRate = configuration.GetInt("Audio", "MainOutSampleRate", 48000);
            mainOutput.bitDepth = (DeviceOutputBitDepthEnum)configuration.GetInt("Audio", "MainOutBitDepth", 16);


            string channels = configuration.Get("Audio", "MainOutChannels", "2");
            mainOutput.MMEaudioChannels = (MMEDeviceOutputAudioChannelsEnum)generalHelper.ToMMEDeviceOutputAudioChannelsEnum(channels);
            mainOutput.WASAPIaudioChannels = (WASAPIDeviceOutputAudioChannelsEnum)generalHelper.ToWASAPIDeviceOutputAudioChannelsEnum(channels);



            mainOutput.bufferLength = configuration.GetInt("Audio", "MainOutBuffer");
            mainOutput.useDSound = configuration.GetBool("Audio", "MainOutUseDSound");



            mainOutput.deviceOutputMethod = (DeviceOutputMethodEnum)generalHelper.ToDeviceOutputMethodEnum(configuration.Get("Audio", "MainOutDeviceMethod"));


            AudioPlayerShared.Instance.deviceManager.LoadOutputDevice(mainOutput);


            // AUDIO ENGINE INITIALIZATION
            AudioPlayerShared.Instance.crossfadeTimeInMs = 1500;
            AudioPlayerShared.Instance.fadeNextTimeInMs = 1500;
            AudioPlayerShared.Instance.repeatTrack = false;
            AudioPlayerShared.Instance.fixClickingWorkaround = configuration.GetBool("Audio", "FixClickingWorkAround");


            // DSP INITIALIZATION

            DSPShared.Instance.dspEngine = new Core.AudioEngine.BuiltInDSP();
            DSPShared.Instance.isEnabled = configuration.GetBool("Built-In DSP", "Enabled");
            DSPShared.Instance.usingAM = configuration.GetBool("Built-In DSP", "AM");
            DSPShared.Instance.usingAMStereo = configuration.GetBool("Built-In DSP", "AMStereo");
            DSPShared.Instance.usingFM = configuration.GetBool("Built-In DSP", "FM");

            if (DSPShared.Instance.isEnabled)
            {
                AudioEQPresetsEnum preset = DSPShared.Instance.usingFM ? AudioEQPresetsEnum.FM :
             DSPShared.Instance.usingAM ? AudioEQPresetsEnum.AM :
             DSPShared.Instance.usingAMStereo ? AudioEQPresetsEnum.AMSTEREO :
             AudioEQPresetsEnum.None;

                DSPShared.Instance.dspEngine.ApplyPreset(preset);

            }

            // VST INITIALIZATION

            VSTEffect.Instance.useEffect = configuration.GetBool("VST", "Enabled");
            VSTEffect.Instance.effectPath = configuration.Get("VST", "EffectPath");

            if (VSTEffect.Instance.useEffect && !File.Exists(VSTEffect.Instance.effectPath))
            {
                throw new FileNotFoundException("VST file not found: " + VSTEffect.Instance.effectPath);
            }
            else if (VSTEffect.Instance.useEffect && File.Exists(VSTEffect.Instance.effectPath))
            {
                VSTEffect.Instance.effectManager.InitVSTEffectForHandle(VSTEffect.Instance.effectPath);
            }

            // BUTT

            WebEncoderNowPlaying.Instance.buttExecutable = configuration.Get("Butt", "Path");
            WebEncoderNowPlaying.Instance.PreText = configuration.Get("Butt", "Pre-Text");
            WebEncoderNowPlaying.Instance.PostText = configuration.Get("Butt", "Post-Text");
            WebEncoderNowPlaying.Instance.noTrackPlaying = configuration.Get("Butt", "NoTrack-Text");
            WebEncoderNowPlaying.Instance.nowPlayingText = configuration.Get("Butt", "Default-Text");

            // NOW PLAYING TEXT FILE
            WebEncoderNowPlaying.Instance.NowPlayingTXT = configuration.Get("Now Playing Text", "OutFile");

            // BEHAVIOUR 
            AudioPlayerShared.Instance.autoStart = configuration.GetBool("General", "PlayAutoAtStart");
            AudioPlayerShared.Instance.trackQueue = new Core.CustomDataTypes.LinkedListQueue<Database.Models.Track>();

            // RDS

           // RDS.DeviceManager deviceManager = new RDS.DeviceManager("rds_devices");
           // deviceManager.LoadEncoder(deviceManager.FindEncoders().FirstOrDefault());


            // DONE
            Console.WriteLine("ForgeVision initialization complete.");
            return;
        }
    }
}
