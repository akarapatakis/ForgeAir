using ForgeAir.Core.Shared;
using ManagedBass;
using ManagedBass.Fx;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.AudioEngine
{
    public class BuiltInDSP
    {

        public PeakEQParameters[] AMStereoAudioOpt()
        {
            PeakEQParameters[] bands = new PeakEQParameters[6]; // ← 6, not 5

            bands[0] = new PeakEQParameters
            {
                fBandwidth = 1.5f,
                fCenter = 50f,
                fGain = -6.0f,
                lBand = 0,
                lChannel = 0
            };
            bands[1] = new PeakEQParameters
            {
                fBandwidth = 1.5f,
                fCenter = 80f,
                fGain = 2.0f,
                lBand = 0,
                lChannel = 0
            };
            bands[2] = new PeakEQParameters
            {
                fBandwidth = 1.5f,
                fCenter = 250f,
                fGain = 2.0f,
                lBand = 0,
                lChannel = 0
            };
            bands[3] = new PeakEQParameters
            {
                fBandwidth = 1.0f,
                fCenter = 3500f,
                fGain = 3.0f,
                lBand = 0,
                lChannel = 0
            };
            bands[4] = new PeakEQParameters
            {
                fBandwidth = 1.5f,
                fCenter = 6500f,
                fGain = 2.5f,
                lBand = 0,
                lChannel = 0
            };
            bands[5] = new PeakEQParameters
            {
                fBandwidth = 2.0f,
                fCenter = 7500f,
                fGain = 3.0f,
                lBand = 0,
                lChannel = 0
            };

            return bands;
        }

        public PeakEQParameters[] AMAudioOpt()
        {
            PeakEQParameters[] bands = new PeakEQParameters[6];

            bands[0] = new PeakEQParameters
            {
                fBandwidth = 1.5f,
                fCenter = 80f,
                fGain = 1.5f,
                lBand = 0,
                lChannel = 0
            };
            bands[1] = new PeakEQParameters
            {
                fBandwidth = 1.5f,
                fCenter = 250f,
                fGain = 1.5f,
                lBand = 1,
                lChannel = 0
            };
            bands[2] = new PeakEQParameters
            {
                fBandwidth = 1.0f,
                fCenter = 3500f,
                fGain = 3.0f,
                lBand = 2,
                lChannel = 0
            };
            bands[3] = new PeakEQParameters
            {
                fBandwidth = 1.5f,
                fCenter = 5500f,
                fGain = 2.0f,
                lBand = 3,
                lChannel = 0
            };
            bands[4] = new PeakEQParameters
            {
                fBandwidth = 1.5f,
                fCenter = 50f,
                fGain = -6.0f,
                lBand = 4,
                lChannel = 0
            };
            bands[5] = new PeakEQParameters
            {
                fBandwidth = 1.5f,
                fCenter = 7500f,
                fGain = -12.0f,
                lBand = 5,
                lChannel = 0
            };

            return bands;
        }

        public PeakEQParameters[] FMAudioOpt()
        {
            PeakEQParameters[] bands = new PeakEQParameters[6];

            bands[0] = new PeakEQParameters
            {
                fBandwidth = 1.5f,
                fCenter = 80f,
                fGain = 2.0f,
                lBand = 0,
                lChannel = 0
            };
            bands[1] = new PeakEQParameters
            {
                fBandwidth = 1.5f,
                fCenter = 250f,
                fGain = 2.0f,
                lBand = 1,
                lChannel = 0
            };
            bands[2] = new PeakEQParameters
            {
                fBandwidth = 1.0f,
                fCenter = 3500f,
                fGain = 3.0f,
                lBand = 2,
                lChannel = 0
            };
            bands[3] = new PeakEQParameters
            {
                fBandwidth = 1.5f,
                fCenter = 10000f,
                fGain = 2.5f,
                lBand = 3,
                lChannel = 0
            };
            bands[4] = new PeakEQParameters
            {
                fBandwidth = 1.5f,
                fCenter = 15000f,
                fGain = 2.0f,
                lBand = 4,
                lChannel = 0
            };
            bands[5] = new PeakEQParameters
            {
                fBandwidth = 1.5f,
                fCenter = 50f,
                fGain = -6.0f,
                lBand = 5,
                lChannel = 0
            };

            return bands;
        }



        public void ApplyPreset(Enums.AudioEQPresetsEnum preset) {
            if (AudioPlayerShared.Instance.currentMainBassMixerHandle == 0)
            {
                throw new Exception("Invalid track handle: Cannot apply VST");
            }
            int eqFx = Bass.ChannelSetFX(Shared.AudioPlayerShared.Instance.currentMainBassMixerHandle, EffectType.PeakEQ, 0);

            switch (preset)
            {
                case Enums.AudioEQPresetsEnum.AMSTEREO:

                    foreach (var band in AMStereoAudioOpt())
                    {
                        Bass.FXSetParameters(eqFx, band);
                    }
                    Debug.WriteLine(Bass.LastError.ToString());
                    break;
                case Enums.AudioEQPresetsEnum.AM:

                    foreach (var band in AMAudioOpt())
                    {
                        Bass.FXSetParameters(eqFx, band);
                    }
                    Debug.WriteLine(Bass.LastError.ToString());
                    break;
                case Enums.AudioEQPresetsEnum.FM:

                    foreach (var band in FMAudioOpt())
                    {
                        Bass.FXSetParameters(eqFx, band);
                    }
                    Debug.WriteLine(Bass.LastError.ToString());
                    break;
                default:
                    break;
            }
        
        }
    }
}
