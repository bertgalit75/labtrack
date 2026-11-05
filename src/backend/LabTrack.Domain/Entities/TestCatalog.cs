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

    /// <summary>Creates an active test-catalog definition. Category existence and authorization are validated by the application.</summary>
    public TestDefinition(long testCategoryId, string testCode, string testName, string? description = null)
    {
        if (testCategoryId <= 0) throw new ArgumentOutOfRangeException(nameof(testCategoryId));
        TestCode = RequiredText(testCode, 50, nameof(testCode));
        TestName = RequiredText(testName, 180, nameof(testName));
        Description = OptionalText(description, 1000, nameof(description));
        TestCategoryId = testCategoryId;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    internal static string RequiredText(string value, int maxLength, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("A value is required.", parameterName);
        var normalized = value.Trim();
        if (normalized.Length > maxLength) throw new ArgumentException($"Value cannot exceed {maxLength} characters.", parameterName);
        return normalized;
    }

    internal static string? OptionalText(string? value, int maxLength, string parameterName)
    {
        var normalized = value?.Trim();
        if (normalized?.Length > maxLength) throw new ArgumentException($"Value cannot exceed {maxLength} characters.", parameterName);
        return normalized;
    }
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

    /// <summary>Creates a draft version. Publication and effective-version selection are application workflows.</summary>
    public TestDefinitionVersion(long testDefinitionId, int versionNumber, long createdByUserId,
        DateOnly? effectiveFrom = null, DateOnly? effectiveTo = null, long? defaultMethodId = null, string? instructions = null)
    {
        if (testDefinitionId <= 0) throw new ArgumentOutOfRangeException(nameof(testDefinitionId));
        if (versionNumber <= 0) throw new ArgumentOutOfRangeException(nameof(versionNumber));
        if (createdByUserId <= 0) throw new ArgumentOutOfRangeException(nameof(createdByUserId));
        if (effectiveFrom.HasValue && effectiveTo.HasValue && effectiveTo < effectiveFrom)
            throw new ArgumentException("Effective-to date cannot precede effective-from date.", nameof(effectiveTo));
        if (defaultMethodId is <= 0) throw new ArgumentOutOfRangeException(nameof(defaultMethodId));
        TestDefinitionId = testDefinitionId;
        VersionNumber = versionNumber;
        CreatedByUserId = createdByUserId;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
        DefaultMethodId = defaultMethodId;
        Instructions = TestDefinition.OptionalText(instructions, 2000, nameof(instructions));
        VersionStatus = "Draft";
        CreatedAtUtc = DateTime.UtcNow;
    }
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

    public TestParameterDefinition(long testDefinitionVersionId, string parameterCode, string parameterName,
        string valueType, long? unitId = null, byte? decimalPlaces = null, bool isRequired = true, int displayOrder = 0)
    {
        if (testDefinitionVersionId <= 0) throw new ArgumentOutOfRangeException(nameof(testDefinitionVersionId));
        if (unitId is <= 0) throw new ArgumentOutOfRangeException(nameof(unitId));
        if (string.IsNullOrWhiteSpace(valueType)) throw new ArgumentException("Value type is required.", nameof(valueType));
        var normalizedType = valueType.Trim();
        if (normalizedType is not ("Numeric" or "Qualitative" or "Text" or "Titer"))
            throw new ArgumentException("Value type must be Numeric, Qualitative, Text, or Titer.", nameof(valueType));
        TestDefinitionVersionId = testDefinitionVersionId;
        ParameterCode = TestDefinition.RequiredText(parameterCode, 60, nameof(parameterCode));
        ParameterName = TestDefinition.RequiredText(parameterName, 160, nameof(parameterName));
        ValueType = normalizedType;
        UnitId = unitId;
        DecimalPlaces = decimalPlaces;
        IsRequired = isRequired;
        DisplayOrder = displayOrder;
        IsActive = true;
    }
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

    public ReferenceInterval(long testParameterDefinitionId, string? sexApplicability = null,
        uint? ageMinimumDays = null, uint? ageMaximumDays = null, decimal? lowerLimit = null, decimal? upperLimit = null,
        string? referenceText = null, long? unitId = null, long? testMethodId = null,
        DateOnly? effectiveFrom = null, DateOnly? effectiveTo = null, int priority = 0)
    {
        if (testParameterDefinitionId <= 0) throw new ArgumentOutOfRangeException(nameof(testParameterDefinitionId));
        if (unitId is <= 0) throw new ArgumentOutOfRangeException(nameof(unitId));
        if (testMethodId is <= 0) throw new ArgumentOutOfRangeException(nameof(testMethodId));
        if (ageMinimumDays.HasValue && ageMaximumDays.HasValue && ageMaximumDays < ageMinimumDays)
            throw new ArgumentException("Maximum age cannot be less than minimum age.", nameof(ageMaximumDays));
        if (lowerLimit is < -999999999999.999999m or > 999999999999.999999m ||
            upperLimit is < -999999999999.999999m or > 999999999999.999999m ||
            (lowerLimit.HasValue && decimal.Round(lowerLimit.Value, 6) != lowerLimit.Value) ||
            (upperLimit.HasValue && decimal.Round(upperLimit.Value, 6) != upperLimit.Value))
            throw new ArgumentOutOfRangeException(nameof(lowerLimit), "Limits must fit DECIMAL(18,6).");
        if (lowerLimit.HasValue && upperLimit.HasValue && upperLimit < lowerLimit)
            throw new ArgumentException("Upper limit cannot be less than lower limit.", nameof(upperLimit));
        if (effectiveFrom.HasValue && effectiveTo.HasValue && effectiveTo < effectiveFrom)
            throw new ArgumentException("Effective-to date cannot precede effective-from date.", nameof(effectiveTo));
        TestParameterDefinitionId = testParameterDefinitionId;
        SexApplicability = TestDefinition.OptionalText(sexApplicability, 30, nameof(sexApplicability));
        AgeMinimumDays = ageMinimumDays;
        AgeMaximumDays = ageMaximumDays;
        LowerLimit = lowerLimit;
        UpperLimit = upperLimit;
        ReferenceText = TestDefinition.OptionalText(referenceText, 500, nameof(referenceText));
        UnitId = unitId;
        TestMethodId = testMethodId;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
        Priority = priority;
        IsActive = true;
    }
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

    public ResultValueOption(long testParameterDefinitionId, string optionCode, string optionLabel,
        int displayOrder = 0, bool isAbnormal = false)
    {
        if (testParameterDefinitionId <= 0) throw new ArgumentOutOfRangeException(nameof(testParameterDefinitionId));
        TestParameterDefinitionId = testParameterDefinitionId;
        OptionCode = TestDefinition.RequiredText(optionCode, 60, nameof(optionCode));
        OptionLabel = TestDefinition.RequiredText(optionLabel, 160, nameof(optionLabel));
        DisplayOrder = displayOrder;
        IsAbnormal = isAbnormal;
        IsActive = true;
    }
}
