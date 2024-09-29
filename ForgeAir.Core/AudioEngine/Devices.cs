using ManagedBass.Asio;
using ManagedBass.Wasapi;
using ManagedBass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.AudioEngine.Enums;
using System.Reflection.Metadata.Ecma335;

namespace ForgeAir.Core.AudioEngine
{
    public class Devices
    {

        public static DeviceInitFlags ProperbitDepthConvertor(DeviceOutputBitDepthEnum bitDepth)
        {

            /*switch (bitDepth)
            {
                case DeviceOutputBitDepthEnum.EightBit:
                    return DeviceInitFlags.Byte;

                case DeviceOutputBitDepthEnum.SixteenBit:
                    return DeviceInitFlags.Bits16;

                case DeviceOutputBitDepthEnum.TwentyfourBit:
                    return DeviceInitFlags.Default;

                case DeviceOutputBitDepthEnum.ThirtyTwoBit:
                    return DeviceInitFlags.Default;
                default:
                    break;
            }
            return DeviceInitFlags.Default;*/

            return (DeviceInitFlags)bitDepth;
            }

            
        

        public void LoadASIODevice(int deviceIndex, int deviceFreq, DeviceOutputAudioChannelsEnum channel, DeviceOutputBitDepthEnum bitDepth, int bufferLength)
        {

            
            Bass.PlaybackBufferLength = bufferLength;
            Bass.DeviceNonStop = true;

            Bass.Init(deviceIndex, deviceFreq, Flags: (DeviceInitFlags)channel | ProperbitDepthConvertor(bitDepth));


        }

        public void LoadWASAPIDevice(int deviceIndex, int deviceFreq, DeviceOutputAudioChannelsEnum channel, DeviceOutputBitDepthEnum bitdepth, int bufferLength)
        {
            Bass.PlaybackBufferLength = bufferLength;
            Bass.DeviceNonStop = true;
        }

        public void LoadMMEDevice(int deviceIndex, int deviceFreq, DeviceOutputAudioChannelsEnum channel, DeviceOutputBitDepthEnum bitdepth, int bufferLength, bool useDsound = false)
        {
            Bass.PlaybackBufferLength = bufferLength;
            Bass.DeviceNonStop = true;


        }


        public string[] GetDevices()
        {

            int totalDeviceCount = Bass.DeviceCount + BassAsio.DeviceCount + BassWasapi.DeviceCount;
            string[] devices = new string[totalDeviceCount];

            int deviceIndex = 0;

            // Get MME devices
            int mmeDeviceIndex = 0;
            for (int i = 0; Bass.GetDeviceInfo(i, out DeviceInfo mmeInfo); i++)
            {
                devices[deviceIndex++] = $"MME:{mmeDeviceIndex++}:{mmeInfo.Name}";
            }


            // Get ASIO devices
            int asioDeviceIndex = 0;
            for (int i = 0; BassAsio.GetDeviceInfo(i, out AsioDeviceInfo asioInfo); i++)
            {
                devices[deviceIndex++] = $"ASIO:{asioDeviceIndex++}:{asioInfo.Name}";
            }

            // Get WASAPI devices
            int wasapiDeviceIndex = 0;
            for (int i = 0; BassWasapi.GetDeviceInfo(i, out WasapiDeviceInfo wasapiInfo); i++)
            {
                // Skip unwanted devices
                if (wasapiInfo.IsInput || wasapiInfo.IsDisabled || wasapiInfo.IsUnplugged || wasapiInfo.Type == WasapiDeviceType.Unknown || wasapiInfo.Type == WasapiDeviceType.LineLevel || wasapiInfo.Type == WasapiDeviceType.Microphone)
                {
                    continue;
                }

                devices[deviceIndex++] = $"WASAPI:{wasapiDeviceIndex++}:{wasapiInfo.Name}";
            }

            // Trim the array to the actual number of devices
            Array.Resize(ref devices, deviceIndex);

            return devices;
        }

        public void InitDevice(string[] devices, int selectedIndex, int freq, bool isMono, bool useDsound)
        {
            int audioChannels;
            if (selectedIndex < 0 || selectedIndex >= devices.Length)
            {
                Console.WriteLine("Invalid device index");
                return;
            }

            string selectedDevice = devices[selectedIndex];
            string[] parts = selectedDevice.Split(':');

            if (parts.Length != 3)
            {
                Console.WriteLine("Invalid device string format");
                return;
            }

            if (isMono)
            {
                audioChannels = 1;
            }
            else { audioChannels = 2; }
            string deviceType = parts[0];
            string deviceIndex = parts[1];
            string deviceName = parts[2];


            switch (deviceType)
            {
                case "MME":
                    Console.WriteLine($"Selected MME device: {deviceIndex}: {deviceName}");
                    //LoadMMEDevice(Int32.Parse(deviceIndex), freq, audioChannels, useDsound, 150);
                    break;
                case "ASIO":
                    Console.WriteLine($"Selected ASIO device: {deviceIndex}: {deviceName}");
                    // Additional code to handle ASIO device selection
                    break;
                case "WASAPI":
                    Console.WriteLine($"Selected WASAPI device: {deviceIndex}: {deviceName}");
                    // Additional code to handle WASAPI device selection
                    break;
                default:
                    Console.WriteLine("Unknown device type");
                    break;
            }
        }
    }
}
