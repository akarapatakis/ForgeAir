    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ForgeAir.Core.AudioEngine.Enums;
    using ForgeAir.Core.Helpers;
    using ForgeAir.Core.Models;
    using ManagedBass;
    using ManagedBass.Asio;
    using ManagedBass.Mix;
    using ManagedBass.Wasapi;

    namespace ForgeAir.Core.Services.DeviceManager
    {
        public class BassManager : Interfaces.IDeviceManager
        {
            BassDevice device;

            public BassManager(BassDevice _device) {
                this.device = _device;
                if (_device != null)
                {
                    InitDevice();
                }
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
            private int LoadASIODevice() // fuck asio fuck asio FUCK ASIO! - most stupid output to implement and actually work
            {
                if (BassAsio.Init(device.TargetDevice.Index, AsioInitFlags.Thread) == false)
                {
                    if (!BassAsio.LastError.HasFlag(Errors.OK)) // piece of shit
                    {
                        throw new Exception(BassCheatsheet.LastErrorToDetailedError(Bass.LastError));
                    }
                    else
                    {
                        Bass.Init();
                    }
                }

                AsioDeviceInfo deviceInfo = BassAsio.GetDeviceInfo(device.TargetDevice.Index);

                AsioInfo asioInfo = BassAsio.Info;
                Console.WriteLine($"ASIO Device Output Channels: {asioInfo.Outputs}");

                if (!BassAsio.ChannelEnableBass(true, 0, 0, false))
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
            private int LoadWASAPIOutDevice()
            {
                if (BassWasapi.Init(device.TargetDevice.Index, device.TargetDevice.SampleRate, device.TargetDevice.Channels, Flags: WasapiInitFlags.Shared | WasapiInitFlags.Async | WasapiInitFlags.CategoryMedia) == false)
                {
                throw new Exception(BassCheatsheet.LastErrorToDetailedError(Bass.LastError));
                return 0;
                }
                else
                {
                    Bass.Init();
                }

                if (!BassWasapi.Start())
                {
                throw new Exception(BassCheatsheet.LastErrorToDetailedError(Bass.LastError));
            }
            device.Handle = BassMix.CreateMixerStream(device.TargetDevice.SampleRate, device.TargetDevice.Channels, BassFlags.Float | BassFlags.MixerNonStop);

                return 0;
            }

            /// <summary>
            /// Calls Bass.Init() with flags set at the required parameter - OutputDevice
            /// </summary>
            /// <param name="device">OutputDevice - contains properties of an output device in order to create flags and locate the device itself </param>
            /// <returns>BASS Init Result (0 = successful)</returns>
            /// <exception cref="Exception">Bass Init Failure Reason (from LastError)</exception>
            private int LoadMMEOutDevice()
            {
                Bass.PlaybackBufferLength = device.TargetDevice.BufferLength;
                Bass.DeviceNonStop = true;


            if (Bass.Init(device.TargetDevice.Index, device.TargetDevice.BufferLength, Flags: (DeviceInitFlags)device.TargetDevice.MMEaudioChannels | GeneralHelpers.ProperbitDepthConvertor(device.TargetDevice.BitDepth)) == false)
            {
                if (!Bass.LastError.HasFlag(Errors.Already))
                {
                    throw new Exception(BassCheatsheet.LastErrorToDetailedError(Bass.LastError));

                }
            }
                device.Handle = BassMix.CreateMixerStream(device.TargetDevice.SampleRate, GeneralHelpers.MMEToMixerChans(device.TargetDevice.MMEaudioChannels), BassFlags.MixerNonStop | BassFlags.AutoFree | BassFlags.Float);
                return 0;

            }

            /// <summary>
            /// Calls Bass.Init() with flags set at the required parameter - OutputDevice
            /// </summary>
            /// <param name="device">OutputDevice - contains properties of an output device in order to create flags and locate the device itself </param>
            /// <returns>BASS Init Result (0 = successful)</returns>
            /// <exception cref="Exception">Bass Init Failure Reason (from LastError)</exception>
            private int LoadDSoundOutDevice()
            {

                Bass.PlaybackBufferLength = device.TargetDevice.BufferLength;
                Bass.DeviceNonStop = true;

                if (Bass.Init(device.TargetDevice.Index, device.TargetDevice.SampleRate, Flags: (DeviceInitFlags)device.TargetDevice.MMEaudioChannels | GeneralHelpers.ProperbitDepthConvertor(device.TargetDevice.BitDepth) | DeviceInitFlags.DirectSound) == false)
                {
                throw new Exception(BassCheatsheet.LastErrorToDetailedError(Bass.LastError));
            }
            device.Handle = BassMix.CreateMixerStream(device.TargetDevice.SampleRate, GeneralHelpers.MMEToMixerChans(device.TargetDevice.MMEaudioChannels), BassFlags.MixerNonStop | BassFlags.AutoFree | BassFlags.Float);
                Debug.WriteLine(Bass.LastError.ToString());
                return 0;
            }

            /// <summary>
            /// Loads a device from a specific Audio Driver
            /// </summary>
            /// <param name="device">The Output Device to be loaded</param>
            ///
            public int InitDevice()
            {
                switch (device.TargetDevice.API)
                {
                    case DeviceOutputMethodEnum.MME:
                        return LoadMMEOutDevice();
                    case DeviceOutputMethodEnum.WASAPI:
                        return LoadWASAPIOutDevice();
                    case DeviceOutputMethodEnum.DirectSound:
                        return LoadDSoundOutDevice();
                    case DeviceOutputMethodEnum.ASIO:
                        return LoadASIODevice();
                }
                return 0;
            }


            ///
            /// Device Listing Part
            ///


            /// <summary>
            /// Gets all output devices that are capable of MME (all) - this array can be used for DirectSound
            /// </summary>
            /// <returns>A list that contains OutputDevice with their ids and names ready to be intialized</returns>
            private static List<OutputDevice> getMMEOutDevices()
            {
                List<OutputDevice> devices = new List<OutputDevice>();

                int mmeDeviceIndex = 0;
                for (int i = 0; Bass.GetDeviceInfo(i, out DeviceInfo mmeInfo); i++)
                {
                    if (!mmeInfo.IsEnabled)
                    {
                        continue;
                    }

                    devices.Add(new OutputDevice
                    {
                        API = DeviceOutputMethodEnum.MME,
                        Index = mmeDeviceIndex++,
                        Name = mmeInfo.Name,

                    });
                }
                return devices;
            }

            /// <summary>
            /// Gets all output devices that are capable of WASAPI
            /// </summary>
            /// <returns>A list that contains OutputDevice with their ids and names ready to be intialized</returns>
            private static List<OutputDevice> getWASAPIOutDevices()
            {
                int totalDeviceCount = BassWasapi.DeviceCount;
                List<OutputDevice> devices = new List<OutputDevice>();

                int wasapiDeviceIndex = 0;
                for (int i = 0; BassWasapi.GetDeviceInfo(i, out WasapiDeviceInfo wasapiInfo); i++)
                {
                    // Skip unwanted devices
                    if (wasapiInfo.IsInput || wasapiInfo.IsDisabled || wasapiInfo.IsUnplugged || wasapiInfo.Type == WasapiDeviceType.Unknown || wasapiInfo.Type == WasapiDeviceType.LineLevel || wasapiInfo.Type == WasapiDeviceType.Microphone)
                    {
                        continue;
                    }

                    devices.Add(new OutputDevice
                    {
                        API = DeviceOutputMethodEnum.WASAPI,
                        Index = wasapiDeviceIndex++,
                        Name = wasapiInfo.Name,

                    });
                }
                return devices;
            }

            /// <summary>
            /// Gets all ASIO Drivers
            /// </summary>
            /// <returns>A list that contains OutputDevice with their ids and names ready to be intialized</returns>
            private static List<OutputDevice> getASIODevices()
            {
                int totalDeviceCount = BassAsio.DeviceCount;
                List<OutputDevice> devices = new List<OutputDevice>();

                int asioDeviceIndex = 0;
                for (int i = 0; BassAsio.GetDeviceInfo(i, out AsioDeviceInfo asioInfo); i++)
                {
                    devices.Add(new OutputDevice
                    {
                        API = DeviceOutputMethodEnum.ASIO,
                        Index = asioDeviceIndex++,
                        Name = asioInfo.Name,

                    });
                }
                return devices;
            }

            /// <summary>
            /// Does the same as ListDevicesByAPI but External to allow "emegerncy" calls from places that do not have access to a DeviceManager
            /// </summary>
            /// <param name="api">Audio Driver (MME, WASAPI, DSound, ASIO)</param>
            public static List<OutputDevice> ListDevicesByAPI_Ex(DeviceOutputMethodEnum api)
            {
                    switch (api)
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
                    return new List<OutputDevice>();
            }

            /// <summary>
            /// Gets all Output Devices from a specific Audio Driver
            /// </summary>
            /// <param name="api">Audio Driver (MME, WASAPI, DSound, ASIO)</param>
            ///
            public List<OutputDevice> ListDevicesByAPI(DeviceOutputMethodEnum api)
            {
                switch (api)
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
                return new List<OutputDevice>();
            }

            public int FreeDevice()
            {
                throw new NotImplementedException();
            }
        }
    }
