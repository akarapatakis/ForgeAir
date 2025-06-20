using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.AudioPlayout.DSP.VST.Interfaces;
using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Host;
using Jacobi.Vst.Host.Interop;
using NAudio.Wave;

namespace ForgeAir.Core.Services.AudioPlayout.DSP.VST
{
    class NAudioVSTService : IVSTService
    {
        public VSTPlugin _loadedPlugin { get; }
        private VSTConfigurationManager _configurationManager { get; }
        public NAudioDevice _targetDevice { get; }

        private VstPluginContext Context;

        public NAudioVSTService() { }
        public void Init()
        {

            // Create the host command stub
            var hostCommandStub = new HostCommandStub();

            // Load the VST plugin
            Context = VstPluginContext.Create(_loadedPlugin.Path, hostCommandStub);

            // Create a VST wave provider
            var vstWaveProvider = new VstWaveProvider(Context, _targetDevice.AudioStream);

        }
        public void Kill()
        {
            Context.Dispose();
        }
    }

    public class HostCommandStub : IVstHostCommandStub
    {
        public IVstPluginContext PluginContext { get; set; }

        public IVstHostCommands20 Commands => throw new NotImplementedException();

        public bool BeginEdit(int index) => false;
        public VstCanDoResult CanDo(string cando) => VstCanDoResult.Unknown;
        public bool CloseFileSelector(VstFileSelect fileSelect) => false;
        public bool EndEdit(int index) => false;
        public VstAutomationStates GetAutomationState() => VstAutomationStates.Off;
        public int GetBlockSize() => 1024;
        public string GetDirectory() => string.Empty;
        public int GetInputLatency() => 0;
        public VstHostLanguage GetLanguage() => VstHostLanguage.NotSupported;
        public int GetOutputLatency() => 0;
        public VstProcessLevels GetProcessLevel() => VstProcessLevels.Unknown;
        public string GetProductString() => "VST.NET";
        public float GetSampleRate() => 44100f;
        public VstTimeInfo GetTimeInfo(VstTimeInfoFlags filterFlags) => null;
        public string GetVendorString() => "VendorString";
        public int GetVendorVersion() => 2400;
        public bool IoChanged() => false;
        public bool OpenFileSelector(VstFileSelect fileSelect) => false;
        public bool ProcessEvents(VstEvent[] events) => false;
        public bool SizeWindow(int width, int height) => false;
        public bool UpdateDisplay() => true;
    }

    public class VstWaveProvider : ISampleProvider
    {
        private readonly VstPluginContext vstPluginContext;
        private readonly ISampleProvider sourceProvider;
        private readonly int channelCount;
        private readonly int blockSize = 1024;
        private readonly VstAudioBufferManager inputMgr;
        private readonly VstAudioBufferManager outputMgr;
        private readonly float[][] inputBuffers;
        private readonly float[][] outputBuffers;

        public VstWaveProvider(VstPluginContext vstPluginContext, ISampleProvider sourceProvider)
        {
            this.vstPluginContext = vstPluginContext;
            this.sourceProvider = sourceProvider;
            this.channelCount = sourceProvider.WaveFormat.Channels;

            inputMgr = new VstAudioBufferManager(channelCount, blockSize);
            outputMgr = new VstAudioBufferManager(channelCount, blockSize);
            inputBuffers = CreateBufferArray(channelCount, blockSize);
            outputBuffers = CreateBufferArray(channelCount, blockSize);

            // VST plugin setup
            vstPluginContext.PluginCommandStub.Commands.Open();
            vstPluginContext.PluginCommandStub.Commands.SetSampleRate(sourceProvider.WaveFormat.SampleRate);
            vstPluginContext.PluginCommandStub.Commands.SetBlockSize(blockSize);
            vstPluginContext.PluginCommandStub.Commands.MainsChanged(true);
            vstPluginContext.PluginCommandStub.Commands.StartProcess();
        }

        public WaveFormat WaveFormat => sourceProvider.WaveFormat;

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = sourceProvider.Read(buffer, offset, count);
            int frames = samplesRead / channelCount;

            // Step 1: Interleaved float[] → inputBuffers[channel][frame]
            for (int i = 0; i < frames; i++)
            {
                for (int ch = 0; ch < channelCount; ch++)
                {
                    inputBuffers[ch][i] = buffer[offset + i * channelCount + ch];
                }
            }

            var input = inputMgr.Buffers.ToArray();
            var output = outputMgr.Buffers.ToArray();

            // Step 2: Copy inputBuffers → VST input
            for (int ch = 0; ch < channelCount; ch++)
            {
                inputBuffers[ch].AsSpan(0, frames).CopyTo(input[ch].AsSpan());
            }

            // Step 3: Process with VST
            vstPluginContext.PluginCommandStub.Commands.ProcessReplacing(input, output);

            // Step 4: Copy VST output back to interleaved float[]
            for (int i = 0; i < frames; i++)
            {
                for (int ch = 0; ch < channelCount; ch++)
                {
                    buffer[offset + i * channelCount + ch] = output[ch][i];
                }
            }

            return samplesRead;
        }

        private float[][] CreateBufferArray(int channels, int size)
        {
            float[][] result = new float[channels][];
            for (int i = 0; i < channels; i++)
                result[i] = new float[size];
            return result;
        }
    }

}
