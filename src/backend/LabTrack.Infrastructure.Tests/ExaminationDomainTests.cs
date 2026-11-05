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
}
