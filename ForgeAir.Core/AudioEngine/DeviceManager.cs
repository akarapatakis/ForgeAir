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
using ForgeAir.Core.Shared;
using ManagedBass.Vst;
using System.Threading.Channels;
using ManagedBass.Mix;
using ForgeAir.Core.Helpers;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace ForgeAir.Core.AudioEngine
{
    public class DeviceManager
    {
        GeneralHelpers generalHelper = new GeneralHelpers();

        private static DeviceInitFlags ProperbitDepthConvertor(DeviceOutputBitDepthEnum bitDepth) // used to explicit convert local enum to DeviceInitFlags enum
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


        ///
        /// Device Loading Part
        ///


        /// <summary>
        /// Calls BassASIO.Init() with flags set at the required parameter - OutputDevice
        /// </summary>
        /// <param name="device">OutputDevice - contains properties of an output device in order to create flags and locate the device itself </param>
        /// <returns>BASS Init Result (0 = successful)</returns>
        /// <exception cref="Exception">Bass Init Failure Reason (from LastError)</exception>
        /// 
        private int LoadASIODevice(OutputDevice device) // fuck asio fuck asio FUCK ASIO! - most stupid output to implement and actually work
        {
            
            
            if (BassAsio.Init(device.deviceIndex, AsioInitFlags.Thread) == false)
            {
                if (!BassAsio.LastError.HasFlag(Errors.OK)) // piece of shit
                {
                    throw new Exception(Bass.LastError.ToString());
                }
                else
                {
                    Bass.Init();
                }
            }

            AsioDeviceInfo deviceInfo = BassAsio.GetDeviceInfo(device.deviceIndex);

            AsioInfo asioInfo = BassAsio.Info;
            Console.WriteLine($"ASIO Device Output Channels: {asioInfo.Outputs}");

            if (!BassAsio.ChannelEnableBass(true,0, 0, false))
            {
                Console.WriteLine("Failed to enable ASIO channel 0: " + BassAsio.LastError);

            } // this motherfucker returns nochannel, idk why - FIX


            return 0;
        }

        /// <summary>
        /// Calls BassWasapi.Init() with flags set at the required parameter - OutputDevice
        /// </summary>
        /// <param name="device">OutputDevice - contains properties of an output device in order to create flags and locate the device itself </param>
        /// <returns>BASS Init Result (0 = successful)</returns>
        /// <exception cref="Exception">Bass Init Failure Reason (from LastError)</exception>
        private int LoadWASAPIOutDevice(OutputDevice device)
        {
            if (BassWasapi.Init(device.deviceIndex, device.sampleRate, (int)device.WASAPIaudioChannels, Flags: WasapiInitFlags.Shared | WasapiInitFlags.Async | WasapiInitFlags.CategoryMedia) == false)
            {
                throw new Exception(Bass.LastError.ToString());
                return 0;
            }
            else
            {
                Bass.Init();
            }

            if (!BassWasapi.Start())
            {
                throw new Exception(Bass.LastError.ToString());
            }
            Shared.AudioPlayerShared.Instance.currentMainBassMixerHandle = BassMix.CreateMixerStream(device.sampleRate, (int)device.WASAPIaudioChannels, BassFlags.Float | BassFlags.MixerNonStop);
            // apply vst - remove after testing!
          VSTEffectManager vstEffect = new VSTEffectManager();
            
            vstEffect.InitVSTEffectForHandle("vst_stereo_tool.dll");
            return 0;
        }

        /// <summary>
        /// Calls Bass.Init() with flags set at the required parameter - OutputDevice
        /// </summary>
        /// <param name="device">OutputDevice - contains properties of an output device in order to create flags and locate the device itself </param>
        /// <returns>BASS Init Result (0 = successful)</returns>
        /// <exception cref="Exception">Bass Init Failure Reason (from LastError)</exception>
        private int LoadMMEOutDevice(OutputDevice device)
        {
            if (device.useDSound)
            {
                Console.WriteLine("MME: Redirecting to DirectSound based on given flags...");
                return LoadDSoundOutDevice(device);
            }
            Bass.PlaybackBufferLength = device.bufferLength;
            Bass.DeviceNonStop = true;


            if (Bass.Init(device.deviceIndex, device.sampleRate, Flags: (DeviceInitFlags)device.MMEaudioChannels | ProperbitDepthConvertor(device.bitDepth)) == false)
            {
                throw new Exception(Bass.LastError.ToString());
            }
            Shared.AudioPlayerShared.Instance.currentMainBassMixerHandle = BassMix.CreateMixerStream(device.sampleRate, generalHelper.MMEToMixerChans(device.MMEaudioChannels), BassFlags.MixerNonStop | BassFlags.AutoFree | BassFlags.Float);
            return 0;

        }

        /// <summary>
        /// Calls Bass.Init() with flags set at the required parameter - OutputDevice
        /// </summary>
        /// <param name="device">OutputDevice - contains properties of an output device in order to create flags and locate the device itself </param>
        /// <returns>BASS Init Result (0 = successful)</returns>
        /// <exception cref="Exception">Bass Init Failure Reason (from LastError)</exception>
        private int LoadDSoundOutDevice(OutputDevice device) {

            if (!device.useDSound)
            {
                Console.WriteLine("DirectSound: Redirecting to MME based on given flags...");
                return LoadMMEOutDevice(device);
            }
            Bass.PlaybackBufferLength = device.bufferLength;
            Bass.DeviceNonStop = true;

            if (Bass.Init(device.deviceIndex, device.sampleRate, Flags: (DeviceInitFlags)device.MMEaudioChannels | ProperbitDepthConvertor(device.bitDepth) | DeviceInitFlags.DirectSound) == false)
            {
                throw new Exception(Bass.LastError.ToString());
            }
            Shared.AudioPlayerShared.Instance.currentMainBassMixerHandle = BassMix.CreateMixerStream(device.sampleRate, generalHelper.MMEToMixerChans(device.MMEaudioChannels), BassFlags.MixerNonStop | BassFlags.AutoFree | BassFlags.Float);
            Debug.WriteLine(Bass.LastError.ToString());
            return 0;
        }

        /// <summary>
        /// Loads a device from a specific Audio Driver
        /// </summary>
        /// <param name="device">The Output Device to be loaded</param>
        ///
        public int LoadOutputDevice(OutputDevice device)
        {
            switch (device.deviceOutputMethod)
            {
                case DeviceOutputMethodEnum.MME:
                    return LoadMMEOutDevice(device);
                case DeviceOutputMethodEnum.WASAPI:
                    return LoadWASAPIOutDevice(device);
                case DeviceOutputMethodEnum.DirectSound:
                    return LoadDSoundOutDevice(device);
                case DeviceOutputMethodEnum.ASIO:
                    return LoadASIODevice(device);
            }
            return 0;
        }


        ///
        /// Device Listing Part
        ///

        
        /// <summary>
        /// Gets all output devices that are capable of MME (all) - this array can be used for DirectSound
        /// </summary>
        /// <returns>An array that contains the above in "MME:{deviceIndex}:{displayName}"</returns>
        private string[] getMMEOutDevices()
        {
            // Get MME devices

            int totalDeviceCount = Bass.DeviceCount;
            string[] devices = new string[totalDeviceCount];
            int deviceIndex = 0;

            
            int mmeDeviceIndex = 0;
            for (int i = 0; Bass.GetDeviceInfo(i, out DeviceInfo mmeInfo); i++)
            {
                if (!mmeInfo.IsEnabled)
                {
                    continue;
                }
                devices[deviceIndex++] = $"MME:{mmeDeviceIndex++}:{mmeInfo.Name}";
            }
            // Trim the array to the actual number of devices
            Array.Resize(ref devices, deviceIndex);
            return devices;
        }

        /// <summary>
        /// Gets all output devices that are capable of WASAPI
        /// </summary>
        /// <returns>An array that contains the above in "WASAPI:{deviceIndex}:{displayName}"</returns>
        private string[] getWASAPIOutDevices()
        {
            // Get WASAPI devices

            int totalDeviceCount = BassWasapi.DeviceCount;
            string[] devices = new string[totalDeviceCount];
            int deviceIndex = 0;


            int wasapiDeviceIndex = 0;
            for (int i = 0; BassWasapi.GetDeviceInfo(i, out WasapiDeviceInfo wasapiInfo); i++)
            {
                // Skip unwanted devices
                if (wasapiInfo.IsInput || wasapiInfo.IsDisabled || wasapiInfo.IsUnplugged ||  wasapiInfo.Type == WasapiDeviceType.Unknown || wasapiInfo.Type == WasapiDeviceType.LineLevel || wasapiInfo.Type == WasapiDeviceType.Microphone)
                {
                    continue;
                }

                devices[deviceIndex++] = $"WASAPI:{wasapiDeviceIndex++}:{wasapiInfo.Name}";
            }
            // Trim the array to the actual number of devices
            Array.Resize(ref devices, deviceIndex);
            return devices;
        }

        /// <summary>
        /// Gets all ASIO Drivers
        /// </summary>
        /// <returns>An array that contains the above in "ASIO:{deviceIndex}:{displayName}"</returns>
        private string[] getASIODevices()
        {
            // Get ASIO devices

            int totalDeviceCount = BassAsio.DeviceCount;
            string[] devices = new string[totalDeviceCount];
            int deviceIndex = 0;


            int asioDeviceIndex = 0;
            for (int i = 0; BassAsio.GetDeviceInfo(i, out AsioDeviceInfo asioInfo); i++)
            {
                devices[deviceIndex++] = $"ASIO:{asioDeviceIndex++}:{asioInfo.Name}";
            }
            // Trim the array to the actual number of devices
            Array.Resize(ref devices, deviceIndex);
            return devices;
        }

        /// <summary>
        /// Gets all Output Devices from a specific Audio Driver
        /// </summary>
        /// <param name="device">Audio Driver (MME, WASAPI, DSound, ASIO)</param>
        ///
        public string[] GetDevices(DeviceOutputMethodEnum device)
        {
            switch (device)
            {
                case DeviceOutputMethodEnum.MME:
                    return getMMEOutDevices();
                case DeviceOutputMethodEnum.DirectSound:
                    return getMMEOutDevices();
                case DeviceOutputMethodEnum.WASAPI:
                    return getWASAPIOutDevices();
                case DeviceOutputMethodEnum.ASIO:
                    return getASIODevices();
            }
            return Array.Empty<string>();
        }

    }
}
