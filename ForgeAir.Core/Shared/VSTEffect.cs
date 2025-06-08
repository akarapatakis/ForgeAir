using ForgeAir.Core.AudioEngine;
using ManagedBass.Vst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Shared
{

    // this is written in a hurry so don't wonder why it sucks

    public class VSTEffect
    {
        public bool useEffect { get; set; }
        public int effectHandle { get; set; }
        public BassVstInfo effectInfo { get; set; }
        public string effectPath { get; set; }

        public VSTEffectManager effectManager { get; set; }

        private static VSTEffect? instance;
        public static VSTEffect Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new VSTEffect();
                }
                return instance;
            }
        }
    }
}
