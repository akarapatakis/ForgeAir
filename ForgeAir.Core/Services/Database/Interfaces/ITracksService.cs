using ForgeAir.Core.DTO;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.Database.Interfaces
{
    public interface ITracksService
    {
        Task<List<TrackDTO>> GetByCategory(CategoryDTO category);
        Task<List<TrackDTO>> GetByArtist(ArtistDTO artist);
        Task<IEnumerable<TrackDTO>> GetTracks(int skip, int take);
        Task<List<TrackDTO>> GetByConditionAsync(ModelTypesEnum type, Expression<Func<Track, bool>> predicate);
        int GetTrackCount();
        Task<TrackDTO> GetTrack(int id);
    }
}
