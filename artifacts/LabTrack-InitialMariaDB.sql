CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `AspNetRoles` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `Name` varchar(256) CHARACTER SET utf8mb4 NULL,
    `NormalizedName` varchar(256) CHARACTER SET utf8mb4 NULL,
    `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetRoles` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `AspNetUsers` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `IsActive` tinyint(1) NOT NULL DEFAULT TRUE,
    `DeactivatedAtUtc` datetime(6) NULL,
    `UserName` varchar(256) CHARACTER SET utf8mb4 NULL,
    `NormalizedUserName` varchar(256) CHARACTER SET utf8mb4 NULL,
    `Email` varchar(256) CHARACTER SET utf8mb4 NULL,
    `NormalizedEmail` varchar(256) CHARACTER SET utf8mb4 NULL,
    `EmailConfirmed` tinyint(1) NOT NULL DEFAULT FALSE,
    `PasswordHash` longtext CHARACTER SET utf8mb4 NULL,
    `SecurityStamp` longtext CHARACTER SET utf8mb4 NULL,
    `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NULL,
    `PhoneNumber` longtext CHARACTER SET utf8mb4 NULL,
    `PhoneNumberConfirmed` tinyint(1) NOT NULL DEFAULT FALSE,
    `TwoFactorEnabled` tinyint(1) NOT NULL DEFAULT FALSE,
    `LockoutEnd` datetime(6) NULL,
    `LockoutEnabled` tinyint(1) NOT NULL DEFAULT TRUE,
    `AccessFailedCount` int NOT NULL DEFAULT 0,
    CONSTRAINT `PK_AspNetUsers` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `TestDefinitions` (
    `TestDefinitionId` bigint NOT NULL AUTO_INCREMENT,
    `TestCategoryId` bigint NOT NULL,
    `TestCode` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `TestName` varchar(180) CHARACTER SET utf8mb4 NOT NULL,
    `Description` varchar(1000) CHARACTER SET utf8mb4 NULL,
    `IsActive` tinyint(1) NOT NULL DEFAULT TRUE,
    `CreatedAtUtc` datetime(6) NOT NULL,
    `UpdatedAtUtc` datetime(6) NULL,
    CONSTRAINT `PK_TestDefinitions` PRIMARY KEY (`TestDefinitionId`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `AspNetRoleClaims` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `RoleId` bigint NOT NULL,
    `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
    `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetRoleClaims` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AspNetRoleClaims_Roles` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `AspNetUserClaims` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `UserId` bigint NOT NULL,
    `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
    `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetUserClaims` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AspNetUserClaims_Users` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `AspNetUserLogins` (
    `LoginProvider` varchar(128) CHARACTER SET utf8mb4 NOT NULL,
    `ProviderKey` varchar(128) CHARACTER SET utf8mb4 NOT NULL,
    `ProviderDisplayName` longtext CHARACTER SET utf8mb4 NULL,
    `UserId` bigint NOT NULL,
    CONSTRAINT `PK_AspNetUserLogins` PRIMARY KEY (`LoginProvider`, `ProviderKey`),
    CONSTRAINT `FK_AspNetUserLogins_Users` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `AspNetUserRoles` (
    `UserId` bigint NOT NULL,
    `RoleId` bigint NOT NULL,
    CONSTRAINT `PK_AspNetUserRoles` PRIMARY KEY (`UserId`, `RoleId`),
    CONSTRAINT `FK_AspNetUserRoles_Roles` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_AspNetUserRoles_Users` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `AspNetUserTokens` (
    `UserId` bigint NOT NULL,
    `LoginProvider` varchar(128) CHARACTER SET utf8mb4 NOT NULL,
    `Name` varchar(128) CHARACTER SET utf8mb4 NOT NULL,
    `Value` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AspNetUserTokens` PRIMARY KEY (`UserId`, `LoginProvider`, `Name`),
    CONSTRAINT `FK_AspNetUserTokens_Users` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `AuditLogs` (
    `AuditLogId` bigint NOT NULL AUTO_INCREMENT,
    `ActorUserId` bigint NULL,
    `ActionType` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `EntityType` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `EntityId` bigint NULL,
    `Description` varchar(2000) CHARACTER SET utf8mb4 NULL,
    `Reason` varchar(1000) CHARACTER SET utf8mb4 NULL,
    `BeforeData` longtext CHARACTER SET utf8mb4 NULL,
    `AfterData` longtext CHARACTER SET utf8mb4 NULL,
    `IpAddress` varchar(45) CHARACTER SET utf8mb4 NULL,
    `UserAgent` varchar(500) CHARACTER SET utf8mb4 NULL,
    `CorrelationId` varchar(100) CHARACTER SET utf8mb4 NULL,
    `OccurredAtUtc` datetime(6) NOT NULL,
    CONSTRAINT `PK_AuditLogs` PRIMARY KEY (`AuditLogId`),
    CONSTRAINT `FK_AuditLogs_Actor` FOREIGN KEY (`ActorUserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `Patients` (
    `PatientId` bigint NOT NULL AUTO_INCREMENT,
    `PatientNumber` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    `FirstName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `MiddleName` varchar(100) CHARACTER SET utf8mb4 NULL,
    `LastName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `Suffix` varchar(20) CHARACTER SET utf8mb4 NULL,
    `DateOfBirth` date NULL,
    `SexAtRegistration` varchar(30) CHARACTER SET utf8mb4 NULL,
    `CivilStatus` varchar(30) CHARACTER SET utf8mb4 NULL,
    `Nationality` varchar(80) CHARACTER SET utf8mb4 NULL,
    `MobileNumber` varchar(30) CHARACTER SET utf8mb4 NULL,
    `EmailAddress` varchar(254) CHARACTER SET utf8mb4 NULL,
    `AddressLine1` varchar(180) CHARACTER SET utf8mb4 NULL,
    `AddressLine2` varchar(180) CHARACTER SET utf8mb4 NULL,
    `Barangay` varchar(100) CHARACTER SET utf8mb4 NULL,
    `CityMunicipality` varchar(100) CHARACTER SET utf8mb4 NULL,
    `Province` varchar(100) CHARACTER SET utf8mb4 NULL,
    `PostalCode` varchar(20) CHARACTER SET utf8mb4 NULL,
    `PatientStatus` varchar(20) CHARACTER SET utf8mb4 NOT NULL DEFAULT 'Active',
    `CanonicalPatientId` bigint NULL,
    `RegisteredAtUtc` datetime(6) NOT NULL,
    `RegisteredByUserId` bigint NOT NULL,
    `UpdatedAtUtc` datetime(6) NULL,
    `UpdatedByUserId` bigint NULL,
    `RowVersion` bigint NOT NULL DEFAULT 1,
    CONSTRAINT `PK_Patients` PRIMARY KEY (`PatientId`),
    CONSTRAINT `FK_Patients_Canonical` FOREIGN KEY (`CanonicalPatientId`) REFERENCES `Patients` (`PatientId`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Patients_RegisteredBy` FOREIGN KEY (`RegisteredByUserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Patients_UpdatedBy` FOREIGN KEY (`UpdatedByUserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `TestDefinitionVersions` (
    `TestDefinitionVersionId` bigint NOT NULL AUTO_INCREMENT,
    `TestDefinitionId` bigint NOT NULL,
    `VersionNumber` int NOT NULL,
    `VersionStatus` varchar(30) CHARACTER SET utf8mb4 NOT NULL,
    `EffectiveFrom` date NULL,
    `EffectiveTo` date NULL,
    `DefaultMethodId` bigint NULL,
    `Instructions` varchar(2000) CHARACTER SET utf8mb4 NULL,
    `PublishedAtUtc` datetime(6) NULL,
    `PublishedByUserId` bigint NULL,
    `CreatedAtUtc` datetime(6) NOT NULL,
    `CreatedByUserId` bigint NOT NULL,
    CONSTRAINT `PK_TestDefinitionVersions` PRIMARY KEY (`TestDefinitionVersionId`),
    CONSTRAINT `FK_TestDefinitionVersions_Creator` FOREIGN KEY (`CreatedByUserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_TestDefinitionVersions_Definition` FOREIGN KEY (`TestDefinitionId`) REFERENCES `TestDefinitions` (`TestDefinitionId`) ON DELETE RESTRICT,
    CONSTRAINT `FK_TestDefinitionVersions_Publisher` FOREIGN KEY (`PublishedByUserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `Examinations` (
    `ExaminationId` bigint NOT NULL AUTO_INCREMENT,
    `ExaminationNumber` varchar(40) CHARACTER SET utf8mb4 NOT NULL,
    `PatientId` bigint NOT NULL,
    `ExaminationDate` date NOT NULL,
    `Priority` varchar(20) CHARACTER SET utf8mb4 NOT NULL DEFAULT 'Routine',
    `Status` varchar(30) CHARACTER SET utf8mb4 NOT NULL DEFAULT 'Registered',
    `Notes` varchar(2000) CHARACTER SET utf8mb4 NULL,
    `CreatedAtUtc` datetime(6) NOT NULL,
    `CreatedByUserId` bigint NOT NULL,
    `RowVersion` bigint NOT NULL DEFAULT 1,
    CONSTRAINT `PK_Examinations` PRIMARY KEY (`ExaminationId`),
    CONSTRAINT `FK_Examinations_Creator` FOREIGN KEY (`CreatedByUserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Examinations_Patient` FOREIGN KEY (`PatientId`) REFERENCES `Patients` (`PatientId`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `TestParameterDefinitions` (
    `TestParameterDefinitionId` bigint NOT NULL AUTO_INCREMENT,
    `TestDefinitionVersionId` bigint NOT NULL,
    `ParameterCode` varchar(60) CHARACTER SET utf8mb4 NOT NULL,
    `ParameterName` varchar(160) CHARACTER SET utf8mb4 NOT NULL,
    `ValueType` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
    `UnitId` bigint NULL,
    `DecimalPlaces` tinyint unsigned NULL,
    `IsRequired` tinyint(1) NOT NULL DEFAULT TRUE,
    `DisplayOrder` int NOT NULL DEFAULT 0,
    `IsActive` tinyint(1) NOT NULL DEFAULT TRUE,
    CONSTRAINT `PK_TestParameterDefinitions` PRIMARY KEY (`TestParameterDefinitionId`),
    CONSTRAINT `CK_TestParameterDefinitions_ValueType` CHECK (ValueType IN ('Numeric','Qualitative','Text','Titer')),
    CONSTRAINT `FK_TestParameterDefinitions_Version` FOREIGN KEY (`TestDefinitionVersionId`) REFERENCES `TestDefinitionVersions` (`TestDefinitionVersionId`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `ExaminationTests` (
    `ExaminationTestId` bigint NOT NULL AUTO_INCREMENT,
    `ExaminationId` bigint NOT NULL,
    `TestDefinitionVersionId` bigint NOT NULL,
    `TestInstanceNumber` smallint unsigned NOT NULL DEFAULT 1,
    `Status` varchar(30) CHARACTER SET utf8mb4 NOT NULL DEFAULT 'Pending',
    `Priority` varchar(20) CHARACTER SET utf8mb4 NOT NULL DEFAULT 'Routine',
    `RequestedAtUtc` datetime(6) NOT NULL,
    `RequestedByUserId` bigint NOT NULL,
    `CompletedAtUtc` datetime(6) NULL,
    `ActualMethodId` bigint NULL,
    CONSTRAINT `PK_ExaminationTests` PRIMARY KEY (`ExaminationTestId`),
    CONSTRAINT `FK_ExaminationTests_Examination` FOREIGN KEY (`ExaminationId`) REFERENCES `Examinations` (`ExaminationId`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ExaminationTests_Requester` FOREIGN KEY (`RequestedByUserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ExaminationTests_Version` FOREIGN KEY (`TestDefinitionVersionId`) REFERENCES `TestDefinitionVersions` (`TestDefinitionVersionId`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `ReferenceIntervals` (
    `ReferenceIntervalId` bigint NOT NULL AUTO_INCREMENT,
    `TestParameterDefinitionId` bigint NOT NULL,
    `SexApplicability` varchar(30) CHARACTER SET utf8mb4 NULL,
    `AgeMinimumDays` int unsigned NULL,
    `AgeMaximumDays` int unsigned NULL,
    `LowerLimit` decimal(18,6) NULL,
    `UpperLimit` decimal(18,6) NULL,
    `ReferenceText` varchar(500) CHARACTER SET utf8mb4 NULL,
    `UnitId` bigint NULL,
    `TestMethodId` bigint NULL,
    `EffectiveFrom` date NULL,
    `EffectiveTo` date NULL,
    `Priority` int NOT NULL DEFAULT 0,
    `IsActive` tinyint(1) NOT NULL DEFAULT TRUE,
    CONSTRAINT `PK_ReferenceIntervals` PRIMARY KEY (`ReferenceIntervalId`),
    CONSTRAINT `CK_ReferenceIntervals_Age` CHECK (AgeMaximumDays IS NULL OR AgeMinimumDays IS NULL OR AgeMaximumDays >= AgeMinimumDays),
    CONSTRAINT `CK_ReferenceIntervals_Limits` CHECK (LowerLimit IS NULL OR UpperLimit IS NULL OR UpperLimit >= LowerLimit),
    CONSTRAINT `FK_ReferenceIntervals_Parameter` FOREIGN KEY (`TestParameterDefinitionId`) REFERENCES `TestParameterDefinitions` (`TestParameterDefinitionId`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `ResultValueOptions` (
    `ResultValueOptionId` bigint NOT NULL AUTO_INCREMENT,
    `TestParameterDefinitionId` bigint NOT NULL,
    `OptionCode` varchar(60) CHARACTER SET utf8mb4 NOT NULL,
    `OptionLabel` varchar(160) CHARACTER SET utf8mb4 NOT NULL,
    `DisplayOrder` int NOT NULL DEFAULT 0,
    `IsAbnormal` tinyint(1) NOT NULL DEFAULT FALSE,
    `IsActive` tinyint(1) NOT NULL DEFAULT TRUE,
    CONSTRAINT `PK_ResultValueOptions` PRIMARY KEY (`ResultValueOptionId`),
    CONSTRAINT `FK_ResultValueOptions_Parameter` FOREIGN KEY (`TestParameterDefinitionId`) REFERENCES `TestParameterDefinitions` (`TestParameterDefinitionId`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `TestResultRevisions` (
    `TestResultRevisionId` bigint NOT NULL AUTO_INCREMENT,
    `TestResultId` bigint NOT NULL,
    `RevisionNumber` int NOT NULL,
    `ValueType` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
    `NumericValue` decimal(18,6) NULL,
    `TextValue` longtext CHARACTER SET utf8mb4 NULL,
    `ResultValueOptionId` bigint NULL,
    `TiterValue` varchar(80) CHARACTER SET utf8mb4 NULL,
    `UnitId` bigint NULL,
    `ReferenceIntervalId` bigint NULL,
    `UnitSnapshot` varchar(120) CHARACTER SET utf8mb4 NULL,
    `ReferenceRangeSnapshot` varchar(500) CHARACTER SET utf8mb4 NULL,
    `MethodSnapshot` varchar(160) CHARACTER SET utf8mb4 NULL,
    `IsAbnormal` tinyint(1) NOT NULL DEFAULT FALSE,
    `IsCritical` tinyint(1) NOT NULL DEFAULT FALSE,
    `Interpretation` varchar(2000) CHARACTER SET utf8mb4 NULL,
    `Status` varchar(30) CHARACTER SET utf8mb4 NOT NULL DEFAULT 'Draft',
    `RevisionReason` varchar(1000) CHARACTER SET utf8mb4 NULL,
    `CreatedAtUtc` datetime(6) NOT NULL,
    `CreatedByUserId` bigint NOT NULL,
    CONSTRAINT `PK_TestResultRevisions` PRIMARY KEY (`TestResultRevisionId`),
    CONSTRAINT `CK_TestResultRevisions_ValueType` CHECK (ValueType IN ('Numeric','Qualitative','Text','Titer')),
    CONSTRAINT `FK_TestResultRevisions_Encoder` FOREIGN KEY (`CreatedByUserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_TestResultRevisions_Option` FOREIGN KEY (`ResultValueOptionId`) REFERENCES `ResultValueOptions` (`ResultValueOptionId`) ON DELETE RESTRICT,
    CONSTRAINT `FK_TestResultRevisions_Reference` FOREIGN KEY (`ReferenceIntervalId`) REFERENCES `ReferenceIntervals` (`ReferenceIntervalId`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `TestResults` (
    `TestResultId` bigint NOT NULL AUTO_INCREMENT,
    `ExaminationTestId` bigint NOT NULL,
    `TestParameterDefinitionId` bigint NOT NULL,
    `CurrentRevisionId` bigint NULL,
    `CreatedAtUtc` datetime(6) NOT NULL,
    `CreatedByUserId` bigint NOT NULL,
    CONSTRAINT `PK_TestResults` PRIMARY KEY (`TestResultId`),
    CONSTRAINT `FK_TestResults_Creator` FOREIGN KEY (`CreatedByUserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_TestResults_CurrentRevision` FOREIGN KEY (`CurrentRevisionId`) REFERENCES `TestResultRevisions` (`TestResultRevisionId`) ON DELETE RESTRICT,
    CONSTRAINT `FK_TestResults_ExaminationTest` FOREIGN KEY (`ExaminationTestId`) REFERENCES `ExaminationTests` (`ExaminationTestId`) ON DELETE RESTRICT,
    CONSTRAINT `FK_TestResults_Parameter` FOREIGN KEY (`TestParameterDefinitionId`) REFERENCES `TestParameterDefinitions` (`TestParameterDefinitionId`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_AspNetRoleClaims_RoleId` ON `AspNetRoleClaims` (`RoleId`);

CREATE UNIQUE INDEX `UX_AspNetRoles_NormalizedName` ON `AspNetRoles` (`NormalizedName`);

CREATE INDEX `IX_AspNetUserClaims_UserId` ON `AspNetUserClaims` (`UserId`);

CREATE INDEX `IX_AspNetUserLogins_UserId` ON `AspNetUserLogins` (`UserId`);

CREATE INDEX `IX_AspNetUserRoles_RoleId` ON `AspNetUserRoles` (`RoleId`);

CREATE INDEX `IX_AspNetUsers_NormalizedEmail` ON `AspNetUsers` (`NormalizedEmail`);

CREATE UNIQUE INDEX `UX_AspNetUsers_NormalizedUserName` ON `AspNetUsers` (`NormalizedUserName`);

CREATE INDEX `IX_AuditLogs_ActionTime` ON `AuditLogs` (`ActionType`, `OccurredAtUtc`);

CREATE INDEX `IX_AuditLogs_ActorTime` ON `AuditLogs` (`ActorUserId`, `OccurredAtUtc`);

CREATE INDEX `IX_AuditLogs_Correlation` ON `AuditLogs` (`CorrelationId`);

CREATE INDEX `IX_AuditLogs_EntityTime` ON `AuditLogs` (`EntityType`, `EntityId`, `OccurredAtUtc`);

CREATE INDEX `IX_Examinations_CreatedByUserId` ON `Examinations` (`CreatedByUserId`);

CREATE INDEX `IX_Examinations_PatientDate` ON `Examinations` (`PatientId`, `ExaminationDate`);

CREATE INDEX `IX_Examinations_PriorityDate` ON `Examinations` (`Priority`, `ExaminationDate`);

CREATE INDEX `IX_Examinations_StatusDate` ON `Examinations` (`Status`, `ExaminationDate`);

CREATE UNIQUE INDEX `UX_Examinations_Number` ON `Examinations` (`ExaminationNumber`);

CREATE INDEX `IX_ExaminationTests_Method` ON `ExaminationTests` (`ActualMethodId`);

CREATE INDEX `IX_ExaminationTests_RequestedByUserId` ON `ExaminationTests` (`RequestedByUserId`);

CREATE INDEX `IX_ExaminationTests_Status` ON `ExaminationTests` (`Status`, `Priority`);

CREATE INDEX `IX_ExaminationTests_Version` ON `ExaminationTests` (`TestDefinitionVersionId`);

CREATE UNIQUE INDEX `UX_ExaminationTests_Instance` ON `ExaminationTests` (`ExaminationId`, `TestDefinitionVersionId`, `TestInstanceNumber`);

CREATE INDEX `IX_Patients_Canonical` ON `Patients` (`CanonicalPatientId`);

CREATE INDEX `IX_Patients_NameDob` ON `Patients` (`LastName`, `FirstName`, `DateOfBirth`);

CREATE INDEX `IX_Patients_RegisteredByUserId` ON `Patients` (`RegisteredByUserId`);

CREATE INDEX `IX_Patients_UpdatedByUserId` ON `Patients` (`UpdatedByUserId`);

CREATE UNIQUE INDEX `UX_Patients_PatientNumber` ON `Patients` (`PatientNumber`);

CREATE INDEX `IX_ReferenceIntervals_Method` ON `ReferenceIntervals` (`TestMethodId`);

CREATE INDEX `IX_ReferenceIntervals_Selection` ON `ReferenceIntervals` (`TestParameterDefinitionId`, `IsActive`, `EffectiveFrom`, `EffectiveTo`);

CREATE INDEX `IX_ReferenceIntervals_Unit` ON `ReferenceIntervals` (`UnitId`);

CREATE UNIQUE INDEX `UX_ResultValueOptions_Code` ON `ResultValueOptions` (`TestParameterDefinitionId`, `OptionCode`);

CREATE INDEX `IX_TestDefinitions_CategoryActive` ON `TestDefinitions` (`TestCategoryId`, `IsActive`);

CREATE UNIQUE INDEX `UX_TestDefinitions_Code` ON `TestDefinitions` (`TestCode`);

CREATE INDEX `IX_TestDefinitionVersions_CreatedByUserId` ON `TestDefinitionVersions` (`CreatedByUserId`);

CREATE INDEX `IX_TestDefinitionVersions_PublishedByUserId` ON `TestDefinitionVersions` (`PublishedByUserId`);

CREATE INDEX `IX_TestDefinitionVersions_StatusEffective` ON `TestDefinitionVersions` (`VersionStatus`, `EffectiveFrom`);

CREATE UNIQUE INDEX `UX_TestDefinitionVersions_Number` ON `TestDefinitionVersions` (`TestDefinitionId`, `VersionNumber`);

CREATE INDEX `IX_TestParameterDefinitions_Unit` ON `TestParameterDefinitions` (`UnitId`);

CREATE UNIQUE INDEX `UX_TestParameterDefinitions_Code` ON `TestParameterDefinitions` (`TestDefinitionVersionId`, `ParameterCode`);

CREATE INDEX `IX_TestResultRevisions_CreatedByUserId` ON `TestResultRevisions` (`CreatedByUserId`);

CREATE INDEX `IX_TestResultRevisions_Option` ON `TestResultRevisions` (`ResultValueOptionId`);

CREATE INDEX `IX_TestResultRevisions_Reference` ON `TestResultRevisions` (`ReferenceIntervalId`);

CREATE INDEX `IX_TestResultRevisions_StatusDate` ON `TestResultRevisions` (`Status`, `CreatedAtUtc`);

CREATE INDEX `IX_TestResultRevisions_Unit` ON `TestResultRevisions` (`UnitId`);

CREATE UNIQUE INDEX `UX_TestResultRevisions_Number` ON `TestResultRevisions` (`TestResultId`, `RevisionNumber`);

CREATE INDEX `IX_TestResults_CreatedByUserId` ON `TestResults` (`CreatedByUserId`);

CREATE INDEX `IX_TestResults_CurrentRevision` ON `TestResults` (`CurrentRevisionId`);

CREATE INDEX `IX_TestResults_TestParameterDefinitionId` ON `TestResults` (`TestParameterDefinitionId`);

CREATE UNIQUE INDEX `UX_TestResults_TestParameter` ON `TestResults` (`ExaminationTestId`, `TestParameterDefinitionId`);

ALTER TABLE `TestResultRevisions` ADD CONSTRAINT `FK_TestResultRevisions_Result` FOREIGN KEY (`TestResultId`) REFERENCES `TestResults` (`TestResultId`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261105030931_InitialMariaDbSchema', '9.0.14');

COMMIT;

