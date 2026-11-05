
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
    public async Task Patient_persistence_constraints_and_rollback_work()
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

        var uniqueSuffix = Guid.NewGuid().ToString("N");
        var username = $"mariadb-test-{uniqueSuffix}";
        var patientNumber = $"IT-{uniqueSuffix[..24]}";
        var dateOfBirth = new DateOnly(1990, 1, 1);

        await using var db = new LabTrackDbContext(options);

        await db.Database.OpenConnectionAsync();

        await using var transaction =
            await db.Database.BeginTransactionAsync();

        try
        {
            // Create a real Identity user for the required patient FK.
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

            // Insert a patient referencing the newly created user.
            var patient = new Patient(
                patientNumber,
                "MariaDB",
                "IntegrationTest",
                dateOfBirth,
                "NotSpecified",
                user.Id);

            db.Patients.Add(patient);
            await db.SaveChangesAsync();

            var patientId = patient.PatientId;

            // Verify actual database round-trip, not just tracked values.
            db.ChangeTracker.Clear();

            var persistedPatient = await db.Patients
                .AsNoTracking()
                .SingleAsync(p => p.PatientId == patientId);

            Assert.Equal(patientNumber, persistedPatient.PatientNumber);
            Assert.Equal("MariaDB", persistedPatient.FirstName);
            Assert.Equal("IntegrationTest", persistedPatient.LastName);
            Assert.Equal(user.Id, persistedPatient.RegisteredByUserId);
            Assert.Equal(dateOfBirth, persistedPatient.DateOfBirth);

            // Verify the unique patient-number constraint.
            var duplicatePatient = new Patient(
                patientNumber,
                "Duplicate",
                "Patient",
                null,
                null,
                user.Id);

            db.Patients.Add(duplicatePatient);

            var duplicateException =
                await Assert.ThrowsAsync<DbUpdateException>(
                    () => db.SaveChangesAsync());

            Assert.IsType<MySqlException>(
                duplicateException.InnerException);

            Assert.Equal(
                1062,
                ((MySqlException)duplicateException.InnerException!).Number);

            // Detach the failed insert before testing the FK constraint.
            db.ChangeTracker.Clear();

            var orphanPatient = new Patient(
                $"FK-{uniqueSuffix[..25]}",
                "Invalid",
                "ForeignKey",
                null,
                null,
                long.MaxValue);

            db.Patients.Add(orphanPatient);

            var foreignKeyException =
                await Assert.ThrowsAsync<DbUpdateException>(
                    () => db.SaveChangesAsync());

            Assert.IsType<MySqlException>(
                foreignKeyException.InnerException);

            Assert.Equal(
                1452,
                ((MySqlException)foreignKeyException.InnerException!).Number);
        }
        finally
        {
            // Remove all successful writes made by this test.
            await transaction.RollbackAsync();
        }

        await db.Database.CloseConnectionAsync();

        // Verify rollback from a separate database context.
        await using var verificationDb =
            new LabTrackDbContext(options);

        Assert.False(
            await verificationDb.Users
                .AnyAsync(u => u.UserName == username));

        Assert.False(
            await verificationDb.Patients
                .AnyAsync(p => p.PatientNumber == patientNumber));
    }
}
