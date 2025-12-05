using ForgeAir.Core.AudioEngine.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.DeviceManager.Converters
{
    public static class ToEnum
    {
        public static DeviceTypeEnum ToDeviceTypeEnum(string value)
        {
            return value.ToLower() switch
            {
                "MainOutput" => DeviceTypeEnum.Main,
                "BoothOutput" => DeviceTypeEnum.Booth,
                "FxOutput" => DeviceTypeEnum.Instants,
                _ => throw new ArgumentException("Invalid device type")
            };
        }
        public static string FromDeviceTypeEnum(DeviceTypeEnum value)
        {
            return value switch
            {
                DeviceTypeEnum.Main => "MainOutput",
                DeviceTypeEnum.Booth => "BoothOutput",
                DeviceTypeEnum.Instants => "FxOutput",
                _ => throw new ArgumentException("Invalid device type")
            };
        }

        public static DeviceOutputBitDepthEnum ToDeviceOutputBitDepthEnum(int value)
        {
            return value switch
            {
                8 => DeviceOutputBitDepthEnum.EightBit,
                16 => DeviceOutputBitDepthEnum.SixteenBit,
                24 => DeviceOutputBitDepthEnum.TwentyfourBit,
                32 => DeviceOutputBitDepthEnum.ThirtyTwoBit,
                _ => throw new ArgumentException("Invalid bit depth")
            };
        }
        public static int FromDeviceOutputBitDepthEnum(DeviceOutputBitDepthEnum value)
        {
            return value switch
            {
                DeviceOutputBitDepthEnum.EightBit => 8,
                DeviceOutputBitDepthEnum.SixteenBit => 16,
                DeviceOutputBitDepthEnum.TwentyfourBit => 24,
                DeviceOutputBitDepthEnum.ThirtyTwoBit => 32,

                _ => throw new ArgumentException("Invalid bit depth")
            };
        }

        public static DeviceOutputMethodEnum ToDeviceAPI(string value)
        {
            return value switch
            {
                "MME" => DeviceOutputMethodEnum.MME,
                "WASAPI" => DeviceOutputMethodEnum.WASAPI,
                "DirectSound" => DeviceOutputMethodEnum.DirectSound,
                "ASIO" => DeviceOutputMethodEnum.ASIO,
                _ => throw new ArgumentException("Invalid device api")
            };
        }
        public static string FromDeviceAPI(DeviceOutputMethodEnum value)
        {
            return value switch
            {
                DeviceOutputMethodEnum.MME => "MME",
                DeviceOutputMethodEnum.DirectSound => "DirectSound",
                DeviceOutputMethodEnum.WASAPI => "WASAPI",
                DeviceOutputMethodEnum.ASIO => "ASIO",

                _ => throw new ArgumentException("Invalid device api")
            };
        }
    }
}
