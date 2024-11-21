using Microsoft.EntityFrameworkCore;
using ForgeAir.Database.Models;
using ForgeAir.Database.Config;

namespace ForgeAir.Database
{
    public class ForgeAirDbContext : DbContext
    {
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Station> Stations { get; set; }

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
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
      
        }
    }
}
