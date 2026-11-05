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
    // EF Core uses this constructor when materializing persisted results.
    private TestResult() { }

    /// <summary>
    /// Creates the stable result record for one examination-test parameter.
    /// CurrentRevisionId is intentionally not set here; the application workflow
    /// advances it only after a revision has been persisted successfully.
    /// </summary>
    public TestResult(
        long examinationTestId,
        long testParameterDefinitionId,
        long createdByUserId)
    {
        if (examinationTestId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(examinationTestId), "Examination-test ID must be a positive value.");
        }

        if (testParameterDefinitionId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(testParameterDefinitionId), "Test-parameter definition ID must be a positive value.");
        }

        if (createdByUserId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(createdByUserId), "Creator user ID must be a positive value.");
        }

        ExaminationTestId = examinationTestId;
        TestParameterDefinitionId = testParameterDefinitionId;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = DateTime.UtcNow;
    }
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

    // EF Core uses this constructor when materializing persisted revisions.
    private TestResultRevision() { }

    /// <summary>
    /// Creates a draft snapshot of a result value. Parameter compatibility,
    /// reference-interval applicability, and clinical interpretation remain
    /// application responsibilities because they depend on catalog configuration.
    /// </summary>
    public TestResultRevision(
        long testResultId,
        int revisionNumber,
        string valueType,
        long createdByUserId,
        decimal? numericValue = null,
        string? textValue = null,
        long? resultValueOptionId = null,
        string? titerValue = null,
        long? unitId = null,
        long? referenceIntervalId = null,
        string? unitSnapshot = null,
        string? referenceRangeSnapshot = null,
        string? methodSnapshot = null,
        bool isAbnormal = false,
        bool isCritical = false,
        string? interpretation = null,
        string? revisionReason = null)
    {
        if (testResultId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(testResultId), "Test-result ID must be a positive value.");
        }

        if (revisionNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(revisionNumber), "Revision number must be a positive value.");
        }

        if (createdByUserId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(createdByUserId), "Creator user ID must be a positive value.");
        }

        if (string.IsNullOrWhiteSpace(valueType))
        {
            throw new ArgumentException("Value type is required.", nameof(valueType));
        }

        var normalizedValueType = valueType.Trim();
        if (normalizedValueType is not ("Numeric" or "Qualitative" or "Text" or "Titer"))
        {
            throw new ArgumentException("Value type must be Numeric, Qualitative, Text, or Titer.", nameof(valueType));
        }

        if (numericValue is < -999999999999.999999m or > 999999999999.999999m ||
            (numericValue.HasValue && decimal.Round(numericValue.Value, 6) != numericValue.Value))
        {
            throw new ArgumentOutOfRangeException(nameof(numericValue), "Numeric value must fit DECIMAL(18,6).");
        }

        if (resultValueOptionId is <= 0 || unitId is <= 0 || referenceIntervalId is <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(resultValueOptionId), "Optional catalog identifiers must be positive when supplied.");
        }

        var normalizedTextValue = textValue?.Trim();
        var normalizedTiterValue = titerValue?.Trim();
        var normalizedUnitSnapshot = unitSnapshot?.Trim();
        var normalizedReferenceRangeSnapshot = referenceRangeSnapshot?.Trim();
        var normalizedMethodSnapshot = methodSnapshot?.Trim();
        var normalizedInterpretation = interpretation?.Trim();
        var normalizedRevisionReason = revisionReason?.Trim();

        if (normalizedTiterValue?.Length > 80 ||
            normalizedUnitSnapshot?.Length > 120 ||
            normalizedReferenceRangeSnapshot?.Length > 500 ||
            normalizedMethodSnapshot?.Length > 160 ||
            normalizedInterpretation?.Length > 2000 ||
            normalizedRevisionReason?.Length > 1000)
        {
            throw new ArgumentException("A revision text field exceeds its database column length.");
        }

        var hasNumeric = numericValue.HasValue;
        var hasText = normalizedTextValue is not null;
        var hasOption = resultValueOptionId.HasValue;
        var hasTiter = normalizedTiterValue is not null;

        var validPayload = normalizedValueType switch
        {
            "Numeric" => hasNumeric && !hasText && !hasOption && !hasTiter,
            "Qualitative" => hasOption && !hasNumeric && !hasText && !hasTiter,
            "Text" => hasText && !hasNumeric && !hasOption && !hasTiter,
            "Titer" => hasTiter && !hasNumeric && !hasText && !hasOption,
            _ => false
        };

        if (!validPayload)
        {
            throw new ArgumentException("Exactly one value payload matching ValueType must be supplied.");
        }

        TestResultId = testResultId;
        RevisionNumber = revisionNumber;
        ValueType = normalizedValueType;
        NumericValue = numericValue;
        TextValue = normalizedTextValue;
        ResultValueOptionId = resultValueOptionId;
        TiterValue = normalizedTiterValue;
        UnitId = unitId;
        ReferenceIntervalId = referenceIntervalId;
        UnitSnapshot = normalizedUnitSnapshot;
        ReferenceRangeSnapshot = normalizedReferenceRangeSnapshot;
        MethodSnapshot = normalizedMethodSnapshot;
        IsAbnormal = isAbnormal;
        IsCritical = isCritical;
        Interpretation = normalizedInterpretation;
        Status = "Draft";
        RevisionReason = normalizedRevisionReason;
        CreatedAtUtc = DateTime.UtcNow;
        CreatedByUserId = createdByUserId;
    }
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
