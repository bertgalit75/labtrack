namespace LabTrack.Domain.Entities;

public sealed class Patient
{
    public long PatientId { get; private set; }
    public string PatientNumber { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string? MiddleName { get; private set; }
    public string LastName { get; private set; } = string.Empty;
    public string? Suffix { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }
    public string? SexAtRegistration { get; private set; }
    public string? CivilStatus { get; private set; }
    public string? Nationality { get; private set; }
    public string? MobileNumber { get; private set; }
    public string? EmailAddress { get; private set; }
    public string? AddressLine1 { get; private set; }
    public string? AddressLine2 { get; private set; }
    public string? Barangay { get; private set; }
    public string? CityMunicipality { get; private set; }
    public string? Province { get; private set; }
    public string? PostalCode { get; private set; }
    public string PatientStatus { get; private set; } = "Active";
    public long? CanonicalPatientId { get; private set; }
    public DateTime RegisteredAtUtc { get; private set; }
    public long RegisteredByUserId { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public long RowVersion { get; private set; } = 1;

    private Patient() { }

    public Patient(string patientNumber, string firstName, string lastName, DateOnly? dateOfBirth,
        string? sexAtRegistration, long registeredByUserId)
    {
        if (string.IsNullOrWhiteSpace(patientNumber)) throw new ArgumentException("Patient number is required.", nameof(patientNumber));
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name is required.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name is required.", nameof(lastName));
        if (registeredByUserId <= 0) throw new ArgumentOutOfRangeException(nameof(registeredByUserId));

        PatientNumber = patientNumber.Trim();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        DateOfBirth = dateOfBirth;
        SexAtRegistration = Normalize(sexAtRegistration);
        RegisteredByUserId = registeredByUserId;
        RegisteredAtUtc = DateTime.UtcNow;
    }

    public void SetRegistrationDemographics(
        string? middleName, string? suffix, string? civilStatus, string? nationality,
        string? mobileNumber, string? emailAddress, string? addressLine1, string? addressLine2,
        string? barangay, string? cityMunicipality, string? province, string? postalCode)
    {
        MiddleName = Normalize(middleName);
        Suffix = Normalize(suffix);
        CivilStatus = Normalize(civilStatus);
        Nationality = Normalize(nationality);
        MobileNumber = Normalize(mobileNumber);
        EmailAddress = Normalize(emailAddress);
        AddressLine1 = Normalize(addressLine1);
        AddressLine2 = Normalize(addressLine2);
        Barangay = Normalize(barangay);
        CityMunicipality = Normalize(cityMunicipality);
        Province = Normalize(province);
        PostalCode = Normalize(postalCode);
    }

    public void UpdateRoutineDemographics(
        string? civilStatus, string? nationality, string? mobileNumber, string? emailAddress,
        string? addressLine1, string? addressLine2, string? barangay,
        string? cityMunicipality, string? province, string? postalCode, long updatedByUserId)
    {
        if (updatedByUserId <= 0) throw new ArgumentOutOfRangeException(nameof(updatedByUserId));
        CivilStatus = Normalize(civilStatus);
        Nationality = Normalize(nationality);
        MobileNumber = Normalize(mobileNumber);
        EmailAddress = Normalize(emailAddress);
        AddressLine1 = Normalize(addressLine1);
        AddressLine2 = Normalize(addressLine2);
        Barangay = Normalize(barangay);
        CityMunicipality = Normalize(cityMunicipality);
        Province = Normalize(province);
        PostalCode = Normalize(postalCode);
        UpdatedByUserId = updatedByUserId;
        UpdatedAtUtc = DateTime.UtcNow;
        RowVersion++;
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
