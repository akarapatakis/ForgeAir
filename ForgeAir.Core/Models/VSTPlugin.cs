using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedBass.Vst;

namespace ForgeAir.Core.Models
{
    /// <summary>
    /// VST Plugin Model
    /// </summary>
    public class VSTPlugin
    {
        /// <summary>
        /// Get/Set the state of the plugin in ForgeAir
        /// </summary>
        public required bool Enabled { get; set; }

        /// <summary>
        /// Get/Set the path of the plugin
        /// </summary>
        public required string Path { get; set; }

    }
}
