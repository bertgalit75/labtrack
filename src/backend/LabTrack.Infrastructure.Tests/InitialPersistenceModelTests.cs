using LabTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace LabTrack.Infrastructure.Tests;

public sealed class InitialPersistenceModelTests
{
    private static readonly string[] ExpectedTables =
    [
        "AspNetRoles", "AspNetUsers", "AspNetRoleClaims", "AspNetUserClaims",
        "AspNetUserLogins", "AspNetUserRoles", "AspNetUserTokens",
        "Patients", "TestDefinitions", "TestDefinitionVersions", "TestParameterDefinitions",
        "ReferenceIntervals", "ResultValueOptions", "Examinations", "ExaminationTests",
        "TestResults", "TestResultRevisions", "AuditLogs"
    ];

    [Fact]
    public void Model_maps_exactly_the_approved_18_tables()
    {
        var options = new DbContextOptionsBuilder<LabTrackDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        using var context = new LabTrackDbContext(options);
        var actual = context.Model.GetEntityTypes()
            .Select(entity => entity.GetTableName())
            .Where(name => name is not null)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(ExpectedTables.Order(StringComparer.Ordinal), actual);
        Assert.Equal(18, actual.Length);
    }

    [Fact]
    public void Model_does_not_map_any_of_the_30_out_of_scope_tables()
    {
        var options = new DbContextOptionsBuilder<LabTrackDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        using var context = new LabTrackDbContext(options);
        var mapped = context.Model.GetEntityTypes()
            .Select(entity => entity.GetTableName())
            .Where(name => name is not null)
            .ToHashSet(StringComparer.Ordinal);

        string[] excluded =
        [
            "StaffProfiles", "Permissions", "RolePermissions", "UserPermissions",
            "ProfessionalCredentials", "StaffAuthorizations", "PatientMatchingRules",
            "PatientDuplicateCandidates", "PatientDuplicateReviews", "PatientMergeEvents",
            "PatientFieldChangePolicies", "PatientChangeRequests", "PatientChangeApprovals",
            "PatientDemographicHistory", "TestCategories", "TestMethods", "Units",
            "ExaminationStatusHistory", "ResultSubmissions", "ResultSubmissionItems",
            "ResultVerificationEvents", "CriticalResultEvents", "StoredFiles",
            "ReportReleases", "ReportReleaseItems", "ReportSignatures", "ReportAmendments",
            "ReportDistributionEvents", "WorkflowEvents", "NumberingCounters"
        ];

        Assert.Empty(excluded.Where(mapped.Contains));
    }

    [Fact]
    public void In_scope_entities_have_the_expected_column_inventory()
    {
        var options = new DbContextOptionsBuilder<LabTrackDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        using var context = new LabTrackDbContext(options);
        var expected = new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["AspNetRoles"] = ["Id", "Name", "NormalizedName", "ConcurrencyStamp"],
            ["AspNetUsers"] = ["Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount", "IsActive", "DeactivatedAtUtc"],
            ["AspNetRoleClaims"] = ["Id", "RoleId", "ClaimType", "ClaimValue"],
            ["AspNetUserClaims"] = ["Id", "UserId", "ClaimType", "ClaimValue"],
            ["AspNetUserLogins"] = ["LoginProvider", "ProviderKey", "ProviderDisplayName", "UserId"],
            ["AspNetUserRoles"] = ["UserId", "RoleId"],
            ["AspNetUserTokens"] = ["UserId", "LoginProvider", "Name", "Value"],
            ["Patients"] = ["PatientId", "PatientNumber", "FirstName", "MiddleName", "LastName", "Suffix", "DateOfBirth", "SexAtRegistration", "CivilStatus", "Nationality", "MobileNumber", "EmailAddress", "AddressLine1", "AddressLine2", "Barangay", "CityMunicipality", "Province", "PostalCode", "PatientStatus", "CanonicalPatientId", "RegisteredAtUtc", "RegisteredByUserId", "UpdatedAtUtc", "UpdatedByUserId", "RowVersion"],
            ["TestDefinitions"] = ["TestDefinitionId", "TestCategoryId", "TestCode", "TestName", "Description", "IsActive", "CreatedAtUtc", "UpdatedAtUtc"],
            ["TestDefinitionVersions"] = ["TestDefinitionVersionId", "TestDefinitionId", "VersionNumber", "VersionStatus", "EffectiveFrom", "EffectiveTo", "DefaultMethodId", "Instructions", "PublishedAtUtc", "PublishedByUserId", "CreatedAtUtc", "CreatedByUserId"],
            ["TestParameterDefinitions"] = ["TestParameterDefinitionId", "TestDefinitionVersionId", "ParameterCode", "ParameterName", "ValueType", "UnitId", "DecimalPlaces", "IsRequired", "DisplayOrder", "IsActive"],
            ["ReferenceIntervals"] = ["ReferenceIntervalId", "TestParameterDefinitionId", "SexApplicability", "AgeMinimumDays", "AgeMaximumDays", "LowerLimit", "UpperLimit", "ReferenceText", "UnitId", "TestMethodId", "EffectiveFrom", "EffectiveTo", "Priority", "IsActive"],
            ["ResultValueOptions"] = ["ResultValueOptionId", "TestParameterDefinitionId", "OptionCode", "OptionLabel", "DisplayOrder", "IsAbnormal", "IsActive"],
            ["Examinations"] = ["ExaminationId", "ExaminationNumber", "PatientId", "ExaminationDate", "Priority", "Status", "Notes", "CreatedAtUtc", "CreatedByUserId", "RowVersion"],
            ["ExaminationTests"] = ["ExaminationTestId", "ExaminationId", "TestDefinitionVersionId", "TestInstanceNumber", "Status", "Priority", "RequestedAtUtc", "RequestedByUserId", "CompletedAtUtc", "ActualMethodId"],
            ["TestResults"] = ["TestResultId", "ExaminationTestId", "TestParameterDefinitionId", "CurrentRevisionId", "CreatedAtUtc", "CreatedByUserId"],
            ["TestResultRevisions"] = ["TestResultRevisionId", "TestResultId", "RevisionNumber", "ValueType", "NumericValue", "TextValue", "ResultValueOptionId", "TiterValue", "UnitId", "ReferenceIntervalId", "UnitSnapshot", "ReferenceRangeSnapshot", "MethodSnapshot", "IsAbnormal", "IsCritical", "Interpretation", "Status", "RevisionReason", "CreatedAtUtc", "CreatedByUserId"],
            ["AuditLogs"] = ["AuditLogId", "ActorUserId", "ActionType", "EntityType", "EntityId", "Description", "Reason", "BeforeData", "AfterData", "IpAddress", "UserAgent", "CorrelationId", "OccurredAtUtc"]
        };

        foreach (var (table, expectedColumns) in expected)
        {
            var entity = context.Model.GetEntityTypes().Single(x => x.GetTableName() == table);
            var storeObject = StoreObjectIdentifier.Table(table, entity.GetSchema());
            var actualColumns = entity.GetProperties()
                .Select(property => property.GetColumnName(storeObject))
                .Order(StringComparer.Ordinal)
                .ToArray();

            Assert.Equal(expectedColumns.Order(StringComparer.Ordinal), actualColumns);
        }
    }

    [Fact]
    public void Declared_application_foreign_keys_use_restrict_delete_behavior()
    {
        var options = new DbContextOptionsBuilder<LabTrackDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        using var context = new LabTrackDbContext(options);
        var foreignKeys = context.Model.GetEntityTypes()
            .SelectMany(entity => entity.GetForeignKeys())
            .Where(fk => fk.DeclaringEntityType.GetTableName() is not null);

        Assert.All(foreignKeys, foreignKey => Assert.Equal(DeleteBehavior.Restrict, foreignKey.DeleteBehavior));
    }
    [Fact]
    public void Model_contains_the_declared_named_indexes_for_the_18_tables()
    {
        var options = new DbContextOptionsBuilder<LabTrackDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        using var context = new LabTrackDbContext(options);
        var expected = new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["AspNetRoles"] = ["UX_AspNetRoles_NormalizedName"],
            ["AspNetUsers"] = ["UX_AspNetUsers_NormalizedUserName", "IX_AspNetUsers_NormalizedEmail"],
            ["AspNetRoleClaims"] = ["IX_AspNetRoleClaims_RoleId"],
            ["AspNetUserClaims"] = ["IX_AspNetUserClaims_UserId"],
            ["AspNetUserLogins"] = ["IX_AspNetUserLogins_UserId"],
            ["AspNetUserRoles"] = ["IX_AspNetUserRoles_RoleId"],
            ["AspNetUserTokens"] = [],
            ["Patients"] = ["UX_Patients_PatientNumber", "IX_Patients_NameDob", "IX_Patients_Canonical"],
            ["TestDefinitions"] = ["UX_TestDefinitions_Code", "IX_TestDefinitions_CategoryActive"],
            ["TestDefinitionVersions"] = ["UX_TestDefinitionVersions_Number", "IX_TestDefinitionVersions_StatusEffective"],
            ["TestParameterDefinitions"] = ["UX_TestParameterDefinitions_Code", "IX_TestParameterDefinitions_Unit"],
            ["ReferenceIntervals"] = ["IX_ReferenceIntervals_Selection", "IX_ReferenceIntervals_Method", "IX_ReferenceIntervals_Unit"],
            ["ResultValueOptions"] = ["UX_ResultValueOptions_Code"],
            ["Examinations"] = ["UX_Examinations_Number", "IX_Examinations_PatientDate", "IX_Examinations_StatusDate", "IX_Examinations_PriorityDate"],
            ["ExaminationTests"] = ["UX_ExaminationTests_Instance", "IX_ExaminationTests_Status", "IX_ExaminationTests_Version", "IX_ExaminationTests_Method"],
            ["TestResults"] = ["UX_TestResults_TestParameter", "IX_TestResults_CurrentRevision"],
            ["TestResultRevisions"] = ["UX_TestResultRevisions_Number", "IX_TestResultRevisions_StatusDate", "IX_TestResultRevisions_Option", "IX_TestResultRevisions_Unit", "IX_TestResultRevisions_Reference"],
            ["AuditLogs"] = ["IX_AuditLogs_ActorTime", "IX_AuditLogs_EntityTime", "IX_AuditLogs_ActionTime", "IX_AuditLogs_Correlation"]
        };

        foreach (var (table, expectedNames) in expected)
        {
            var entity = context.Model.GetEntityTypes().Single(x => x.GetTableName() == table);
            var actualNames = entity.GetIndexes()
                .Select(index => index.GetDatabaseName())
                .Where(name => name is not null)
                .Order(StringComparer.Ordinal)
                .ToArray();

            Assert.Equal(expectedNames.Order(StringComparer.Ordinal), actualNames);
        }
    }

    [Fact]
    public void Model_contains_the_declared_check_constraints_and_critical_store_types()
    {
        var options = new DbContextOptionsBuilder<LabTrackDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        using var context = new LabTrackDbContext(options);
        var checks = context.Model.GetEntityTypes()
            .SelectMany(entity => entity.GetCheckConstraints())
            .Select(check => check.Name)
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            new[]
            {
                "CK_ReferenceIntervals_Age",
                "CK_ReferenceIntervals_Limits",
                "CK_TestParameterDefinitions_ValueType",
                "CK_TestResultRevisions_ValueType"
            },
            checks);

        AssertColumnType(context, "ReferenceIntervals", "AgeMinimumDays", "int unsigned");
        AssertColumnType(context, "ExaminationTests", "TestInstanceNumber", "smallint unsigned");
        AssertColumnType(context, "TestResultRevisions", "NumericValue", "decimal(18,6)");
        AssertColumnType(context, "Patients", "DateOfBirth", "date");
        AssertColumnType(context, "AuditLogs", "BeforeData", "longtext");
        AssertColumnType(context, "AspNetUsers", "LockoutEnd", "datetime(6)");
    }

    private static void AssertColumnType(
        LabTrackDbContext context,
        string table,
        string propertyName,
        string expectedType)
    {
        var entity = context.Model.GetEntityTypes().Single(x => x.GetTableName() == table);
        var property = entity.FindProperty(propertyName);
        Assert.NotNull(property);
        Assert.Equal(expectedType, property!.GetColumnType());
    }

}
