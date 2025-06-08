using ForgeAir.Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.RDS.VariableCollections
{
    public class DynamicVariables
    {
        public string currentTime => DateTime.Now.ToString("HH:mm");
        public string currentSong => $"{RDSParams.Instance.currenttrackAnnouncement} {RDSParams.Instance.currenttrackArtistTitleDivider} {AudioPlayerShared.Instance.currentTrack.Title}";
        public string currentDate => DateTime.Now.ToString("d");

        private static DynamicVariables? instance;
        public static DynamicVariables Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DynamicVariables();
                }
                return instance;
            }
        }
    }
}
