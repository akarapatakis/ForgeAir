using ForgeAir.Core.AudioEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Shared
{
    public class DSPShared
    {

        public bool isEnabled { get; set; } = true;
        public bool usingAM { get; set; } = false;
        public bool usingAMStereo { get; set; } = false;
        public bool usingFM { get; set; } = false;
        public BuiltInDSP dspEngine { get; set; }

        private static DSPShared? instance;
        public static DSPShared Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DSPShared();
                }
                return instance;
            }
        }
    }
}
