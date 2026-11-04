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
    private Examination() { }
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
