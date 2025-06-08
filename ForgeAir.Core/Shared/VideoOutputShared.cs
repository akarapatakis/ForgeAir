using Microsoft.EntityFrameworkCore.Storage.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Shared
{
    public class VideoOutputShared
    {
        public VideoOutputShared() { 
        
        
        }

        public bool Enabled  { get; set; } = false;
        public bool stretchFourToThree { get; set; } = false;

        public bool useOverlay { get; set; } = true;
        public bool useLogo { get; set; } = false;
        public bool showClock { get; set; } = false;
        
        public double videoResHeight { get; set; } = 720;
        public double videoResWidth { get; set; } = 1280;
        
        public string overlayPath { get; set; } = "overlays/basic/";
        public string logoPath { get; set; } = "";
        

        public event EventHandler? updateVideoTexts;

        public void RaiseOnTrackChanged()
        {

            if (updateVideoTexts != null)
            {
                updateVideoTexts.DynamicInvoke(this, EventArgs.Empty);

            }
        }

        private static VideoOutputShared? instance;
        public static VideoOutputShared Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new VideoOutputShared();
                }
                return instance;
            }
        }
    }
}
