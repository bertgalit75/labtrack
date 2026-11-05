using LabTrack.Domain.Entities;
using Xunit;

namespace LabTrack.Infrastructure.Tests;

public sealed class ExaminationDomainTests
{
    private static readonly DateOnly ExaminationDate = new(2026, 11, 5);

    [Fact]
    public void Creation_sets_required_values_and_registered_defaults()
    {
        var beforeCreation = DateTime.UtcNow;

        var examination = new Examination(
            "EX-202611-000001",
            patientId: 12,
            examinationDate: ExaminationDate,
            createdByUserId: 34);

        var afterCreation = DateTime.UtcNow;

        Assert.Equal("EX-202611-000001", examination.ExaminationNumber);
        Assert.Equal(12, examination.PatientId);
        Assert.Equal(ExaminationDate, examination.ExaminationDate);
        Assert.Equal(34, examination.CreatedByUserId);
        Assert.Equal("Routine", examination.Priority);
        Assert.Equal("Registered", examination.Status);
        Assert.Null(examination.Notes);
        Assert.Equal(1, examination.RowVersion);
        Assert.Equal(DateTimeKind.Utc, examination.CreatedAtUtc.Kind);
        Assert.InRange(examination.CreatedAtUtc, beforeCreation, afterCreation);
    }

    [Fact]
    public void Creation_trims_text_fields_and_preserves_custom_priority_and_notes()
    {
        var examination = new Examination(
            "  EX-202611-000002  ",
            patientId: 12,
            examinationDate: ExaminationDate,
            createdByUserId: 34,
            priority: "  Urgent  ",
            notes: "  Fasting patient  ");

        Assert.Equal("EX-202611-000002", examination.ExaminationNumber);
        Assert.Equal("Urgent", examination.Priority);
        Assert.Equal("Fasting patient", examination.Notes);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Creation_rejects_missing_examination_number(string? examinationNumber)
    {
        Assert.Throws<ArgumentException>(() => new Examination(
            examinationNumber!,
            patientId: 12,
            examinationDate: ExaminationDate,
            createdByUserId: 34));
    }

    [Fact]
    public void Creation_rejects_values_that_exceed_database_lengths()
    {
        Assert.Throws<ArgumentException>(() => new Examination(
            new string('E', 41),
            patientId: 12,
            examinationDate: ExaminationDate,
            createdByUserId: 34));

        Assert.Throws<ArgumentException>(() => new Examination(
            "EX-202611-000003",
            patientId: 12,
            examinationDate: ExaminationDate,
            createdByUserId: 34,
            priority: new string('P', 21)));

        Assert.Throws<ArgumentException>(() => new Examination(
            "EX-202611-000004",
            patientId: 12,
            examinationDate: ExaminationDate,
            createdByUserId: 34,
            notes: new string('N', 2001)));
    }

    [Fact]
    public void Creation_rejects_non_positive_foreign_key_identifiers()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Examination(
            "EX-202611-000005",
            patientId: 0,
            examinationDate: ExaminationDate,
            createdByUserId: 34));

        Assert.Throws<ArgumentOutOfRangeException>(() => new Examination(
            "EX-202611-000006",
            patientId: 12,
            examinationDate: ExaminationDate,
            createdByUserId: 0));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Creation_rejects_missing_priority(string? priority)
    {
        Assert.Throws<ArgumentException>(() => new Examination(
            "EX-202611-000007",
            patientId: 12,
            examinationDate: ExaminationDate,
            createdByUserId: 34,
            priority: priority!));
    }

    [Fact]
    public void Examination_test_creation_sets_pending_defaults_and_utc_request_time()
    {
        var beforeCreation = DateTime.UtcNow;

        var examinationTest = new ExaminationTest(
            examinationId: 12,
            testDefinitionVersionId: 56,
            requestedByUserId: 34);

        var afterCreation = DateTime.UtcNow;

        Assert.Equal(12, examinationTest.ExaminationId);
        Assert.Equal(56, examinationTest.TestDefinitionVersionId);
        Assert.Equal(34, examinationTest.RequestedByUserId);
        Assert.Equal((ushort)1, examinationTest.TestInstanceNumber);
        Assert.Equal("Routine", examinationTest.Priority);
        Assert.Equal("Pending", examinationTest.Status);
        Assert.Null(examinationTest.CompletedAtUtc);
        Assert.Null(examinationTest.ActualMethodId);
        Assert.Equal(DateTimeKind.Utc, examinationTest.RequestedAtUtc.Kind);
        Assert.InRange(examinationTest.RequestedAtUtc, beforeCreation, afterCreation);
    }

    [Fact]
    public void Examination_test_creation_accepts_custom_instance_priority_and_method()
    {
        var examinationTest = new ExaminationTest(
            examinationId: 12,
            testDefinitionVersionId: 56,
            requestedByUserId: 34,
            testInstanceNumber: 2,
            priority: " Urgent ",
            actualMethodId: 78);

        Assert.Equal((ushort)2, examinationTest.TestInstanceNumber);
        Assert.Equal("Urgent", examinationTest.Priority);
        Assert.Equal(78, examinationTest.ActualMethodId);
    }

    [Fact]
    public void Examination_test_creation_rejects_non_positive_foreign_key_identifiers()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ExaminationTest(
            examinationId: 0,
            testDefinitionVersionId: 56,
            requestedByUserId: 34));

        Assert.Throws<ArgumentOutOfRangeException>(() => new ExaminationTest(
            examinationId: 12,
            testDefinitionVersionId: 0,
            requestedByUserId: 34));

        Assert.Throws<ArgumentOutOfRangeException>(() => new ExaminationTest(
            examinationId: 12,
            testDefinitionVersionId: 56,
            requestedByUserId: 0));

        Assert.Throws<ArgumentOutOfRangeException>(() => new ExaminationTest(
            examinationId: 12,
            testDefinitionVersionId: 56,
            requestedByUserId: 34,
            actualMethodId: 0));
    }

    [Fact]
    public void Examination_test_creation_rejects_zero_instance_number_and_invalid_priority()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ExaminationTest(
            examinationId: 12,
            testDefinitionVersionId: 56,
            requestedByUserId: 34,
            testInstanceNumber: 0));

        Assert.Throws<ArgumentException>(() => new ExaminationTest(
            examinationId: 12,
            testDefinitionVersionId: 56,
            requestedByUserId: 34,
            priority: " "));

        Assert.Throws<ArgumentException>(() => new ExaminationTest(
            examinationId: 12,
            testDefinitionVersionId: 56,
            requestedByUserId: 34,
            priority: new string('P', 21)));
    }


    [Fact]
    public void Test_result_creation_sets_identity_and_utc_timestamp_without_current_revision()
    {
        var beforeCreation = DateTime.UtcNow;

        var result = new TestResult(
            examinationTestId: 12,
            testParameterDefinitionId: 56,
            createdByUserId: 34);

        var afterCreation = DateTime.UtcNow;

        Assert.Equal(12, result.ExaminationTestId);
        Assert.Equal(56, result.TestParameterDefinitionId);
        Assert.Equal(34, result.CreatedByUserId);
        Assert.Null(result.CurrentRevisionId);
        Assert.Equal(DateTimeKind.Utc, result.CreatedAtUtc.Kind);
        Assert.InRange(result.CreatedAtUtc, beforeCreation, afterCreation);
    }

    [Fact]
    public void Test_result_creation_rejects_non_positive_identifiers()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new TestResult(0, 56, 34));
        Assert.Throws<ArgumentOutOfRangeException>(() => new TestResult(12, 0, 34));
        Assert.Throws<ArgumentOutOfRangeException>(() => new TestResult(12, 56, 0));
    }

    [Fact]
    public void Numeric_revision_creation_sets_draft_defaults_and_value_snapshots()
    {
        var beforeCreation = DateTime.UtcNow;

        var revision = new TestResultRevision(
            testResultId: 10,
            revisionNumber: 1,
            valueType: " Numeric ",
            createdByUserId: 34,
            numericValue: 12.345678m,
            unitId: 4,
            referenceIntervalId: 5,
            unitSnapshot: " mg/dL ",
            referenceRangeSnapshot: " 5.0-15.0 ",
            methodSnapshot: " Method A ",
            isAbnormal: true,
            interpretation: " Above expected ",
            revisionReason: " Initial entry ");

        var afterCreation = DateTime.UtcNow;

        Assert.Equal(10, revision.TestResultId);
        Assert.Equal(1, revision.RevisionNumber);
        Assert.Equal("Numeric", revision.ValueType);
        Assert.Equal(12.345678m, revision.NumericValue);
        Assert.Equal(4, revision.UnitId);
        Assert.Equal(5, revision.ReferenceIntervalId);
        Assert.Equal("mg/dL", revision.UnitSnapshot);
        Assert.Equal("5.0-15.0", revision.ReferenceRangeSnapshot);
        Assert.Equal("Method A", revision.MethodSnapshot);
        Assert.True(revision.IsAbnormal);
        Assert.False(revision.IsCritical);
        Assert.Equal("Above expected", revision.Interpretation);
        Assert.Equal("Initial entry", revision.RevisionReason);
        Assert.Equal("Draft", revision.Status);
        Assert.Equal(34, revision.CreatedByUserId);
        Assert.Equal(DateTimeKind.Utc, revision.CreatedAtUtc.Kind);
        Assert.InRange(revision.CreatedAtUtc, beforeCreation, afterCreation);
    }

    [Theory]
    [InlineData("Qualitative")]
    [InlineData("Text")]
    [InlineData("Titer")]
    public void Revision_creation_accepts_each_non_numeric_value_type(string valueType)
    {
        var revision = valueType switch
        {
            "Qualitative" => new TestResultRevision(10, 1, valueType, 34, resultValueOptionId: 7),
            "Text" => new TestResultRevision(10, 1, valueType, 34, textValue: "Detected"),
            "Titer" => new TestResultRevision(10, 1, valueType, 34, titerValue: "1:160"),
            _ => throw new InvalidOperationException()
        };

        Assert.Equal(valueType, revision.ValueType);
        Assert.Equal("Draft", revision.Status);
    }

    [Fact]
    public void Revision_creation_rejects_invalid_identifiers_revision_number_and_value_type()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new TestResultRevision(0, 1, "Numeric", 34, numericValue: 1m));
        Assert.Throws<ArgumentOutOfRangeException>(() => new TestResultRevision(10, 0, "Numeric", 34, numericValue: 1m));
        Assert.Throws<ArgumentOutOfRangeException>(() => new TestResultRevision(10, 1, "Numeric", 0, numericValue: 1m));
        Assert.Throws<ArgumentException>(() => new TestResultRevision(10, 1, "Unknown", 34, numericValue: 1m));
        Assert.Throws<ArgumentException>(() => new TestResultRevision(10, 1, " ", 34, numericValue: 1m));
    }

    [Fact]
    public void Revision_creation_requires_exactly_one_payload_matching_value_type()
    {
        Assert.Throws<ArgumentException>(() => new TestResultRevision(10, 1, "Numeric", 34));
        Assert.Throws<ArgumentException>(() => new TestResultRevision(10, 1, "Numeric", 34, numericValue: 1m, textValue: "also text"));
        Assert.Throws<ArgumentException>(() => new TestResultRevision(10, 1, "Qualitative", 34, numericValue: 1m));
        Assert.Throws<ArgumentException>(() => new TestResultRevision(10, 1, "Text", 34, textValue: " "));
        Assert.Throws<ArgumentException>(() => new TestResultRevision(10, 1, "Titer", 34, titerValue: " "));
    }

    [Fact]
    public void Revision_creation_enforces_decimal_precision_and_optional_identifier_rules()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new TestResultRevision(
            10, 1, "Numeric", 34, numericValue: 1.1234567m));

        Assert.Throws<ArgumentOutOfRangeException>(() => new TestResultRevision(
            10, 1, "Numeric", 34, numericValue: 1m, unitId: 0));
    }

    [Fact]
    public void Revision_creation_enforces_snapshot_and_reason_column_lengths()
    {
        Assert.Throws<ArgumentException>(() => new TestResultRevision(
            10, 1, "Titer", 34, titerValue: new string('T', 81)));

        Assert.Throws<ArgumentException>(() => new TestResultRevision(
            10, 1, "Numeric", 34, numericValue: 1m, interpretation: new string('I', 2001)));

        Assert.Throws<ArgumentException>(() => new TestResultRevision(
            10, 1, "Numeric", 34, numericValue: 1m, revisionReason: new string('R', 1001)));
    }


    [Fact]
    public void Revision_creation_rejects_blank_text_payloads_after_normalization()
    {
        Assert.Throws<ArgumentException>(() => new TestResultRevision(
            10, 1, "Text", 34, textValue: "   "));

        Assert.Throws<ArgumentException>(() => new TestResultRevision(
            10, 1, "Titer", 34, titerValue: "   "));
    }

}
