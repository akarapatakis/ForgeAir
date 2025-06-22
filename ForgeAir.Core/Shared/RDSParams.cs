using ForgePlugin;
using ForgePlugin.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
namespace ForgeAir.Core.Shared
{
    public class RDSParams
    {
        public IRDSDevice rdsEncoder { get; set; }

        public string currentPS { get; set; }
        public string currentRT { get; set; }
        public string chunkedPS { get; set; }
        public string chunkedRT { get; set; }

        public string[]? psBlocks { get; set; }
        public string[]? rtBlocks { get; set; }

        public int psHoldInterval { get; set; }
        public int rtHoldInterval { get; set; }

        public System.Timers.Timer psHoldTimer;
        public System.Timers.Timer rtHoldTimer;

        public string currenttrackAnnouncement { get; set; } = "On Air: ";
        public string currenttrackArtistTitleDivider { get; set; } = " - ";

        public event EventHandler? psChanged;
        public event EventHandler? rtChanged;
        public event EventHandler? updatedCurrentTrack;

        public void RaisePSChanged() { psChanged?.Invoke(this, EventArgs.Empty); }
        public void RaiseRTChanged() { rtChanged?.Invoke(this, EventArgs.Empty); }

        private static RDSParams? instance;
        public static RDSParams Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RDSParams();
                }
                return instance;
            }
        }
    }
}
