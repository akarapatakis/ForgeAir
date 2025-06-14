using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.InstallationWizard
{
    public static class Utils
    {
        public static async Task DownloadFileTaskAsync(this HttpClient client, Uri uri, string FileName)
        {
            if (File.Exists(FileName))
            {
                return;
            }
            using (var s = await client.GetStreamAsync(uri))
            {
                using (var fs = new FileStream(FileName, FileMode.CreateNew))
                {
                    await s.CopyToAsync(fs);
                }
            }
        }
    }
}
