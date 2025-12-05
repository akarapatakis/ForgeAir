using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Database.Models;

namespace ForgeAir.Core.DTO
{
    public class ArtistDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }


        public static ArtistDTO FromEntity(Artist artist)
        {
            return new ArtistDTO { Id = artist.Id, Name = artist.Name };
        }

        public static Artist ToEntity(ArtistDTO dto)
        {
            return new Artist { Id = dto.Id, Name = dto.Name };
        }
    }
}
