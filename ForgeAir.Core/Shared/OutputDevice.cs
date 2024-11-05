using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.AudioEngine.Enums;

namespace ForgeAir.Core.Shared
{
    public class OutputDevice
    {
        private static Dictionary<string, OutputDevice> instances = new Dictionary<string, OutputDevice>();
        private Dictionary<string, object> data = new Dictionary<string, object>();

        public OutputDevice() { return; }

        public DeviceOutputMethodEnum deviceOutputMethod { get; set; }

        public int deviceIndex { get; set; }
        public bool useDSound {  get; set; }
        public int sampleRate { get; set; } = 44100;
        public DeviceOutputAudioChannelsEnum audioChannels { get; set; } = DeviceOutputAudioChannelsEnum.Stereo;

        public DeviceOutputBitDepthEnum bitDepth { get; set; } = DeviceOutputBitDepthEnum.SixteenBit;
        public int bufferLength { get; set; } = 150;

        public static OutputDevice RetreiveOrCreate(string name)
        {
            if (!instances.ContainsKey(name))
            {
                instances[name] = new OutputDevice();
            }
            return instances[name];
        }
    }
}
