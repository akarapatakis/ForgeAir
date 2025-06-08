using ForgeAir.Core.Shared;
using ManagedBass;
using ManagedBass.Vst;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.AudioEngine
{
    public class VSTEffectManager
    {
        public VSTEffectManager() { }

        public void InitVSTEffectForHandle(string dllPath)
        {
            // Remove any previously applied VST effects
            KillVSTEffect(VSTEffect.Instance.effectHandle);

            if (!VSTEffect.Instance.useEffect)
            {
                return;
            }
            VSTEffect.Instance.effectHandle = 0;

            if (AudioPlayerShared.Instance.currentMainBassMixerHandle == 0)
            {
                throw new Exception("Invalid track handle: Cannot apply VST");
            }

            // Apply the VST effect to the existing stream
            int dspHandle = BassVst.ChannelSetDSP(AudioPlayerShared.Instance.currentMainBassMixerHandle, dllPath, BassVstDsp.Default, 0);

            if (dspHandle == 0)
            {
                throw new Exception("Failed to apply VST DSP: " + Bass.LastError);
            }

            Shared.VSTEffect.Instance.effectPath = dllPath;
            if (File.Exists($"{Path.GetFileName(VSTEffect.Instance.effectPath)}.conf"))
            {


                RestoreVSTSettings(dspHandle);

            }

            // Store the DSP handle if needed
            VSTEffect.Instance.effectHandle = dspHandle;


        }

        public void RestoreVSTSettings(int dspHandle)
        {
            if (dspHandle == 0)
            {
                return;
            }

            if (!File.Exists($"{Path.GetFileName(VSTEffect.Instance.effectPath)}.conf"))
            {
                return;
            }

            string[] lines = File.ReadAllLines($"{Path.GetFileName(VSTEffect.Instance.effectPath)}.conf");

            for (int i = 0; i < lines.Length; i++)
            {
                if (float.TryParse(lines[i], out float paramValue))
                {
                    BassVst.SetParam(dspHandle, i, paramValue);
                }
                else
                {
                    Console.WriteLine($"Skipping invalid parameter value: {lines[i]}");
                }
            }
        }



        public void SaveVSTSettings()
        {
            if (VSTEffect.Instance.effectHandle == 0)
                return;

            string configPath = $"{Path.GetFileName(VSTEffect.Instance.effectPath)}.conf";
            File.Delete(configPath);

            int paramCount = BassVst.GetParamCount(VSTEffect.Instance.effectHandle);
            List<string> paramValues = new List<string>();

            for (int i = 0; i < paramCount; i++)
            {
                float value = BassVst.GetParam(VSTEffect.Instance.effectHandle, i);
                paramValues.Add(value.ToString());
            }

            File.WriteAllLines(configPath, paramValues);

            Console.WriteLine("VST parameters saved.");
        }


        public void KillVSTEffect(int vstHandle)
        {
            if (vstHandle != 0)
            {
                BassVst.ChannelRemoveDSP(AudioPlayerShared.Instance.currentMainBassMixerHandle, vstHandle);
                VSTEffect.Instance.effectHandle = 0;  // Clear handle to avoid further access
            }
        }

        public void OpenVSTConfigurationPage(int vstHandle, nint windowCallingThat)
        {
            BassVst.EmbedEditor(vstHandle, windowCallingThat);
        }
        public double[] GetEditorDimensions(int vstHandle) {

            BassVstInfo? info = GetVstInfo(vstHandle);

            return new double[] { info.Value.EditorWidth, info.Value.EditorHeight };

        }
        public BassVstInfo? GetVstInfo(int vstHandle)
        {
            Task.Run(() =>
            {
                BassVstInfo info = new BassVstInfo();
                if (BassVst.GetInfo(vstHandle, out info))
                {
                    return info;
                }
                else
                {
                    return info;
                }
            });
            return null;

        }

    }

}
