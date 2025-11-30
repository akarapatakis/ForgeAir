using ForgeAir.Database;
using Microsoft.EntityFrameworkCore;

public class StationManager
{
    private readonly IDbContextFactory<ForgeAirDbContext> _dbContextFactory;

    public StationManager(IDbContextFactory<ForgeAirDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task Update(
        int stationId,
        string name,
        string slogan,
        string email,
        string website,
        string telephone,
        string logoFilePath,
        string genre,
        ushort pi
        )
    {
        using var _context = _dbContextFactory.CreateDbContext();
        var station = await _context.Stations.FirstOrDefaultAsync(s => s.Id == stationId);

        if (station != null)
        {
            station.Name = name;
            station.Slogan = slogan;
            station.Email = email;
            station.Website = website;
            station.Telephone = telephone;
            station.LogoFilePath = logoFilePath;
            station.Genre = genre;
            station.RdsPI = pi;

            await _context.SaveChangesAsync();
        }
    }
}
