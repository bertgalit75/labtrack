namespace LabTrack.Domain.Results;

public enum ResultValueType { Numeric, Qualitative }
public enum ResultInterpretation { Low, Normal, High, Abnormal, Indeterminate, Unclassified }

public sealed record NumericReferenceInterval(decimal? LowerBound, decimal? UpperBound)
{
    public ResultInterpretation Interpret(decimal value)
    {
        if (LowerBound is null && UpperBound is null) return ResultInterpretation.Unclassified;
        if (LowerBound is not null && value < LowerBound.Value) return ResultInterpretation.Low;
        if (UpperBound is not null && value > UpperBound.Value) return ResultInterpretation.High;
        return ResultInterpretation.Normal;
    }
}

public sealed record QualitativeValueOption(string Code, string DisplayText, ResultInterpretation Interpretation);

/// <summary>Pure interpretation logic. It does not supply or invent clinical reference ranges.</summary>
public static class ResultInterpreter
{
    public static ResultInterpretation InterpretNumeric(decimal value, NumericReferenceInterval interval)
        => interval.Interpret(value);

    public static ResultInterpretation InterpretQualitative(string selectedCode, IReadOnlyCollection<QualitativeValueOption> configuredOptions)
    {
        var option = configuredOptions.SingleOrDefault(x => string.Equals(x.Code, selectedCode, StringComparison.Ordinal));
        return option?.Interpretation ?? ResultInterpretation.Unclassified;
    }
}
