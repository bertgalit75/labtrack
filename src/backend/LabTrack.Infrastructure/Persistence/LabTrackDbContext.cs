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
        builder.Entity<ApplicationUser>().ToTable("AspNetUsers");
        builder.Entity<IdentityRole<long>>().ToTable("AspNetRoles");
        builder.Entity<IdentityUserRole<long>>().ToTable("AspNetUserRoles");
        builder.Entity<IdentityUserClaim<long>>().ToTable("AspNetUserClaims");
        builder.Entity<IdentityUserLogin<long>>().ToTable("AspNetUserLogins");
        builder.Entity<IdentityRoleClaim<long>>().ToTable("AspNetRoleClaims");
        builder.Entity<IdentityUserToken<long>>().ToTable("AspNetUserTokens");

        builder.Entity<Patient>().ToTable("Patients").HasKey(x => x.PatientId);
        builder.Entity<Patient>().Property(x => x.PatientId).ValueGeneratedOnAdd();
        builder.Entity<Patient>().Property(x => x.PatientNumber).HasMaxLength(32).IsRequired();
        builder.Entity<Patient>().HasIndex(x => x.PatientNumber).IsUnique();
        builder.Entity<Patient>().Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        builder.Entity<Patient>().Property(x => x.MiddleName).HasMaxLength(100);
        builder.Entity<Patient>().Property(x => x.LastName).HasMaxLength(100).IsRequired();
        builder.Entity<Patient>().Property(x => x.Suffix).HasMaxLength(20);
        builder.Entity<Patient>().Property(x => x.SexAtRegistration).HasMaxLength(30);
        builder.Entity<Patient>().Property(x => x.CivilStatus).HasMaxLength(30);
        builder.Entity<Patient>().Property(x => x.Nationality).HasMaxLength(80);
        builder.Entity<Patient>().Property(x => x.MobileNumber).HasMaxLength(30);
        builder.Entity<Patient>().Property(x => x.EmailAddress).HasMaxLength(254);
        builder.Entity<Patient>().Property(x => x.AddressLine1).HasMaxLength(180);
        builder.Entity<Patient>().Property(x => x.AddressLine2).HasMaxLength(180);
        builder.Entity<Patient>().Property(x => x.Barangay).HasMaxLength(100);
        builder.Entity<Patient>().Property(x => x.CityMunicipality).HasMaxLength(100);
        builder.Entity<Patient>().Property(x => x.Province).HasMaxLength(100);
        builder.Entity<Patient>().Property(x => x.PostalCode).HasMaxLength(20);
        builder.Entity<Patient>().Property(x => x.PatientStatus).HasMaxLength(20).HasDefaultValue("Active").IsRequired();
        builder.Entity<Patient>().Property(x => x.RowVersion).IsConcurrencyToken();
        builder.Entity<Patient>().HasIndex(x => new { x.LastName, x.FirstName, x.DateOfBirth });
        builder.Entity<Patient>().HasIndex(x => x.CanonicalPatientId);
        builder.Entity<Patient>().HasOne<Patient>().WithMany().HasForeignKey(x => x.CanonicalPatientId).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<AuditLog>().ToTable("AuditLogs").HasKey(x => x.AuditLogId);
        builder.Entity<AuditLog>().Property(x => x.ActionType).HasMaxLength(100).IsRequired();
        builder.Entity<AuditLog>().Property(x => x.EntityType).HasMaxLength(100).IsRequired();
        builder.Entity<AuditLog>().Property(x => x.Description).HasMaxLength(2000);
        builder.Entity<AuditLog>().Property(x => x.Reason).HasMaxLength(1000);

        builder.Entity<TestDefinition>().ToTable("TestDefinitions").HasKey(x => x.TestDefinitionId);
        builder.Entity<TestDefinition>().Property(x => x.TestCode).HasMaxLength(50).IsRequired();
        builder.Entity<TestDefinition>().Property(x => x.TestName).HasMaxLength(180).IsRequired();
        builder.Entity<TestDefinitionVersion>().ToTable("TestDefinitionVersions").HasKey(x => x.TestDefinitionVersionId);
        builder.Entity<TestParameterDefinition>().ToTable("TestParameterDefinitions").HasKey(x => x.TestParameterDefinitionId);
        builder.Entity<TestParameterDefinition>().Property(x => x.ValueType).HasMaxLength(20).IsRequired();
        builder.Entity<ReferenceInterval>().ToTable("ReferenceIntervals").HasKey(x => x.ReferenceIntervalId);
        builder.Entity<ReferenceInterval>().Property(x => x.LowerLimit).HasPrecision(18, 6);
        builder.Entity<ReferenceInterval>().Property(x => x.UpperLimit).HasPrecision(18, 6);
        builder.Entity<ResultValueOption>().ToTable("ResultValueOptions").HasKey(x => x.ResultValueOptionId);
        builder.Entity<Examination>().ToTable("Examinations").HasKey(x => x.ExaminationId);
        builder.Entity<Examination>().Property(x => x.ExaminationNumber).HasMaxLength(40).IsRequired();
        builder.Entity<ExaminationTest>().ToTable("ExaminationTests").HasKey(x => x.ExaminationTestId);
        builder.Entity<TestResult>().ToTable("TestResults").HasKey(x => x.TestResultId);
        builder.Entity<TestResultRevision>().ToTable("TestResultRevisions").HasKey(x => x.TestResultRevisionId);
        builder.Entity<TestResultRevision>().Property(x => x.NumericValue).HasPrecision(18, 6);
        // This initial mapping is a scaffold only. Exact column/type/constraint mapping must be reconciled
        // against SDD-03 and the consolidated SQL, then tested on disposable MariaDB 10.11.
    }
}
