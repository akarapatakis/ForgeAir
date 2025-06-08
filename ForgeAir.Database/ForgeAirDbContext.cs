using Microsoft.EntityFrameworkCore;
using ForgeAir.Database.Models;
using ForgeAir.Database.Config;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace ForgeAir.Database
{
    public class ForgeAirDbContext : DbContext
    {
        public DbSet<Track> Tracks { get; set; }
        public DbSet<ArtistTrack> ArtistTracks { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Category> Category { get; set; }

        public DbSet<FX> Fx { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Video> Videos { get; set; }
        public ForgeAirDbContext()
        {
        }

        public ForgeAirDbContext(DbContextOptions<ForgeAirDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfiguration(new ArtistTrackConfig());
            modelBuilder.ApplyConfiguration(new StationConfig());
            modelBuilder.ApplyConfiguration(new TrackConfig());
            modelBuilder.ApplyConfiguration(new CategoryConfig());
            modelBuilder.ApplyConfiguration(new VideoConfig());


            modelBuilder.Entity<Track>().ToTable("Tracks");
            modelBuilder.Entity<Artist>().ToTable("Artists");
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<ArtistTrack>().ToTable("Artists_Tracks");
            modelBuilder.Entity<Video>().ToTable("Videos");
            modelBuilder.Entity<FX>().ToTable("FX");
            modelBuilder.Entity<Station>().ToTable("Stations");
            modelBuilder.Entity<User>().ToTable("Users");


        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
#if DEBUG
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseMySql(
                "Server=localhost;Port=3306;Database=forgeair_dev;User=root;Password=ForgeAir;",
                new MySqlServerVersion(new Version(9, 1, 0)));
#else
            
            optionsBuilder.UseMySql(
                $"Server=localhost;Port={Shared.DatabaseConnectionProperties.Instance.serverPort};Database={Shared.DatabaseConnectionProperties.Instance.dbName};User=root;Password={Shared.DatabaseConnectionProperties.Instance.password};",
                new MySqlServerVersion(new Version(9, 1, 0)));
#endif

        }

    }
}
