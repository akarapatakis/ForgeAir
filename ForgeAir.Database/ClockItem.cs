using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Database.Models;

namespace ForgeAir.Database
{
    public class ClockItem
    {
        public DateTime startHour { get; set; }
        public DateTime endHour { get; set; }

        public bool useML { get; set; }

        public int minBpm { get; set; }
        public int maxBpm { get; set; }

        public List<Artist> artistsRules { get; set; }
        public List<Track> generatedTrackList {  get; set; }

        public List<Category> categoriesRules { get; set; } 

        public int jinglesBetweenTracks {  get; set; }
        public int adsBetweenTracks { get; set; }
    }
}
