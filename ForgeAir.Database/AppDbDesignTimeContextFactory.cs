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
#if (DEBUG)
        private readonly static string connectionString = "Server=localhost;Port=3307;Database=forgeair_dev;User=root;Password=ForgeAir;";
#else
        private readonly static string connectionString = $"Server=localhost;Port={Shared.DatabaseConnectionProperties.Instance.serverPort};Database={Shared.DatabaseConnectionProperties.Instance.dbName};User=root;Password={Shared.DatabaseConnectionProperties.Instance.password};";
#endif
        public ForgeAirDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<ForgeAirDbContext> builder = new();

            DbContextOptions<ForgeAirDbContext> options = builder.UseMySql(connectionString, new MySqlServerVersion(new Version(9, 1, 0))).Options;

            return new ForgeAirDbContext(options);

        }
    }
}
