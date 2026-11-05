using LabTrack.Application.Patients;
using LabTrack.Domain.Entities;
using LabTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LabTrack.Infrastructure.Patients;

public sealed class PatientService(LabTrackDbContext db) : IPatientService
{
    public async Task<PagedResult<PatientListItemDto>> SearchAsync(
        string? search, string? status, int page, int pageSize, string? sort, long actorUserId, CancellationToken cancellationToken)
    {
        var query = ApplyPatientScope(db.Patients.AsNoTracking(), actorUserId);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p => p.PatientNumber.Contains(term) ||
                p.FirstName.Contains(term) || (p.MiddleName != null && p.MiddleName.Contains(term)) ||
                p.LastName.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(p => p.PatientStatus == status.Trim());

        query = sort?.Trim().ToLowerInvariant() switch
        {
            "patientnumber" => query.OrderBy(p => p.PatientNumber),
            "registeredat" => query.OrderByDescending(p => p.RegisteredAtUtc),
            "firstname" => query.OrderBy(p => p.FirstName).ThenBy(p => p.LastName),
            _ => query.OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
        };

        var total = await query.LongCountAsync(cancellationToken);
        var rows = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        var items = rows.Select(p => new PatientListItemDto(
            p.PatientId.ToString(), p.PatientNumber, p.FirstName, p.MiddleName, p.LastName,
            p.PatientStatus, ETag(p.RowVersion))).ToArray();
        return new PagedResult<PatientListItemDto>(items, page, pageSize, total,
            total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize));
    }

    public async Task<PatientDto?> GetAsync(long patientId, long actorUserId, CancellationToken cancellationToken)
    {
        var patient = await ApplyPatientScope(db.Patients.AsNoTracking(), actorUserId).SingleOrDefaultAsync(p => p.PatientId == patientId, cancellationToken);
        return patient is null ? null : Map(patient);
    }

    public async Task<PatientDto> RegisterAsync(RegisterPatientRequest request, long actorUserId, CancellationToken cancellationToken)
    {
        var patientNumber = "PT-" + Guid.NewGuid().ToString("N")[..26].ToUpperInvariant();
        var patient = new Patient(patientNumber, request.FirstName, request.LastName,
            request.DateOfBirth, request.SexAtRegistration, actorUserId);
        patient.SetRegistrationDemographics(request.MiddleName, request.Suffix, request.CivilStatus,
            request.Nationality, request.MobileNumber, request.EmailAddress, request.AddressLine1,
            request.AddressLine2, request.Barangay, request.CityMunicipality, request.Province, request.PostalCode);

        await using var transaction = db.Database.IsRelational()
            ? await db.Database.BeginTransactionAsync(cancellationToken)
            : null;
        db.Patients.Add(patient);
        await db.SaveChangesAsync(cancellationToken);
        db.AuditLogs.Add(AuditLog.Create(actorUserId, "patients.register", "Patient", patient.PatientId, "Patient registered"));
        await db.SaveChangesAsync(cancellationToken);
        if (transaction is not null) await transaction.CommitAsync(cancellationToken);
        return Map(patient);
    }

    public async Task<PatientDto?> UpdateDemographicsAsync(
        long patientId, UpdatePatientDemographicsRequest request, long actorUserId,
        long expectedRowVersion, CancellationToken cancellationToken)
    {
        var patient = await ApplyPatientScope(db.Patients, actorUserId).SingleOrDefaultAsync(p => p.PatientId == patientId, cancellationToken);
        if (patient is null) return null;
        if (patient.RowVersion != expectedRowVersion)
            throw new DbUpdateConcurrencyException("The patient record has changed.");

        await using var transaction = db.Database.IsRelational()
            ? await db.Database.BeginTransactionAsync(cancellationToken)
            : null;
        patient.UpdateRoutineDemographics(
            request.CivilStatus ?? patient.CivilStatus,
            request.Nationality ?? patient.Nationality,
            request.MobileNumber ?? patient.MobileNumber,
            request.EmailAddress ?? patient.EmailAddress,
            request.AddressLine1 ?? patient.AddressLine1,
            request.AddressLine2 ?? patient.AddressLine2,
            request.Barangay ?? patient.Barangay,
            request.CityMunicipality ?? patient.CityMunicipality,
            request.Province ?? patient.Province,
            request.PostalCode ?? patient.PostalCode,
            actorUserId);
        await db.SaveChangesAsync(cancellationToken);
        db.AuditLogs.Add(AuditLog.Create(actorUserId, "patients.demographics.update", "Patient", patient.PatientId,
            "Routine patient demographics updated"));
        await db.SaveChangesAsync(cancellationToken);
        if (transaction is not null) await transaction.CommitAsync(cancellationToken);
        return Map(patient);
    }


    private IQueryable<Patient> ApplyPatientScope(IQueryable<Patient> patients, long actorUserId)
    {
        var now = DateTime.UtcNow;
        return patients.Where(p => db.PatientExaminationAssignments.Any(a =>
            a.PatientId == p.PatientId && a.UserId == actorUserId && a.IsActive &&
            a.EffectiveFromUtc <= now && (a.EffectiveToUtc == null || a.EffectiveToUtc > now) &&
            db.Examinations.Any(e => e.ExaminationId == a.ExaminationId && e.PatientId == p.PatientId) &&
            db.UserDepartmentMemberships.Any(m => m.UserId == actorUserId && m.DepartmentId == a.DepartmentId &&
                m.IsActive && m.EffectiveFromUtc <= now && (m.EffectiveToUtc == null || m.EffectiveToUtc > now))));
    }

    private static string ETag(long rowVersion) => $"\"{rowVersion}\"";

    private static PatientDto Map(Patient p) => new(
        p.PatientId.ToString(), p.PatientNumber, p.FirstName, p.MiddleName, p.LastName, p.Suffix,
        p.DateOfBirth, p.SexAtRegistration, p.CivilStatus, p.Nationality, p.MobileNumber, p.EmailAddress,
        p.AddressLine1, p.AddressLine2, p.Barangay, p.CityMunicipality, p.Province, p.PostalCode,
        p.PatientStatus, p.CanonicalPatientId?.ToString(), p.RegisteredAtUtc,
        p.UpdatedAtUtc?.ToString("O"), ETag(p.RowVersion));
}
