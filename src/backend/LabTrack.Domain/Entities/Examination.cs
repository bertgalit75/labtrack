namespace LabTrack.Domain.Entities;

public sealed class Examination
{
    public long ExaminationId { get; private set; }
    public string ExaminationNumber { get; private set; } = string.Empty;
    public long PatientId { get; private set; }
    public DateOnly ExaminationDate { get; private set; }
    public string Priority { get; private set; } = "Routine";
    public string Status { get; private set; } = "Registered";
    public string? Notes { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public long CreatedByUserId { get; private set; }
    public long RowVersion { get; private set; } = 1;

    // EF Core uses this constructor when materializing persisted examinations.
    private Examination() { }

    /// <summary>
    /// Creates a registered examination. The caller must obtain the examination
    /// number from the transaction-safe numbering service before constructing it.
    /// Patient activity/canonical status and actor authorization are enforced by
    /// the application workflow, not inferred by this entity.
    /// </summary>
    public Examination(
        string examinationNumber,
        long patientId,
        DateOnly examinationDate,
        long createdByUserId,
        string priority = "Routine",
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(examinationNumber))
        {
            throw new ArgumentException("Examination number is required.", nameof(examinationNumber));
        }

        var normalizedExaminationNumber = examinationNumber.Trim();
        if (normalizedExaminationNumber.Length > 40)
        {
            throw new ArgumentException("Examination number cannot exceed 40 characters.", nameof(examinationNumber));
        }

        if (patientId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(patientId), "Patient ID must be a positive value.");
        }

        if (createdByUserId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(createdByUserId), "Creator user ID must be a positive value.");
        }

        if (string.IsNullOrWhiteSpace(priority))
        {
            throw new ArgumentException("Priority is required.", nameof(priority));
        }

        var normalizedPriority = priority.Trim();
        if (normalizedPriority.Length > 20)
        {
            throw new ArgumentException("Priority cannot exceed 20 characters.", nameof(priority));
        }

        var normalizedNotes = notes?.Trim();
        if (normalizedNotes?.Length > 2000)
        {
            throw new ArgumentException("Notes cannot exceed 2000 characters.", nameof(notes));
        }

        ExaminationNumber = normalizedExaminationNumber;
        PatientId = patientId;
        ExaminationDate = examinationDate;
        CreatedByUserId = createdByUserId;
        Priority = normalizedPriority;
        Notes = normalizedNotes;
        Status = "Registered";
        CreatedAtUtc = DateTime.UtcNow;
        RowVersion = 1;
    }
}

public sealed class ExaminationTest
{
    public long ExaminationTestId { get; private set; }
    public long ExaminationId { get; private set; }
    public long TestDefinitionVersionId { get; private set; }
    public ushort TestInstanceNumber { get; private set; } = 1;
    public string Status { get; private set; } = "Pending";
    public string Priority { get; private set; } = "Routine";
    public DateTime RequestedAtUtc { get; private set; }
    public long RequestedByUserId { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public long? ActualMethodId { get; private set; }
    /// <summary>
    /// Adds a selected test-definition version to an examination. The application
    /// workflow must validate that the version is published and effective, and
    /// that the actor is authorized to register it.
    /// </summary>
    public ExaminationTest(
        long examinationId,
        long testDefinitionVersionId,
        long requestedByUserId,
        ushort testInstanceNumber = 1,
        string priority = "Routine",
        long? actualMethodId = null)
    {
        if (examinationId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(examinationId), "Examination ID must be a positive value.");
        }

        if (testDefinitionVersionId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(testDefinitionVersionId), "Test-definition version ID must be a positive value.");
        }

        if (requestedByUserId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(requestedByUserId), "Requester user ID must be a positive value.");
        }

        if (testInstanceNumber == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(testInstanceNumber), "Test instance number must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(priority))
        {
            throw new ArgumentException("Priority is required.", nameof(priority));
        }

        var normalizedPriority = priority.Trim();
        if (normalizedPriority.Length > 20)
        {
            throw new ArgumentException("Priority cannot exceed 20 characters.", nameof(priority));
        }

        if (actualMethodId is <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(actualMethodId), "Actual method ID must be a positive value when supplied.");
        }

        ExaminationId = examinationId;
        TestDefinitionVersionId = testDefinitionVersionId;
        RequestedByUserId = requestedByUserId;
        TestInstanceNumber = testInstanceNumber;
        Priority = normalizedPriority;
        ActualMethodId = actualMethodId;
        Status = "Pending";
        RequestedAtUtc = DateTime.UtcNow;
    }

    private ExaminationTest() { }
}

public sealed class TestResult
{
    public long TestResultId { get; private set; }
    public long ExaminationTestId { get; private set; }
    public long TestParameterDefinitionId { get; private set; }
    public long? CurrentRevisionId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public long CreatedByUserId { get; private set; }
    private TestResult() { }
}

public sealed class TestResultRevision
{
    public long TestResultRevisionId { get; private set; }
    public long TestResultId { get; private set; }
    public int RevisionNumber { get; private set; }
    public string ValueType { get; private set; } = "Numeric";
    public decimal? NumericValue { get; private set; }
    public string? TextValue { get; private set; }
    public long? ResultValueOptionId { get; private set; }
    public string? TiterValue { get; private set; }
    public long? UnitId { get; private set; }
    public long? ReferenceIntervalId { get; private set; }
    public string? UnitSnapshot { get; private set; }
    public string? ReferenceRangeSnapshot { get; private set; }
    public string? MethodSnapshot { get; private set; }
    public bool IsAbnormal { get; private set; }
    public bool IsCritical { get; private set; }
    public string? Interpretation { get; private set; }
    public string Status { get; private set; } = "Draft";
    public string? RevisionReason { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public long CreatedByUserId { get; private set; }
    private TestResultRevision() { }
}

public sealed class AuditLog
{
    public long AuditLogId { get; private set; }
    public long? ActorUserId { get; private set; }
    public string ActionType { get; private set; } = string.Empty;
    public string EntityType { get; private set; } = string.Empty;
    public long? EntityId { get; private set; }
    public string? Description { get; private set; }
    public string? Reason { get; private set; }
    public string? BeforeData { get; private set; }
    public string? AfterData { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? CorrelationId { get; private set; }
    public DateTime OccurredAtUtc { get; private set; }
    private AuditLog() { }
}
