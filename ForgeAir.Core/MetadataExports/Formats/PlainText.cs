using ForgeAir.Core.DTO;
using ForgeAir.Core.MetadataExports.Enums;
using ForgeAir.Core.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.MetadataExports.Formats
{
    public class PlainText : IFormat
    {
        private readonly StationPaths _paths;

        public PlainText(StationPaths paths) { 
       
            _paths = paths;
        }

        public void WriteNowPlaying(NowPlayingModel nP)
        {
            string path = _paths.ExportsPath + "\\now_playing.txt";

            if (!File.Exists(path))
            {
                if (path == null || path == "")
                {
                    return;
                }
                try
                {
                    System.IO.FileInfo file = new System.IO.FileInfo(path);
                    file.Directory.Create();
                    File.Create(path);
                }
                catch
                {
                    return;
                }
            }

            try
            {
                File.WriteAllText(path, BuildStringOutput(nP));
            }
            catch (Exception ex)
            {
                return;
            }
            return;
        }

        private string BuildStringOutput(NowPlayingModel nP)
        {
            return $"{nP.StringPrefix}{nP.CurrentTrack.DisplayArtists} - {nP.CurrentTrack.Title}{nP.StringSuffix}";
        }
    }
}
