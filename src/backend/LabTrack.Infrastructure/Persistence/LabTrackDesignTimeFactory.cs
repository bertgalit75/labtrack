using LabTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public sealed class LabTrackDesignTimeFactory
    : IDesignTimeDbContextFactory<LabTrackDbContext>
{
    public LabTrackDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("LABTRACK_MARIADB_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "LABTRACK_MARIADB_CONNECTION_STRING is not configured.");
        }

        var options = new DbContextOptionsBuilder<LabTrackDbContext>()
            .UseMySql(
                connectionString,
                new MariaDbServerVersion(new Version(10, 11, 6)))
            .Options;

        return new LabTrackDbContext(options);
    }
}