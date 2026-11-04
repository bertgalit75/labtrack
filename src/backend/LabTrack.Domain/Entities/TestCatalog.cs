namespace LabTrack.Domain.Entities;

public sealed class TestDefinition
{
    public long TestDefinitionId { get; private set; }
    public long TestCategoryId { get; private set; }
    public string TestCode { get; private set; } = string.Empty;
    public string TestName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    private TestDefinition() { }
}

public sealed class TestDefinitionVersion
{
    public long TestDefinitionVersionId { get; private set; }
    public long TestDefinitionId { get; private set; }
    public int VersionNumber { get; private set; }
    public string VersionStatus { get; private set; } = "Draft";
    public DateOnly? EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public long? DefaultMethodId { get; private set; }
    public string? Instructions { get; private set; }
    public DateTime? PublishedAtUtc { get; private set; }
    public long? PublishedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public long CreatedByUserId { get; private set; }
    private TestDefinitionVersion() { }
}

public sealed class TestParameterDefinition
{
    public long TestParameterDefinitionId { get; private set; }
    public long TestDefinitionVersionId { get; private set; }
    public string ParameterCode { get; private set; } = string.Empty;
    public string ParameterName { get; private set; } = string.Empty;
    public string ValueType { get; private set; } = "Numeric";
    public long? UnitId { get; private set; }
    public byte? DecimalPlaces { get; private set; }
    public bool IsRequired { get; private set; } = true;
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; } = true;
    private TestParameterDefinition() { }
}

public sealed class ReferenceInterval
{
    public long ReferenceIntervalId { get; private set; }
    public long TestParameterDefinitionId { get; private set; }
    public string? SexApplicability { get; private set; }
    public uint? AgeMinimumDays { get; private set; }
    public uint? AgeMaximumDays { get; private set; }
    public decimal? LowerLimit { get; private set; }
    public decimal? UpperLimit { get; private set; }
    public string? ReferenceText { get; private set; }
    public long? UnitId { get; private set; }
    public long? TestMethodId { get; private set; }
    public DateOnly? EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public int Priority { get; private set; }
    public bool IsActive { get; private set; } = true;
    private ReferenceInterval() { }
}

public sealed class ResultValueOption
{
    public long ResultValueOptionId { get; private set; }
    public long TestParameterDefinitionId { get; private set; }
    public string OptionCode { get; private set; } = string.Empty;
    public string OptionLabel { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }
    public bool IsAbnormal { get; private set; }
    public bool IsActive { get; private set; } = true;
    private ResultValueOption() { }
}
