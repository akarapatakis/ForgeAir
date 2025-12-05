using ForgeAir.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.Database.Interfaces
{
    public interface ISearchService
    {
        Task<List<Artist>> SearchArtists(string text);
        Task<List<Track>> SearchTracks(string text);
    }
}
