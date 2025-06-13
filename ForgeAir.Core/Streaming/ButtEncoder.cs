using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.Shared;

namespace ForgeAir.Core.WebEncoder
{
    public class ButtEncoder
    {
        public void UpdateNowPlayingText()
        {
            if (!File.Exists(Shared.WebEncoderNowPlaying.Instance.buttExecutable))
            {
                return;
            }
            Process process = new Process();
            // Configure the process using the StartInfo properties.
            process.StartInfo.FileName = Shared.WebEncoderNowPlaying.Instance.buttExecutable;
            process.StartInfo.Arguments = $"-u \"{WebEncoderNowPlaying.Instance.nowPlayingText.Replace("\"", "\\\"")}\""; // Escape any internal quotes

            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            try
            {
                process.Start();
            }
            catch (Exception ex) {
                return;
            }
            return;
        }
    }
}
