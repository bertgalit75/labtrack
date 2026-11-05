using LabTrack.Domain.Entities;
using LabTrack.Infrastructure.Identity;
using LabTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Xunit;

namespace LabTrack.Infrastructure.Tests;

public sealed class MariaDbFactAttribute : FactAttribute
{
    public MariaDbFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(
                Environment.GetEnvironmentVariable(
                    "LABTRACK_MARIADB_CONNECTION_STRING")))
        {
            Skip = "LABTRACK_MARIADB_CONNECTION_STRING is not configured.";
        }
    }
}

public sealed class MariaDbPersistenceIntegrationTests
{
    [MariaDbFact]
    [Trait("Category", "MariaDB")]
    public async Task Patient_can_be_inserted_read_and_unique_number_is_enforced()
    {
        var connectionString =
            Environment.GetEnvironmentVariable(
                "LABTRACK_MARIADB_CONNECTION_STRING");

        Assert.False(string.IsNullOrWhiteSpace(connectionString));

        var connectionBuilder =
            new MySqlConnectionStringBuilder(connectionString);

        Assert.Equal(
            "labtrack_qualification",
            connectionBuilder.Database);

        var options = new DbContextOptionsBuilder<LabTrackDbContext>()
            .UseMySql(
                connectionString,
                new MariaDbServerVersion(new Version(10, 11, 6)))
            .Options;

        await using var db = new LabTrackDbContext(options);

        await db.Database.OpenConnectionAsync();

        await using var transaction =
            await db.Database.BeginTransactionAsync();

        try
        {
            var uniqueSuffix = Guid.NewGuid().ToString("N");
            var username = $"mariadb-test-{uniqueSuffix}";

            var user = new ApplicationUser
            {
                UserName = username,
                NormalizedUserName = username.ToUpperInvariant(),
                Email = $"{username}@example.invalid",
                NormalizedEmail =
                    $"{username}@example.invalid".ToUpperInvariant(),
                EmailConfirmed = false,
                IsActive = true
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            var patientNumber = $"IT-{uniqueSuffix[..24]}";

            var patient = new Patient(
                patientNumber,
                "MariaDB",
                "IntegrationTest",
                new DateOnly(1990, 1, 1),
                "NotSpecified",
                user.Id);

            db.Patients.Add(patient);
            await db.SaveChangesAsync();

            var patientId = patient.PatientId;

            db.ChangeTracker.Clear();

            var persistedPatient = await db.Patients
                .AsNoTracking()
                .SingleAsync(p => p.PatientId == patientId);

            Assert.Equal(patientNumber, persistedPatient.PatientNumber);
            Assert.Equal("MariaDB", persistedPatient.FirstName);
            Assert.Equal("IntegrationTest", persistedPatient.LastName);
            Assert.Equal(user.Id, persistedPatient.RegisteredByUserId);

            var duplicatePatient = new Patient(
                patientNumber,
                "Duplicate",
                "Patient",
                null,
                null,
                user.Id);

            db.Patients.Add(duplicatePatient);

            await Assert.ThrowsAsync<DbUpdateException>(
                () => db.SaveChangesAsync());
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }
}