using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgeAir.Database.Config
{
        public class PlaylistToWatchConfig : IEntityTypeConfiguration<PlaylistToWatch>
        {
        public void Configure(EntityTypeBuilder<PlaylistToWatch> builder)
        {

            builder.Property(t => t.DisplayName);

            builder
                .HasMany(p => p.DesiredCategories)
                .WithMany()
                .UsingEntity(j => j.ToTable("PlaylistToWatchCategories"));

            builder.Property(t => t.isWatching).IsRequired();

            builder.HasIndex(t => t.FilePath);
        }

    }
}
