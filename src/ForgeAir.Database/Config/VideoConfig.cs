using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Database.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ForgeAir.Database.Config
{
    public class VideoConfig : IEntityTypeConfiguration<Video>
    {
        public void Configure(EntityTypeBuilder<Video> builder)
        {
            builder.HasKey(v => v.Id);

            builder.HasIndex(v => v.FilePath).IsUnique();

            builder.Property(v => v.Name)
                   .IsRequired()
                   .HasMaxLength(400);

            builder.Property(v => v.Duration).HasColumnType("time(3)");
            builder.Property(v => v.StartPoint).HasColumnType("time(3)");
            builder.Property(v => v.EndPoint).HasColumnType("time(3)");
            builder.Property(v => v.AdBreaks).HasColumnType("time(3)");
            builder.Property(v => v.AdBreakLength).HasColumnType("time(3)");

            builder.Property(v => v.CustomAttributesJson)
                   .HasColumnName("CustomAttributesJson")
                   .HasColumnType("longtext");

            builder.Ignore(v => v.customAttributes);
        }
    }

}
