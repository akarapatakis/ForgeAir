using ForgeAir.Database;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services
{
    public class ArtistsService
    {
        public async Task<bool> ArtistExists(string artistName) {


            DTO.Artist artist = new DTO.Artist();

            artist = await Task.Run(() => GetArtistByName(artistName));
            if (artist != null)
            {
                return true;
            }
            else { return false; }
            
        }
        public Task<DTO.Artist> GetArtistById(int id)
        {
            using (var context = new ForgeAirDbContext())
            {
                var sql = $"SELECT * FROM Artists WHERE Id={id}";
                return Task.FromResult(context.Database.SqlQueryRaw<DTO.Artist>(sql).FirstOrDefault());
            }
        }

        public Task<DTO.Artist> GetArtistByName(string name)
        {
            using (var context = new ForgeAirDbContext())
            {
                var sql = "SELECT * FROM Artists WHERE Name = @name";
                return Task.FromResult(context.Database.SqlQueryRaw<DTO.Artist>(sql, new MySqlParameter("@name", name)).FirstOrDefault());
            }
        }

        public int GetArtistCount()
        {
            using (var context = new ForgeAirDbContext())
            {
                var sql = "SELECT COUNT(*) AS Value FROM Artists";
                return context.Database.SqlQueryRaw<int>(sql).First();
            }
        }

        public async Task<List<DTO.Artist>> GetArtistNamesAsync()
        {
            using (var context = new ForgeAirDbContext())
            {
                var sql = "SELECT Id, Name FROM Artists";
                var list = Task.FromResult(context.Database.SqlQueryRaw<DTO.Artist>(sql).ToList());

                return list.Result;
            }
        }

        public async Task<List<DTO.Artist>> SearchArtistAsync(string name)
        {
            using (var context = new ForgeAirDbContext())
            {
                var sql = "SELECT Id, Name FROM Artists WHERE Name LIKE @name";
                var list = context.Database.SqlQueryRaw<DTO.Artist>(sql, new MySqlParameter("@name", $"%{name}%")).ToList();


                return list;
            }
        }

        public List<DTO.Artist> GetArtists()
        {
            using (var context = new ForgeAirDbContext())
            {
                var sql = "SELECT Id, Name FROM Artists";
                return context.Database.SqlQueryRaw<DTO.Artist>(sql).ToList();
            }
        }
    }
}
