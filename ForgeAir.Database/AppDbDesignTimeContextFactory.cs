using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Database
{
#if (DEBUG)

    public class AppDbDesignTimeContextFactory : IDesignTimeDbContextFactory<ForgeAirDbContext>
    {
        private readonly static string connectionString = "Server=localhost;Port=3306;Database=forgeair_dev;User=root;Password=12345678;";

    public ForgeAirDbContext CreateDbContext(string[] args)
        {

            DbContextOptionsBuilder<ForgeAirDbContext> builder = new();

            DbContextOptions<ForgeAirDbContext> options = builder.UseMySql(connectionString, new MySqlServerVersion(new Version(9, 1, 0))).Options;

            return new ForgeAirDbContext(options);

        }
    }
#endif
}
