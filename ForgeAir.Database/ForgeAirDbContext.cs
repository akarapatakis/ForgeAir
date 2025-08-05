using Microsoft.EntityFrameworkCore;
using ForgeAir.Database.Models;
using ForgeAir.Database.Config;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using ForgeAir.Database.Repositories;

namespace ForgeAir.Database
{
    public class ForgeAirDbContext : DbContext
    {
        public DbSet<Track> Tracks { get; set; }
        public DbSet<ArtistTrack> ArtistTracks { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<PlaylistToWatch> PlaylistsToWatch { get; set; }
        public DbSet<FX> Fx { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Video> Videos { get; set; }


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
            modelBuilder.ApplyConfiguration(new PlaylistToWatchConfig());

            modelBuilder.Entity<Track>().ToTable("Tracks");
            modelBuilder.Entity<Artist>().ToTable("Artists");
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<ArtistTrack>().ToTable("Artists_Tracks");
            modelBuilder.Entity<PlaylistToWatch>().ToTable("PlaylistsToWatch");
            modelBuilder.Entity<Video>().ToTable("Videos");
            modelBuilder.Entity<FX>().ToTable("FX");
            modelBuilder.Entity<Station>().ToTable("Stations");
            modelBuilder.Entity<User>().ToTable("Users");


        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

#if DEBUG
            optionsBuilder.EnableSensitiveDataLogging();
#endif

        }

    }
}
