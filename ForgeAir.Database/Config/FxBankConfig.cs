using ForgeAir.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Database.Config
{
    public class FXBankConfig : IEntityTypeConfiguration<FxBank>
    {
        public void Configure(EntityTypeBuilder<FxBank> builder)
        {
            builder.HasKey(ta => new { ta.BankId, ta.FxId });

        }
    }
}
