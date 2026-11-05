using System.ComponentModel.DataAnnotations;

namespace LabTrack.Application.Patients;

public sealed record PatientDto(
    string Id, string PatientNumber, string FirstName, string? MiddleName, string LastName, string? Suffix,
    DateOnly? DateOfBirth, string? SexAtRegistration, string? CivilStatus, string? Nationality,
    string? MobileNumber, string? EmailAddress, string? AddressLine1, string? AddressLine2,
    string? Barangay, string? CityMunicipality, string? Province, string? PostalCode,
    string PatientStatus, string? CanonicalPatientId, DateTime RegisteredAtUtc,
    string? UpdatedAtUtc, string ETag);

public sealed record PatientRegistrationReceipt(string Id, string PatientNumber, string ETag);

public sealed record PatientReadSummaryDto(string Id, string PatientNumber, string FirstName, string? MiddleName, string LastName, string PatientStatus, string ETag);

public sealed record PatientListItemDto(
    string Id, string PatientNumber, string FirstName, string? MiddleName, string LastName,
    string PatientStatus, string ETag);

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, long TotalItems, int TotalPages);

public sealed class RegisterPatientRequest
{
    [Required, StringLength(100)] public string FirstName { get; init; } = string.Empty;
    [StringLength(100)] public string? MiddleName { get; init; }
    [Required, StringLength(100)] public string LastName { get; init; } = string.Empty;
    [StringLength(20)] public string? Suffix { get; init; }
    public DateOnly? DateOfBirth { get; init; }
    [StringLength(30)] public string? SexAtRegistration { get; init; }
    [StringLength(30)] public string? CivilStatus { get; init; }
    [StringLength(80)] public string? Nationality { get; init; }
    [StringLength(30)] public string? MobileNumber { get; init; }
    [EmailAddress, StringLength(254)] public string? EmailAddress { get; init; }
    [StringLength(180)] public string? AddressLine1 { get; init; }
    [StringLength(180)] public string? AddressLine2 { get; init; }
    [StringLength(100)] public string? Barangay { get; init; }
    [StringLength(100)] public string? CityMunicipality { get; init; }
    [StringLength(100)] public string? Province { get; init; }
    [StringLength(20)] public string? PostalCode { get; init; }
}

// Null means "leave unchanged"; an empty string explicitly clears an optional field.
public sealed class UpdatePatientDemographicsRequest
{
    [StringLength(30)] public string? CivilStatus { get; init; }
    [StringLength(80)] public string? Nationality { get; init; }
    [StringLength(30)] public string? MobileNumber { get; init; }
    [EmailAddress, StringLength(254)] public string? EmailAddress { get; init; }
    [StringLength(180)] public string? AddressLine1 { get; init; }
    [StringLength(180)] public string? AddressLine2 { get; init; }
    [StringLength(100)] public string? Barangay { get; init; }
    [StringLength(100)] public string? CityMunicipality { get; init; }
    [StringLength(100)] public string? Province { get; init; }
    [StringLength(20)] public string? PostalCode { get; init; }
}

public interface IPatientService
{
    Task<PagedResult<PatientListItemDto>> SearchAsync(string? search, string? status, int page, int pageSize, string? sort, long actorUserId, CancellationToken cancellationToken);
    Task<PatientDto?> GetAsync(long patientId, long actorUserId, CancellationToken cancellationToken);
    Task<PatientDto> RegisterAsync(RegisterPatientRequest request, long actorUserId, CancellationToken cancellationToken);
    Task<PatientDto?> UpdateDemographicsAsync(long patientId, UpdatePatientDemographicsRequest request, long actorUserId, long expectedRowVersion, CancellationToken cancellationToken);
}
