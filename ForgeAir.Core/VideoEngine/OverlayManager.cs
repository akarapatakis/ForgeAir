using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
namespace ForgeAir.Core.VideoEngine
{
    public class OverlayManager
    {
        public Overlay getOverlay(string jsonFile)
        {
            using (StreamReader reader = new StreamReader(jsonFile))
            {
                JsonReader jsonReader = new JsonTextReader(reader);

                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();

                OverlayCollection overlays = serializer.Deserialize<OverlayCollection>(jsonReader);

                return overlays.Overlay.FirstOrDefault();
            }
        }
    }
}
