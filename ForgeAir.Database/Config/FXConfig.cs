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
    public class FXConfig : IEntityTypeConfiguration<FX>
    {
        public void Configure(EntityTypeBuilder<FX> builder)
        {

            builder.Property(t => t.DateDeleted).IsRequired(false);

            builder.Property(t => t.Color).IsRequired(true);

            builder.HasIndex(t => t.FilePath);



        }
    }
}
