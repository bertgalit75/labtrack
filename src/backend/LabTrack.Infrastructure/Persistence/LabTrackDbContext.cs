using LabTrack.Domain.Entities;
using LabTrack.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LabTrack.Infrastructure.Persistence;

public sealed class LabTrackDbContext(DbContextOptions<LabTrackDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<long>, long>(options)
{
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<TestDefinition> TestDefinitions => Set<TestDefinition>();
    public DbSet<TestDefinitionVersion> TestDefinitionVersions => Set<TestDefinitionVersion>();
    public DbSet<TestParameterDefinition> TestParameterDefinitions => Set<TestParameterDefinition>();
    public DbSet<ReferenceInterval> ReferenceIntervals => Set<ReferenceInterval>();
    public DbSet<ResultValueOption> ResultValueOptions => Set<ResultValueOption>();
    public DbSet<Examination> Examinations => Set<Examination>();
    public DbSet<ExaminationTest> ExaminationTests => Set<ExaminationTest>();
    public DbSet<TestResult> TestResults => Set<TestResult>();
    public DbSet<TestResultRevision> TestResultRevisions => Set<TestResultRevision>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // The SQL baseline intentionally contains the seven classic Identity tables only.
        // EF Core 10 Identity schema v3 adds AspNetUserPasskeys; keep that feature outside ADR-001 scope.
        builder.Ignore<IdentityUserPasskey<long>>();
        ConfigureIdentity(builder);
        ConfigureApplicationModel(builder);
    }

    private static void ConfigureIdentity(ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>(b =>
        {
            b.ToTable("AspNetUsers");
            b.Property(x => x.Id).HasColumnName("Id").HasColumnType("bigint").ValueGeneratedOnAdd();
            b.Property(x => x.UserName).HasColumnType("varchar(256)").HasMaxLength(256);
            b.Property(x => x.NormalizedUserName).HasColumnType("varchar(256)").HasMaxLength(256);
            b.Property(x => x.Email).HasColumnType("varchar(256)").HasMaxLength(256);
            b.Property(x => x.NormalizedEmail).HasColumnType("varchar(256)").HasMaxLength(256);
            b.Property(x => x.EmailConfirmed).HasColumnType("tinyint(1)").HasDefaultValue(false);
            b.Property(x => x.PasswordHash).HasColumnType("longtext");
            b.Property(x => x.SecurityStamp).HasColumnType("longtext");
            b.Property(x => x.ConcurrencyStamp).HasColumnType("longtext");
            b.Property(x => x.PhoneNumber).HasColumnType("longtext");
            b.Property(x => x.PhoneNumberConfirmed).HasColumnType("tinyint(1)").HasDefaultValue(false);
            b.Property(x => x.TwoFactorEnabled).HasColumnType("tinyint(1)").HasDefaultValue(false);
            b.Property(x => x.LockoutEnd).HasColumnType("datetime(6)");
            b.Property(x => x.LockoutEnabled).HasColumnType("tinyint(1)").HasDefaultValue(true);
            b.Property(x => x.AccessFailedCount).HasColumnType("int").HasDefaultValue(0);
            b.Property(x => x.IsActive).HasColumnType("tinyint(1)").HasDefaultValue(true);
            b.Property(x => x.DeactivatedAtUtc).HasColumnType("datetime(6)");
            b.HasIndex(x => x.NormalizedUserName).IsUnique().HasDatabaseName("UX_AspNetUsers_NormalizedUserName");
            b.HasIndex(x => x.NormalizedEmail).HasDatabaseName("IX_AspNetUsers_NormalizedEmail");
        });

        builder.Entity<IdentityRole<long>>(b =>
        {
            b.ToTable("AspNetRoles");
            b.Property(x => x.Id).HasColumnType("bigint").ValueGeneratedOnAdd();
            b.Property(x => x.Name).HasColumnType("varchar(256)").HasMaxLength(256);
            b.Property(x => x.NormalizedName).HasColumnType("varchar(256)").HasMaxLength(256);
            b.Property(x => x.ConcurrencyStamp).HasColumnType("longtext");
            b.HasIndex(x => x.NormalizedName).IsUnique().HasDatabaseName("UX_AspNetRoles_NormalizedName");
        });

        builder.Entity<IdentityUserClaim<long>>(b =>
        {
            b.ToTable("AspNetUserClaims");
            b.Property(x => x.Id).HasColumnType("bigint").ValueGeneratedOnAdd();
            b.Property(x => x.ClaimType).HasColumnType("longtext");
            b.Property(x => x.ClaimValue).HasColumnType("longtext");
            b.HasIndex(x => x.UserId).HasDatabaseName("IX_AspNetUserClaims_UserId");
            b.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_AspNetUserClaims_Users");
        });
        builder.Entity<IdentityRoleClaim<long>>(b =>
        {
            b.ToTable("AspNetRoleClaims");
            b.Property(x => x.Id).HasColumnType("bigint").ValueGeneratedOnAdd();
            b.Property(x => x.ClaimType).HasColumnType("longtext");
            b.Property(x => x.ClaimValue).HasColumnType("longtext");
            b.HasIndex(x => x.RoleId).HasDatabaseName("IX_AspNetRoleClaims_RoleId");
            b.HasOne<IdentityRole<long>>().WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_AspNetRoleClaims_Roles");
        });
        builder.Entity<IdentityUserLogin<long>>(b =>
        {
            b.ToTable("AspNetUserLogins");
            b.Property(x => x.LoginProvider).HasColumnType("varchar(128)").HasMaxLength(128);
            b.Property(x => x.ProviderKey).HasColumnType("varchar(128)").HasMaxLength(128);
            b.Property(x => x.ProviderDisplayName).HasColumnType("longtext");
            b.HasKey(x => new { x.LoginProvider, x.ProviderKey });
            b.HasIndex(x => x.UserId).HasDatabaseName("IX_AspNetUserLogins_UserId");
            b.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_AspNetUserLogins_Users");
        });
        builder.Entity<IdentityUserRole<long>>(b =>
        {
            b.ToTable("AspNetUserRoles");
            b.HasKey(x => new { x.UserId, x.RoleId });
            b.HasIndex(x => x.RoleId).HasDatabaseName("IX_AspNetUserRoles_RoleId");
            b.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_AspNetUserRoles_Users");
            b.HasOne<IdentityRole<long>>().WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_AspNetUserRoles_Roles");
        });
        builder.Entity<IdentityUserToken<long>>(b =>
        {
            b.ToTable("AspNetUserTokens");
            b.Property(x => x.LoginProvider).HasColumnType("varchar(128)").HasMaxLength(128);
            b.Property(x => x.Name).HasColumnType("varchar(128)").HasMaxLength(128);
            b.Property(x => x.Value).HasColumnType("longtext");
            b.HasKey(x => new { x.UserId, x.LoginProvider, x.Name });
            b.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_AspNetUserTokens_Users");
        });
    }

    private static void ConfigureApplicationModel(ModelBuilder b)
    {
        b.Entity<Patient>(e =>
        {
            e.ToTable("Patients"); e.HasKey(x => x.PatientId);
            e.Property(x => x.PatientId).HasColumnType("bigint").ValueGeneratedOnAdd();
            e.Property(x => x.PatientNumber).HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
            e.Property(x => x.FirstName).HasColumnType("varchar(100)").HasMaxLength(100).IsRequired();
            e.Property(x => x.MiddleName).HasColumnType("varchar(100)").HasMaxLength(100);
            e.Property(x => x.LastName).HasColumnType("varchar(100)").HasMaxLength(100).IsRequired();
            e.Property(x => x.Suffix).HasColumnType("varchar(20)").HasMaxLength(20);
            e.Property(x => x.DateOfBirth).HasColumnType("date");
            e.Property(x => x.SexAtRegistration).HasColumnType("varchar(30)").HasMaxLength(30);
            e.Property(x => x.CivilStatus).HasColumnType("varchar(30)").HasMaxLength(30);
            e.Property(x => x.Nationality).HasColumnType("varchar(80)").HasMaxLength(80);
            e.Property(x => x.MobileNumber).HasColumnType("varchar(30)").HasMaxLength(30);
            e.Property(x => x.EmailAddress).HasColumnType("varchar(254)").HasMaxLength(254);
            e.Property(x => x.AddressLine1).HasColumnType("varchar(180)").HasMaxLength(180);
            e.Property(x => x.AddressLine2).HasColumnType("varchar(180)").HasMaxLength(180);
            e.Property(x => x.Barangay).HasColumnType("varchar(100)").HasMaxLength(100);
            e.Property(x => x.CityMunicipality).HasColumnType("varchar(100)").HasMaxLength(100);
            e.Property(x => x.Province).HasColumnType("varchar(100)").HasMaxLength(100);
            e.Property(x => x.PostalCode).HasColumnType("varchar(20)").HasMaxLength(20);
            e.Property(x => x.PatientStatus).HasColumnType("varchar(20)").HasMaxLength(20).IsRequired().HasDefaultValue("Active");
            e.Property(x => x.CanonicalPatientId).HasColumnType("bigint");
            e.Property(x => x.RegisteredAtUtc).HasColumnType("datetime(6)");
            e.Property(x => x.RegisteredByUserId).HasColumnType("bigint");
            e.Property(x => x.UpdatedAtUtc).HasColumnType("datetime(6)");
            e.Property(x => x.UpdatedByUserId).HasColumnType("bigint");
            e.Property(x => x.RowVersion).HasColumnType("bigint").HasDefaultValue(1L);
            e.HasIndex(x => x.PatientNumber).IsUnique().HasDatabaseName("UX_Patients_PatientNumber");
            e.HasIndex(x => new { x.LastName, x.FirstName, x.DateOfBirth }).HasDatabaseName("IX_Patients_NameDob");
            e.HasIndex(x => x.CanonicalPatientId).HasDatabaseName("IX_Patients_Canonical");
            e.HasOne<Patient>().WithMany().HasForeignKey(x => x.CanonicalPatientId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Patients_Canonical");
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.RegisteredByUserId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Patients_RegisteredBy");
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UpdatedByUserId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Patients_UpdatedBy");
        });

        b.Entity<TestDefinition>(e =>
        {
            e.ToTable("TestDefinitions"); e.HasKey(x => x.TestDefinitionId);
            e.Property(x => x.TestDefinitionId).HasColumnType("bigint").ValueGeneratedOnAdd();
            e.Property(x => x.TestCategoryId).HasColumnType("bigint");
            e.Property(x => x.TestCode).HasColumnType("varchar(50)").HasMaxLength(50).IsRequired();
            e.Property(x => x.TestName).HasColumnType("varchar(180)").HasMaxLength(180).IsRequired();
            e.Property(x => x.Description).HasColumnType("varchar(1000)").HasMaxLength(1000);
            e.Property(x => x.IsActive).HasColumnType("tinyint(1)").HasDefaultValue(true);
            e.Property(x => x.CreatedAtUtc).HasColumnType("datetime(6)");
            e.Property(x => x.UpdatedAtUtc).HasColumnType("datetime(6)");
            e.HasIndex(x => x.TestCode).IsUnique().HasDatabaseName("UX_TestDefinitions_Code");
            e.HasIndex(x => new { x.TestCategoryId, x.IsActive }).HasDatabaseName("IX_TestDefinitions_CategoryActive");
        });

        b.Entity<TestDefinitionVersion>(e =>
        {
            e.ToTable("TestDefinitionVersions"); e.HasKey(x => x.TestDefinitionVersionId);
            e.Property(x => x.TestDefinitionVersionId).HasColumnType("bigint").ValueGeneratedOnAdd();
            e.Property(x => x.TestDefinitionId).HasColumnType("bigint");
            e.Property(x => x.VersionNumber).HasColumnType("int");
            e.Property(x => x.VersionStatus).HasColumnType("varchar(30)").HasMaxLength(30).IsRequired();
            e.Property(x => x.EffectiveFrom).HasColumnType("date"); e.Property(x => x.EffectiveTo).HasColumnType("date");
            e.Property(x => x.DefaultMethodId).HasColumnType("bigint");
            e.Property(x => x.Instructions).HasColumnType("varchar(2000)").HasMaxLength(2000);
            e.Property(x => x.PublishedAtUtc).HasColumnType("datetime(6)");
            e.Property(x => x.PublishedByUserId).HasColumnType("bigint");
            e.Property(x => x.CreatedAtUtc).HasColumnType("datetime(6)"); e.Property(x => x.CreatedByUserId).HasColumnType("bigint");
            e.HasIndex(x => new { x.TestDefinitionId, x.VersionNumber }).IsUnique().HasDatabaseName("UX_TestDefinitionVersions_Number");
            e.HasIndex(x => new { x.VersionStatus, x.EffectiveFrom }).HasDatabaseName("IX_TestDefinitionVersions_StatusEffective");
            e.HasOne<TestDefinition>().WithMany().HasForeignKey(x => x.TestDefinitionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_TestDefinitionVersions_Definition");
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.PublishedByUserId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_TestDefinitionVersions_Publisher");
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_TestDefinitionVersions_Creator");
        });

        b.Entity<TestParameterDefinition>(e =>
        {
            e.ToTable("TestParameterDefinitions", t => t.HasCheckConstraint("CK_TestParameterDefinitions_ValueType", "ValueType IN ('Numeric','Qualitative','Text','Titer')"));
            e.HasKey(x => x.TestParameterDefinitionId);
            e.Property(x => x.TestParameterDefinitionId).HasColumnType("bigint").ValueGeneratedOnAdd();
            e.Property(x => x.TestDefinitionVersionId).HasColumnType("bigint");
            e.Property(x => x.ParameterCode).HasColumnType("varchar(60)").HasMaxLength(60).IsRequired();
            e.Property(x => x.ParameterName).HasColumnType("varchar(160)").HasMaxLength(160).IsRequired();
            e.Property(x => x.ValueType).HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
            e.Property(x => x.UnitId).HasColumnType("bigint");
            e.Property(x => x.DecimalPlaces).HasColumnType("tinyint unsigned");
            e.Property(x => x.IsRequired).HasColumnType("tinyint(1)").HasDefaultValue(true);
            e.Property(x => x.DisplayOrder).HasColumnType("int").HasDefaultValue(0);
            e.Property(x => x.IsActive).HasColumnType("tinyint(1)").HasDefaultValue(true);
            e.HasIndex(x => new { x.TestDefinitionVersionId, x.ParameterCode }).IsUnique().HasDatabaseName("UX_TestParameterDefinitions_Code");
            e.HasIndex(x => x.UnitId).HasDatabaseName("IX_TestParameterDefinitions_Unit");
            e.HasOne<TestDefinitionVersion>().WithMany().HasForeignKey(x => x.TestDefinitionVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_TestParameterDefinitions_Version");
        });

        b.Entity<ReferenceInterval>(e =>
        {
            e.ToTable("ReferenceIntervals", t =>
            {
                t.HasCheckConstraint("CK_ReferenceIntervals_Age", "AgeMaximumDays IS NULL OR AgeMinimumDays IS NULL OR AgeMaximumDays >= AgeMinimumDays");
                t.HasCheckConstraint("CK_ReferenceIntervals_Limits", "LowerLimit IS NULL OR UpperLimit IS NULL OR UpperLimit >= LowerLimit");
            });
            e.HasKey(x => x.ReferenceIntervalId);
            e.Property(x => x.ReferenceIntervalId).HasColumnType("bigint").ValueGeneratedOnAdd();
            e.Property(x => x.TestParameterDefinitionId).HasColumnType("bigint");
            e.Property(x => x.SexApplicability).HasColumnType("varchar(30)").HasMaxLength(30);
            e.Property(x => x.AgeMinimumDays).HasColumnType("int unsigned");
            e.Property(x => x.AgeMaximumDays).HasColumnType("int unsigned");
            e.Property(x => x.LowerLimit).HasColumnType("decimal(18,6)").HasPrecision(18, 6);
            e.Property(x => x.UpperLimit).HasColumnType("decimal(18,6)").HasPrecision(18, 6);
            e.Property(x => x.ReferenceText).HasColumnType("varchar(500)").HasMaxLength(500);
            e.Property(x => x.UnitId).HasColumnType("bigint"); e.Property(x => x.TestMethodId).HasColumnType("bigint");
            e.Property(x => x.EffectiveFrom).HasColumnType("date"); e.Property(x => x.EffectiveTo).HasColumnType("date");
            e.Property(x => x.Priority).HasColumnType("int").HasDefaultValue(0);
            e.Property(x => x.IsActive).HasColumnType("tinyint(1)").HasDefaultValue(true);
            e.HasIndex(x => new { x.TestParameterDefinitionId, x.IsActive, x.EffectiveFrom, x.EffectiveTo }).HasDatabaseName("IX_ReferenceIntervals_Selection");
            e.HasIndex(x => x.TestMethodId).HasDatabaseName("IX_ReferenceIntervals_Method");
            e.HasIndex(x => x.UnitId).HasDatabaseName("IX_ReferenceIntervals_Unit");
            e.HasOne<TestParameterDefinition>().WithMany().HasForeignKey(x => x.TestParameterDefinitionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_ReferenceIntervals_Parameter");
        });

        b.Entity<ResultValueOption>(e =>
        {
            e.ToTable("ResultValueOptions"); e.HasKey(x => x.ResultValueOptionId);
            e.Property(x => x.ResultValueOptionId).HasColumnType("bigint").ValueGeneratedOnAdd();
            e.Property(x => x.TestParameterDefinitionId).HasColumnType("bigint");
            e.Property(x => x.OptionCode).HasColumnType("varchar(60)").HasMaxLength(60).IsRequired();
            e.Property(x => x.OptionLabel).HasColumnType("varchar(160)").HasMaxLength(160).IsRequired();
            e.Property(x => x.DisplayOrder).HasColumnType("int").HasDefaultValue(0);
            e.Property(x => x.IsAbnormal).HasColumnType("tinyint(1)").HasDefaultValue(false);
            e.Property(x => x.IsActive).HasColumnType("tinyint(1)").HasDefaultValue(true);
            e.HasIndex(x => new { x.TestParameterDefinitionId, x.OptionCode }).IsUnique().HasDatabaseName("UX_ResultValueOptions_Code");
            e.HasOne<TestParameterDefinition>().WithMany().HasForeignKey(x => x.TestParameterDefinitionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_ResultValueOptions_Parameter");
        });

        b.Entity<Examination>(e =>
        {
            e.ToTable("Examinations"); e.HasKey(x => x.ExaminationId);
            e.Property(x => x.ExaminationId).HasColumnType("bigint").ValueGeneratedOnAdd();
            e.Property(x => x.ExaminationNumber).HasColumnType("varchar(40)").HasMaxLength(40).IsRequired();
            e.Property(x => x.PatientId).HasColumnType("bigint");
            e.Property(x => x.ExaminationDate).HasColumnType("date");
            e.Property(x => x.Priority).HasColumnType("varchar(20)").HasMaxLength(20).IsRequired().HasDefaultValue("Routine");
            e.Property(x => x.Status).HasColumnType("varchar(30)").HasMaxLength(30).IsRequired().HasDefaultValue("Registered");
            e.Property(x => x.Notes).HasColumnType("varchar(2000)").HasMaxLength(2000);
            e.Property(x => x.CreatedAtUtc).HasColumnType("datetime(6)"); e.Property(x => x.CreatedByUserId).HasColumnType("bigint");
            e.Property(x => x.RowVersion).HasColumnType("bigint").HasDefaultValue(1L);
            e.HasIndex(x => x.ExaminationNumber).IsUnique().HasDatabaseName("UX_Examinations_Number");
            e.HasIndex(x => new { x.PatientId, x.ExaminationDate }).HasDatabaseName("IX_Examinations_PatientDate");
            e.HasIndex(x => new { x.Status, x.ExaminationDate }).HasDatabaseName("IX_Examinations_StatusDate");
            e.HasIndex(x => new { x.Priority, x.ExaminationDate }).HasDatabaseName("IX_Examinations_PriorityDate");
            e.HasOne<Patient>().WithMany().HasForeignKey(x => x.PatientId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Examinations_Patient");
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Examinations_Creator");
        });

        b.Entity<ExaminationTest>(e =>
        {
            e.ToTable("ExaminationTests"); e.HasKey(x => x.ExaminationTestId);
            e.Property(x => x.ExaminationTestId).HasColumnType("bigint").ValueGeneratedOnAdd();
            e.Property(x => x.ExaminationId).HasColumnType("bigint"); e.Property(x => x.TestDefinitionVersionId).HasColumnType("bigint");
            e.Property(x => x.TestInstanceNumber).HasColumnType("smallint unsigned").HasDefaultValue((ushort)1);
            e.Property(x => x.Status).HasColumnType("varchar(30)").HasMaxLength(30).IsRequired().HasDefaultValue("Pending");
            e.Property(x => x.Priority).HasColumnType("varchar(20)").HasMaxLength(20).IsRequired().HasDefaultValue("Routine");
            e.Property(x => x.RequestedAtUtc).HasColumnType("datetime(6)"); e.Property(x => x.RequestedByUserId).HasColumnType("bigint");
            e.Property(x => x.CompletedAtUtc).HasColumnType("datetime(6)"); e.Property(x => x.ActualMethodId).HasColumnType("bigint");
            e.HasIndex(x => new { x.ExaminationId, x.TestDefinitionVersionId, x.TestInstanceNumber }).IsUnique().HasDatabaseName("UX_ExaminationTests_Instance");
            e.HasIndex(x => new { x.Status, x.Priority }).HasDatabaseName("IX_ExaminationTests_Status");
            e.HasIndex(x => x.TestDefinitionVersionId).HasDatabaseName("IX_ExaminationTests_Version");
            e.HasIndex(x => x.ActualMethodId).HasDatabaseName("IX_ExaminationTests_Method");
            e.HasOne<Examination>().WithMany().HasForeignKey(x => x.ExaminationId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_ExaminationTests_Examination");
            e.HasOne<TestDefinitionVersion>().WithMany().HasForeignKey(x => x.TestDefinitionVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_ExaminationTests_Version");
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.RequestedByUserId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_ExaminationTests_Requester");
        });

        b.Entity<TestResult>(e =>
        {
            e.ToTable("TestResults"); e.HasKey(x => x.TestResultId);
            e.Property(x => x.TestResultId).HasColumnType("bigint").ValueGeneratedOnAdd();
            e.Property(x => x.ExaminationTestId).HasColumnType("bigint"); e.Property(x => x.TestParameterDefinitionId).HasColumnType("bigint");
            e.Property(x => x.CurrentRevisionId).HasColumnType("bigint"); e.Property(x => x.CreatedAtUtc).HasColumnType("datetime(6)");
            e.Property(x => x.CreatedByUserId).HasColumnType("bigint");
            e.HasIndex(x => new { x.ExaminationTestId, x.TestParameterDefinitionId }).IsUnique().HasDatabaseName("UX_TestResults_TestParameter");
            e.HasIndex(x => x.CurrentRevisionId).HasDatabaseName("IX_TestResults_CurrentRevision");
            e.HasOne<ExaminationTest>().WithMany().HasForeignKey(x => x.ExaminationTestId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_TestResults_ExaminationTest");
            e.HasOne<TestParameterDefinition>().WithMany().HasForeignKey(x => x.TestParameterDefinitionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_TestResults_Parameter");
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_TestResults_Creator");
        });

        b.Entity<TestResultRevision>(e =>
        {
            e.ToTable("TestResultRevisions", t => t.HasCheckConstraint("CK_TestResultRevisions_ValueType", "ValueType IN ('Numeric','Qualitative','Text','Titer')"));
            e.HasKey(x => x.TestResultRevisionId);
            e.Property(x => x.TestResultRevisionId).HasColumnType("bigint").ValueGeneratedOnAdd();
            e.Property(x => x.TestResultId).HasColumnType("bigint"); e.Property(x => x.RevisionNumber).HasColumnType("int");
            e.Property(x => x.ValueType).HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
            e.Property(x => x.NumericValue).HasColumnType("decimal(18,6)").HasPrecision(18, 6);
            e.Property(x => x.TextValue).HasColumnType("longtext");
            e.Property(x => x.ResultValueOptionId).HasColumnType("bigint"); e.Property(x => x.TiterValue).HasColumnType("varchar(80)").HasMaxLength(80);
            e.Property(x => x.UnitId).HasColumnType("bigint"); e.Property(x => x.ReferenceIntervalId).HasColumnType("bigint");
            e.Property(x => x.UnitSnapshot).HasColumnType("varchar(120)").HasMaxLength(120);
            e.Property(x => x.ReferenceRangeSnapshot).HasColumnType("varchar(500)").HasMaxLength(500);
            e.Property(x => x.MethodSnapshot).HasColumnType("varchar(160)").HasMaxLength(160);
            e.Property(x => x.IsAbnormal).HasColumnType("tinyint(1)").HasDefaultValue(false);
            e.Property(x => x.IsCritical).HasColumnType("tinyint(1)").HasDefaultValue(false);
            e.Property(x => x.Interpretation).HasColumnType("varchar(2000)").HasMaxLength(2000);
            e.Property(x => x.Status).HasColumnType("varchar(30)").HasMaxLength(30).IsRequired().HasDefaultValue("Draft");
            e.Property(x => x.RevisionReason).HasColumnType("varchar(1000)").HasMaxLength(1000);
            e.Property(x => x.CreatedAtUtc).HasColumnType("datetime(6)"); e.Property(x => x.CreatedByUserId).HasColumnType("bigint");
            e.HasIndex(x => new { x.TestResultId, x.RevisionNumber }).IsUnique().HasDatabaseName("UX_TestResultRevisions_Number");
            e.HasIndex(x => new { x.Status, x.CreatedAtUtc }).HasDatabaseName("IX_TestResultRevisions_StatusDate");
            e.HasIndex(x => x.ResultValueOptionId).HasDatabaseName("IX_TestResultRevisions_Option");
            e.HasIndex(x => x.UnitId).HasDatabaseName("IX_TestResultRevisions_Unit");
            e.HasIndex(x => x.ReferenceIntervalId).HasDatabaseName("IX_TestResultRevisions_Reference");
            e.HasOne<TestResult>().WithMany().HasForeignKey(x => x.TestResultId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_TestResultRevisions_Result");
            e.HasOne<ResultValueOption>().WithMany().HasForeignKey(x => x.ResultValueOptionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_TestResultRevisions_Option");
            e.HasOne<ReferenceInterval>().WithMany().HasForeignKey(x => x.ReferenceIntervalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_TestResultRevisions_Reference");
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_TestResultRevisions_Encoder");
        });

        b.Entity<TestResult>().HasOne<TestResultRevision>().WithMany().HasForeignKey(x => x.CurrentRevisionId)
            .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_TestResults_CurrentRevision");

        b.Entity<AuditLog>(e =>
        {
            e.ToTable("AuditLogs"); e.HasKey(x => x.AuditLogId);
            e.Property(x => x.AuditLogId).HasColumnType("bigint").ValueGeneratedOnAdd();
            e.Property(x => x.ActorUserId).HasColumnType("bigint");
            e.Property(x => x.ActionType).HasColumnType("varchar(100)").HasMaxLength(100).IsRequired();
            e.Property(x => x.EntityType).HasColumnType("varchar(100)").HasMaxLength(100).IsRequired();
            e.Property(x => x.EntityId).HasColumnType("bigint");
            e.Property(x => x.Description).HasColumnType("varchar(2000)").HasMaxLength(2000);
            e.Property(x => x.Reason).HasColumnType("varchar(1000)").HasMaxLength(1000);
            e.Property(x => x.BeforeData).HasColumnType("longtext"); e.Property(x => x.AfterData).HasColumnType("longtext");
            e.Property(x => x.IpAddress).HasColumnType("varchar(45)").HasMaxLength(45);
            e.Property(x => x.UserAgent).HasColumnType("varchar(500)").HasMaxLength(500);
            e.Property(x => x.CorrelationId).HasColumnType("varchar(100)").HasMaxLength(100);
            e.Property(x => x.OccurredAtUtc).HasColumnType("datetime(6)");
            e.HasIndex(x => new { x.ActorUserId, x.OccurredAtUtc }).HasDatabaseName("IX_AuditLogs_ActorTime");
            e.HasIndex(x => new { x.EntityType, x.EntityId, x.OccurredAtUtc }).HasDatabaseName("IX_AuditLogs_EntityTime");
            e.HasIndex(x => new { x.ActionType, x.OccurredAtUtc }).HasDatabaseName("IX_AuditLogs_ActionTime");
            e.HasIndex(x => x.CorrelationId).HasDatabaseName("IX_AuditLogs_Correlation");
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.ActorUserId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_AuditLogs_Actor");
        });
    }
}
