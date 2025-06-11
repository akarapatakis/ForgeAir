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
            return -1;
        }

        /// <summary>
        /// Calls BassWasapi.Init() with flags set at the required parameter - OutputDevice
        /// </summary>
        /// <param name="device">OutputDevice - contains properties of an output device in order to create flags and locate the device itself </param>
        /// <returns>BASS Init Result (0 = successful)</returns>
        /// <exception cref="Exception">Bass Init Failure Reason (from LastError)</exception>
        private int LoadWASAPIOutDevice(OutputDevice device)
        {
            return -1;
        }

        /// <summary>
        /// Calls Bass.Init() with flags set at the required parameter - OutputDevice
        /// </summary>
        /// <param name="device">OutputDevice - contains properties of an output device in order to create flags and locate the device itself </param>
        /// <returns>BASS Init Result (0 = successful)</returns>
        /// <exception cref="Exception">Bass Init Failure Reason (from LastError)</exception>
        private int LoadMMEOutDevice(OutputDevice device)
        {
            return -1;
        }

        /// <summary>
        /// Calls Bass.Init() with flags set at the required parameter - OutputDevice
        /// </summary>
        /// <param name="device">OutputDevice - contains properties of an output device in order to create flags and locate the device itself </param>
        /// <returns>BASS Init Result (0 = successful)</returns>
        /// <exception cref="Exception">Bass Init Failure Reason (from LastError)</exception>
        private int LoadDSoundOutDevice(OutputDevice device) {

            return -1;
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
            return Array.Empty<string>();
        }

        /// <summary>
        /// Gets all output devices that are capable of WASAPI
        /// </summary>
        /// <returns>An array that contains the above in "WASAPI:{deviceIndex}:{displayName}"</returns>
        private string[] getWASAPIOutDevices()
        {
            return Array.Empty<string>();
        }

        /// <summary>
        /// Gets all ASIO Drivers
        /// </summary>
        /// <returns>An array that contains the above in "ASIO:{deviceIndex}:{displayName}"</returns>
        private string[] getASIODevices()
        {
            return Array.Empty<string>();
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
