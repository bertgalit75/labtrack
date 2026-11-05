using LabTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public sealed class LabTrackDesignTimeFactory
    : IDesignTimeDbContextFactory<LabTrackDbContext>
{
    public LabTrackDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<LabTrackDbContext>()
            .UseMySql(new MariaDbServerVersion(new Version(10, 11, 0)))
            .Options;

        return new LabTrackDbContext(options);
    }
}