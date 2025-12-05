using ForgeAir.Database.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Database.Config
{
    public class StationConfig : IEntityTypeConfiguration<Station>
    {
        public void Configure(EntityTypeBuilder<Station> builder)
        {
            builder.Property(s => s.Name).IsRequired(true);
            builder.Property(s => s.Telephone).IsRequired(false);
            builder.Property(s => s.Slogan).IsRequired(false);
            builder.Property(s => s.Website).IsRequired(false);
            builder.Property(s => s.Email).IsRequired(false);
            builder.Property(s => s.Genre).IsRequired(false);
        }
    }
}
//dotnet ef migrations add AddStationInformation
