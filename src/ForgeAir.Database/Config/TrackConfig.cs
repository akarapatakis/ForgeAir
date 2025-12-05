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
    public class TrackConfig : IEntityTypeConfiguration<Track>
    {
        public void Configure(EntityTypeBuilder<Track> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.DateDeleted).IsRequired(false);
            builder.Property(t => t.Album).IsRequired(false);

            builder.Property(t => t.ISRC).IsRequired(false);

            builder.HasIndex(t => t.FilePath);
            builder.HasMany(t => t.Categories)
                .WithMany(c => c.Tracks)
                .UsingEntity<Dictionary<string, object>>(
                "Categories_Tracks",
                j => j
                    .HasOne<Category>()
                    .WithMany()
                    .HasForeignKey("CategoryId")
                    .HasConstraintName("FK_TrackCategories_CategoryId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<Track>()
                    .WithMany()
                    .HasForeignKey("TrackId")
                    .HasConstraintName("FK_TrackCategories_TrackId")
                    .OnDelete(DeleteBehavior.Cascade)
                    );

            //builder.HasMany(t => t.TrackTags)
            //    .WithOne(tt => tt.Track)
            //    .HasForeignKey(tt => tt.TrackId)
            //    .OnDelete(DeleteBehavior.Cascade);




        }
    }
}
