using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ForgeAir.Core.Services.DeviceManager;
using ForgeAir.Core.Models;
using ForgeAir.Core.AudioEngine.Enums;
using ForgeAir.Core.Services.AudioPlayout.Players;
using ForgeAir.Core.DTO;
using System;
using System.IO;
using System.Linq;

namespace ForgeAir.TestingConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new Program().TestNAudio();
        }

        public void TestBass()
        {
            if (!(File.Exists("bass.dll") && File.Exists("bassmix.dll") &&
                  File.Exists("basswasapi.dll") && File.Exists("bassasio.dll")))
            {
                Console.WriteLine("Missing BASS libraries");
                return;
            }

            var services = new ServiceCollection();
            services.AddLogging(cfg => cfg.AddConsole());

            var loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("ForgeAirTest");

            logger.LogInformation("ForgeAir BASS test starting...");

            string[] devices = BassManager.ListDevicesByAPI(DeviceOutputMethodEnum.MME);

            if (devices.Length == 0)
            {
                Console.WriteLine("[-] No output devices found.");
                return;
            }

            for (int i = 0; i < devices.Length; i++)
            {
                Console.WriteLine($"{i}: {devices[i]}");
            }

            Console.Write("Select output device index: ");
            if (!int.TryParse(Console.ReadLine(), out int selectedIndex))
            {
                Console.WriteLine("[-] Invalid index.");
                return;
            }

            var device = new BassDevice
            {
                TargetDevice = new OutputDevice
                {
                    Index = selectedIndex,
                    Name = devices[selectedIndex],
                    SampleRate = 44100,
                    Channels = 2,
                    BufferLength = 500,
                    MMEaudioChannels = MMEDeviceOutputAudioChannelsEnum.Stereo,
                    BitDepth = DeviceOutputBitDepthEnum.SixteenBit,
                    API = DeviceOutputMethodEnum.MME
                }
            };

            var manager = new BassManager(device);
            try
            {
                manager.InitDevice();
                Console.WriteLine("[+] Device initialized.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[-] Device init failed: {ex.Message}");
                return;
            }

            var player = new BassPlayer(device);
            player.Play(new TrackDTO { FilePath = "test1.mp3" }, null);
            Console.WriteLine("[!] Playing test1.mp3...");
            Console.ReadKey();

            player.Play(new TrackDTO { FilePath = "test2.flac" }, null);
            Console.WriteLine("[!] Playing test2.flac...");
            Console.ReadKey();
        }



        public void TestNAudio()
        {


            var services = new ServiceCollection();
            services.AddLogging(cfg => cfg.AddConsole());

            var loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("ForgeAirTest");

            logger.LogInformation("[!]ForgeAir NAudio test starting...");

            string[] devices = NAudioManager.ListDevicesByAPI(DeviceOutputMethodEnum.MME);


            for (int i = 0; i < devices.Length; i++)
            {
                Console.WriteLine($"{i}: {devices[i]}");
            }

            Console.Write("[!]Select output device index: ");
            if (!int.TryParse(Console.ReadLine(), out int selectedIndex))
            {
                Console.WriteLine("[-]Invalid index.");
                return;
            }

            var device = new NAudioDevice
            {
                TargetDevice = new OutputDevice
                {
                    Index = selectedIndex,
                    Name = "empty",
                    SampleRate = 44100,
                    Channels = 2,
                    BufferLength = 500,
                    MMEaudioChannels = MMEDeviceOutputAudioChannelsEnum.Stereo,
                    BitDepth = DeviceOutputBitDepthEnum.SixteenBit,
                    API = DeviceOutputMethodEnum.MME
                }
            };

            var manager = new NAudioManager(device);
            try
            {
                manager.InitDevice();
                Console.WriteLine("[+]Device initialized.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[-]Device init failed: {ex.Message}");
                return;
            }

            var player = new NAudioPlayer(device);
            player.Play(new TrackDTO { FilePath = "test1.mp3" }, null);
            Console.WriteLine("[!]Playing test1.mp3...\nPress any key to play the second test");
            Console.ReadKey();

            player.Play(new TrackDTO { FilePath = "test2.flac" }, null);
            Console.WriteLine("[!]Playing test2.flac...");
            Console.ReadKey();
        }
    }
}
