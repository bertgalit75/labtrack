using LabTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public sealed class LabTrackDesignTimeFactory
    : IDesignTimeDbContextFactory<LabTrackDbContext>
{
    // This reserved .invalid host lets EF inspect the model and migrations without
    // requiring database credentials. Any design-time command that actually opens
    // a connection must be run with LABTRACK_MARIADB_CONNECTION_STRING configured.
    private const string DesignTimeConnectionString =
        "Server=labtrack-design-time.invalid;Port=3306;Database=LabTrackDesignTime;User=design_time;Password=design_time;";

    public LabTrackDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("LABTRACK_MARIADB_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = DesignTimeConnectionString;
        }

        var options = new DbContextOptionsBuilder<LabTrackDbContext>()
            .UseMySql(
                connectionString,
                new MariaDbServerVersion(new Version(10, 11, 6)))
            .Options;

        return new LabTrackDbContext(options);
    }
}
