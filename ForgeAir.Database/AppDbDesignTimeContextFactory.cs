using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Database
{
    public class AppDbDesignTimeContextFactory : IDesignTimeDbContextFactory<ForgeAirDbContext>
    {
        private readonly static string connectionString = "Server=localhost;User Id=sa;Database=forgeair_dev;TrustServerCertificate=True;";
        public ForgeAirDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<ForgeAirDbContext> builder = new();

            DbContextOptions<ForgeAirDbContext> options = builder.UseSqlServer(connectionString).Options;

            return new ForgeAirDbContext(options);

        }
    }
}
