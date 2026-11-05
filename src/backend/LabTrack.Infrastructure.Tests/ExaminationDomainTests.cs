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


    [Fact]
    public void Test_catalog_definition_creation_normalizes_fields_and_sets_active_timestamp()
    {
        var before = DateTime.UtcNow;
        var entity = new TestDefinition(3, " CBC ", " Complete Blood Count ", " Basic panel ");
        Assert.Equal(3, entity.TestCategoryId);
        Assert.Equal("CBC", entity.TestCode);
        Assert.Equal("Complete Blood Count", entity.TestName);
        Assert.Equal("Basic panel", entity.Description);
        Assert.True(entity.IsActive);
        Assert.Equal(DateTimeKind.Utc, entity.CreatedAtUtc.Kind);
        Assert.InRange(entity.CreatedAtUtc, before, DateTime.UtcNow);
    }

    [Fact]
    public void Test_catalog_definition_rejects_invalid_ids_and_lengths()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new TestDefinition(0, "CBC", "CBC"));
        Assert.Throws<ArgumentException>(() => new TestDefinition(1, " ", "CBC"));
        Assert.Throws<ArgumentException>(() => new TestDefinition(1, new string('C', 51), "CBC"));
        Assert.Throws<ArgumentException>(() => new TestDefinition(1, "CBC", new string('N', 181)));
    }

    [Fact]
    public void Test_definition_version_starts_as_draft_and_validates_dates_and_ids()
    {
        var version = new TestDefinitionVersion(5, 2, 8, new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), 9, " Instructions ");
        Assert.Equal(5, version.TestDefinitionId);
        Assert.Equal(2, version.VersionNumber);
        Assert.Equal("Draft", version.VersionStatus);
        Assert.Equal(8, version.CreatedByUserId);
        Assert.Equal("Instructions", version.Instructions);
        Assert.Null(version.PublishedAtUtc);
        Assert.Null(version.PublishedByUserId);
        Assert.Equal(DateTimeKind.Utc, version.CreatedAtUtc.Kind);
        Assert.Throws<ArgumentException>(() => new TestDefinitionVersion(5, 1, 8,
            new DateOnly(2026, 12, 31), new DateOnly(2026, 1, 1)));
        Assert.Throws<ArgumentOutOfRangeException>(() => new TestDefinitionVersion(0, 1, 8));
    }

    [Fact]
    public void Test_parameter_definition_validates_value_type_and_required_fields()
    {
        var parameter = new TestParameterDefinition(4, " HGB ", " Hemoglobin ", " Numeric ", 7, 2, false, 3);
        Assert.Equal("HGB", parameter.ParameterCode);
        Assert.Equal("Hemoglobin", parameter.ParameterName);
        Assert.Equal("Numeric", parameter.ValueType);
        Assert.Equal((byte)2, parameter.DecimalPlaces);
        Assert.False(parameter.IsRequired);
        Assert.Equal(3, parameter.DisplayOrder);
        Assert.True(parameter.IsActive);
        Assert.Throws<ArgumentException>(() => new TestParameterDefinition(4, "HGB", "Hemoglobin", "Other"));
        Assert.Throws<ArgumentOutOfRangeException>(() => new TestParameterDefinition(4, "HGB", "Hemoglobin", "Numeric", 0));
    }

    [Fact]
    public void Reference_interval_enforces_database_ordering_and_precision_constraints()
    {
        var interval = new ReferenceInterval(4, ageMinimumDays: 10, ageMaximumDays: 20,
            lowerLimit: 1.123456m, upperLimit: 9.5m, referenceText: " Normal ", unitId: 2,
            effectiveFrom: new DateOnly(2026, 1, 1), effectiveTo: new DateOnly(2026, 12, 31));
        Assert.Equal(4, interval.TestParameterDefinitionId);
        Assert.Equal("Normal", interval.ReferenceText);
        Assert.Equal(1.123456m, interval.LowerLimit);
        Assert.Throws<ArgumentException>(() => new ReferenceInterval(4, ageMinimumDays: 20, ageMaximumDays: 10));
        Assert.Throws<ArgumentException>(() => new ReferenceInterval(4, lowerLimit: 10m, upperLimit: 1m));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ReferenceInterval(4, lowerLimit: 1.1234567m));
    }

    [Fact]
    public void Result_value_option_validates_required_fields_and_sets_defaults()
    {
        var option = new ResultValueOption(4, " POS ", " Positive ", 2, true);
        Assert.Equal(4, option.TestParameterDefinitionId);
        Assert.Equal("POS", option.OptionCode);
        Assert.Equal("Positive", option.OptionLabel);
        Assert.Equal(2, option.DisplayOrder);
        Assert.True(option.IsAbnormal);
        Assert.True(option.IsActive);
        Assert.Throws<ArgumentException>(() => new ResultValueOption(4, " ", "Positive"));
    }

    [Fact]
    public void Audit_log_creation_normalizes_fields_and_sets_utc_timestamp()
    {
        var before = DateTime.UtcNow;
        var audit = new AuditLog(" UPDATE ", " Patient ", 12, 34, " Changed ", " Correction ",
            "{\"old\":1}", "{\"new\":2}", "127.0.0.1", " test-agent ", " corr-1 ");
        Assert.Equal("UPDATE", audit.ActionType);
        Assert.Equal("Patient", audit.EntityType);
        Assert.Equal(12, audit.ActorUserId);
        Assert.Equal(34, audit.EntityId);
        Assert.Equal("Correction", audit.Reason);
        Assert.Equal("{\"old\":1}", audit.BeforeData);
        Assert.Equal("{\"new\":2}", audit.AfterData);
        Assert.Equal("test-agent", audit.UserAgent);
        Assert.Equal("corr-1", audit.CorrelationId);
        Assert.Equal(DateTimeKind.Utc, audit.OccurredAtUtc.Kind);
        Assert.InRange(audit.OccurredAtUtc, before, DateTime.UtcNow);
        Assert.Throws<ArgumentException>(() => new AuditLog(" ", "Patient"));
        Assert.Throws<ArgumentOutOfRangeException>(() => new AuditLog("UPDATE", "Patient", actorUserId: 0));
        Assert.Throws<ArgumentException>(() => new AuditLog("UPDATE", "Patient", description: new string('D', 2001)));
    }

}
