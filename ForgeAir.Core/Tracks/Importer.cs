using ForgeAir.Core.DTO;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using m3uParser;
namespace ForgeAir.Core.Tracks
{
    public class Importer
    {
        public void AddCategory(CategoryDTO category) {
            if (category == null) { return; }
            using (var context = new ForgeAirDbContext())
            {

                context.Category.Add(CategoryDTO.ToEntity(category));
                context.SaveChanges();
                context.ChangeTracker.Clear();
                return;
            }
        }

        public void AddFx(FxDTO fx) {
            if (fx == null) { return; }

            using (var context = new ForgeAirDbContext())
            {
                context.Fx.Add(FxDTO.ToEntity(fx));
                context.SaveChanges();
                context.ChangeTracker.Clear();
                return;
            }
        }
        public static ICollection<TrackDTO> M3UToTracks(string m3uFile) {
            ICollection<TrackDTO> tracks = new List<TrackDTO>();
            var listTracks = m3uParser.M3U.ParseFromFile(m3uFile);
            foreach (var track in listTracks.Medias)
            {
                tracks.Add(new TrackDTO { FilePath = track.MediaFile});
            }
            return tracks;

        }
        public void AddFX(FxDTO fx) {

            using (var context = new ForgeAirDbContext())
            {
                context.Fx.Add(FxDTO.ToEntity(fx));
                context.SaveChanges();
                context.ChangeTracker.Clear();
                return;
            }
        }
    }
}
