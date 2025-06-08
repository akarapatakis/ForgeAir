using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.Shared;

namespace ForgeAir.Core.WebEncoder
{
    public class TextOutputEncoder
    {
        public void UpdateNowPlayingText()
        {
            if (!File.Exists(Shared.WebEncoderNowPlaying.Instance.NowPlayingTXT))
            {
                if (Shared.WebEncoderNowPlaying.Instance.NowPlayingTXT == null || Shared.WebEncoderNowPlaying.Instance.NowPlayingTXT == "")
                {
                    return;
                }
                try
                {
                    System.IO.FileInfo file = new System.IO.FileInfo(Shared.WebEncoderNowPlaying.Instance.NowPlayingTXT);
                    file.Directory.Create();
                    File.Create(Shared.WebEncoderNowPlaying.Instance.NowPlayingTXT);
                }
                catch
                {
                    return ;
                }
            }

            try
            {
                File.WriteAllText(Shared.WebEncoderNowPlaying.Instance.NowPlayingTXT, WebEncoderNowPlaying.Instance.nowPlayingText);
            }
            catch (Exception ex)
            {
                return;
            }
            return;
        }
    }
}
