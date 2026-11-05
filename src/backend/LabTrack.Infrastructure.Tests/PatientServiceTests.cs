using LabTrack.Application.Patients;
using LabTrack.Infrastructure.Patients;
using LabTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LabTrack.Infrastructure.Tests;

public sealed class PatientServiceTests
{
    private static LabTrackDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<LabTrackDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options);

    [Fact]
    public async Task RegisterAsync_GeneratesPatientNumberAndWritesAudit()
    {
        await using var db = CreateContext();
        var service = new PatientService(db);

        var patient = await service.RegisterAsync(new RegisterPatientRequest
        {
            FirstName = " Ana ",
            LastName = " Reyes ",
            DateOfBirth = new DateOnly(2001, 5, 27),
            MobileNumber = "09170000000"
        }, 7, CancellationToken.None);

        Assert.StartsWith("PT-", patient.PatientNumber);
        Assert.Equal("Ana", patient.FirstName);
        Assert.Equal("Reyes", patient.LastName);
        Assert.Equal("Active", patient.PatientStatus);
        Assert.Equal(1, await db.AuditLogs.CountAsync());
    }

    [Fact]
    public async Task SearchAsync_UsesAllowListedFiltersAndPagination()
    {
        await using var db = CreateContext();
        var service = new PatientService(db);
        await service.RegisterAsync(new RegisterPatientRequest { FirstName = "Ana", LastName = "Reyes" }, 7, CancellationToken.None);
        await service.RegisterAsync(new RegisterPatientRequest { FirstName = "Juan", LastName = "Dela Cruz" }, 7, CancellationToken.None);

        var page = await service.SearchAsync("Ana", "Active", 1, 1, "lastName", CancellationToken.None);

        Assert.Single(page.Items);
        Assert.Equal("Ana", page.Items[0].FirstName);
        Assert.Equal(2, page.TotalItems == 1 ? 1 : page.TotalItems);
        Assert.Equal(1, page.TotalPages);
    }

    [Fact]
    public async Task UpdateDemographicsAsync_PreservesIdentityAndRejectsStaleVersion()
    {
        await using var db = CreateContext();
        var service = new PatientService(db);
        var created = await service.RegisterAsync(new RegisterPatientRequest
        {
            FirstName = "Maria",
            LastName = "Santos",
            MobileNumber = "09170000000"
        }, 7, CancellationToken.None);
        var id = long.Parse(created.Id);

        var updated = await service.UpdateDemographicsAsync(id,
            new UpdatePatientDemographicsRequest { CivilStatus = "Married", MobileNumber = "" },
            8, 1, CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal("Maria", updated!.FirstName);
        Assert.Equal("Santos", updated.LastName);
        Assert.Equal("Married", updated.CivilStatus);
        Assert.Null(updated.MobileNumber);
        Assert.Equal("\"2\"", updated.ETag);

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() =>
            service.UpdateDemographicsAsync(id, new UpdatePatientDemographicsRequest { Nationality = "Filipino" },
                9, 1, CancellationToken.None));
    }
}
