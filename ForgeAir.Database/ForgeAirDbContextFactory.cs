using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace ForgeAir.Database
{
    public class ForgeAirDbContextFactory : IDbContextFactory<ForgeAirDbContext>
    {

        public ForgeAirDbContextFactory()
        {
        }

        public ForgeAirDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ForgeAirDbContext>();
#if DEBUG
            optionsBuilder.UseMySql(
                "Server=localhost;Port=3306;Database=forgeair_dev;User=root;Password=ForgeAir;",
                new MySqlServerVersion(new Version(9, 1, 0)));
#else
            
            optionsBuilder.UseMySql(
                $"Server=localhost;Port={Shared.DatabaseConnectionProperties.Instance.serverPort};Database={Shared.DatabaseConnectionProperties.Instance.dbName};User=root;Password={Shared.DatabaseConnectionProperties.Instance.password};",
                new MySqlServerVersion(new Version(9, 1, 0)));
#endif

            return new ForgeAirDbContext(optionsBuilder.Options);
        }
    }
}
