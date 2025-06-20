using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.AudioPlayout.DSP.VST.Interfaces
{
    public interface IVSTService
    {


        /// <summary>
        /// (pew pew)
        /// Kills and unhooks the loaded effect (the same effect as doing Dispose() but it is needed to run upon initalization)
        /// </summary>
        public void Kill();
    }
}
