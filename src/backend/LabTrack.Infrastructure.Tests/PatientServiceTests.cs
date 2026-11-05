using LabTrack.Application.Patients;
using LabTrack.Domain.Entities;
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

        await GrantPatientAccessAsync(db, 7, 3);
        var page = await service.SearchAsync("Ana", "Active", 1, 1, "lastName", 7, CancellationToken.None);

        Assert.Single(page.Items);
        Assert.Equal("Ana", page.Items[0].FirstName);
        Assert.Equal(1L, page.TotalItems);
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
        await GrantPatientAccessAsync(db, 8, 3, id);

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
                8, 1, CancellationToken.None));
    }

    [Fact]
    public async Task SearchAndDetail_RequireMatchingActiveDepartmentMembership()
    {
        await using var db = CreateContext();
        var service = new PatientService(db);
        var created = await service.RegisterAsync(new RegisterPatientRequest { FirstName = "Lia", LastName = "Cruz" }, 7, CancellationToken.None);
        var patientId = long.Parse(created.Id);

        await GrantPatientAccessAsync(db, 7, 3, patientId, membershipDepartmentId: 4);
        var blocked = await service.SearchAsync(null, null, 1, 25, null, 7, CancellationToken.None);
        Assert.Empty(blocked.Items);
        Assert.Null(await service.GetAsync(patientId, 7, CancellationToken.None));

        db.UserDepartmentMemberships.Add(new UserDepartmentMembership
        {
            UserId = 7, DepartmentId = 3, IsActive = true, EffectiveFromUtc = DateTime.UtcNow.AddMinutes(-1)
        });
        await db.SaveChangesAsync();

        var allowed = await service.SearchAsync(null, null, 1, 25, null, 7, CancellationToken.None);
        Assert.Single(allowed.Items);
        Assert.NotNull(await service.GetAsync(patientId, 7, CancellationToken.None));
    }

    private static async Task GrantPatientAccessAsync(LabTrackDbContext db, long userId, long departmentId, long? patientId = null, long? membershipDepartmentId = null)
    {
        var patient = patientId is null
            ? await db.Patients.OrderBy(p => p.PatientId).FirstAsync()
            : await db.Patients.SingleAsync(p => p.PatientId == patientId.Value);
        var examination = new Examination("EX-" + Guid.NewGuid().ToString("N")[..12], patient.PatientId, DateOnly.FromDateTime(DateTime.UtcNow), 7);
        db.Examinations.Add(examination);
        await db.SaveChangesAsync();
        var membershipDepartmentId = departmentId;
        if (db.Departments.All(d => d.DepartmentId != departmentId))
            db.Departments.Add(new Department { DepartmentId = departmentId, DepartmentCode = "D" + departmentId, DepartmentName = "Department " + departmentId, IsActive = true, CreatedAtUtc = DateTime.UtcNow });
        if (membershipDepartmentId is long otherDepartmentId && db.Departments.All(d => d.DepartmentId != otherDepartmentId))
            db.Departments.Add(new Department { DepartmentId = otherDepartmentId, DepartmentCode = "D" + otherDepartmentId, DepartmentName = "Department " + otherDepartmentId, IsActive = true, CreatedAtUtc = DateTime.UtcNow });
        await db.SaveChangesAsync();
        var now = DateTime.UtcNow.AddMinutes(-1);
        db.PatientExaminationAssignments.Add(new PatientExaminationAssignment
        {
            ExaminationId = examination.ExaminationId, PatientId = patient.PatientId, UserId = userId,
            DepartmentId = departmentId, IsActive = true, EffectiveFromUtc = now, AssignedByUserId = 7, CreatedAtUtc = now
        });
        db.UserDepartmentMemberships.Add(new UserDepartmentMembership
        {
            UserId = userId, DepartmentId = membershipDepartmentId ?? departmentId, IsActive = true, EffectiveFromUtc = now
        });
        await db.SaveChangesAsync();
    }

}
