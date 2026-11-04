# SDD-03 — Data Dictionary and Database Constraints

**Version:** 0.2 — Reconciled against consolidated SQL (static review)
**Status:** Approved documentation baseline — MariaDB runtime validation outstanding
**Release:** 1
**Review date:** 2026-11-03

## 1. Purpose and review boundary

This revision records the column and constraint definitions found in `database/LabTrack_Consolidated_Install.sql`. It is a static source reconciliation, not a claim that the SQL has been successfully executed by MariaDB. The SQL remains a review baseline. No schema changes, database execution, or application implementation are authorized by this document.

## 2. Source and method

- Source file: `database/LabTrack_Consolidated_Install.sql`.
- Parsed `CREATE TABLE` statements and the deferred `ALTER TABLE` foreign-key statement.
- Column definitions below are transcribed from SQL declaration text; table-level keys and constraints are listed separately.
- The SQL contains seven numbered batches and 48 `CREATE TABLE` statements.
- Counts below are static counts from this file; they are not `SHOW CREATE TABLE` runtime counts.

## 3. Static inventory summary

| Inventory item | Count |
|---|---:|
| Tables | 48 |
| Columns | 428 |
| Primary-key declarations | 48 |
| Unique-key declarations | 33 |
| Secondary index declarations | 58 |
| Foreign-key declarations (including deferred ALTER) | 98 |
| CHECK constraint declarations | 8 |

Every table has one declared primary key. The table-by-table declarations follow.

## 4. Column dictionary and declared constraints

Legend: `NN` = NOT NULL; `NULL` = nullable; defaults and AUTO_INCREMENT are shown verbatim where present. Constraint declarations are source-defined only when explicitly present in the SQL.

### AspNetRoles

| Column | SQL declaration |
|---|---|
| `Id` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `Name` | `VARCHAR(256) NULL` |
| `NormalizedName` | `VARCHAR(256) NULL` |
| `ConcurrencyStamp` | `LONGTEXT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (Id)`
- `UNIQUE KEY UX_AspNetRoles_NormalizedName (NormalizedName)`

### AspNetUsers

| Column | SQL declaration |
|---|---|
| `Id` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `UserName` | `VARCHAR(256) NULL` |
| `NormalizedUserName` | `VARCHAR(256) NULL` |
| `Email` | `VARCHAR(256) NULL` |
| `NormalizedEmail` | `VARCHAR(256) NULL` |
| `EmailConfirmed` | `TINYINT(1) NOT NULL DEFAULT 0` |
| `PasswordHash` | `LONGTEXT NULL` |
| `SecurityStamp` | `LONGTEXT NULL` |
| `ConcurrencyStamp` | `LONGTEXT NULL` |
| `PhoneNumber` | `LONGTEXT NULL` |
| `PhoneNumberConfirmed` | `TINYINT(1) NOT NULL DEFAULT 0` |
| `TwoFactorEnabled` | `TINYINT(1) NOT NULL DEFAULT 0` |
| `LockoutEnd` | `DATETIME(6) NULL` |
| `LockoutEnabled` | `TINYINT(1) NOT NULL DEFAULT 1` |
| `AccessFailedCount` | `INT NOT NULL DEFAULT 0` |
| `IsActive` | `TINYINT(1) NOT NULL DEFAULT 1` |
| `DeactivatedAtUtc` | `DATETIME(6) NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (Id)`
- `UNIQUE KEY UX_AspNetUsers_NormalizedUserName (NormalizedUserName)`
- `KEY IX_AspNetUsers_NormalizedEmail (NormalizedEmail)`

### AspNetRoleClaims

| Column | SQL declaration |
|---|---|
| `Id` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `RoleId` | `BIGINT NOT NULL` |
| `ClaimType` | `LONGTEXT NULL` |
| `ClaimValue` | `LONGTEXT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (Id)`
- `KEY IX_AspNetRoleClaims_RoleId (RoleId)`
- `CONSTRAINT FK_AspNetRoleClaims_Roles FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE RESTRICT`

### AspNetUserClaims

| Column | SQL declaration |
|---|---|
| `Id` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `UserId` | `BIGINT NOT NULL` |
| `ClaimType` | `LONGTEXT NULL` |
| `ClaimValue` | `LONGTEXT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (Id)`
- `KEY IX_AspNetUserClaims_UserId (UserId)`
- `CONSTRAINT FK_AspNetUserClaims_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`

### AspNetUserLogins

| Column | SQL declaration |
|---|---|
| `LoginProvider` | `VARCHAR(128) NOT NULL` |
| `ProviderKey` | `VARCHAR(128) NOT NULL` |
| `ProviderDisplayName` | `LONGTEXT NULL` |
| `UserId` | `BIGINT NOT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (LoginProvider, ProviderKey)`
- `KEY IX_AspNetUserLogins_UserId (UserId)`
- `CONSTRAINT FK_AspNetUserLogins_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`

### AspNetUserRoles

| Column | SQL declaration |
|---|---|
| `UserId` | `BIGINT NOT NULL` |
| `RoleId` | `BIGINT NOT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (UserId, RoleId)`
- `KEY IX_AspNetUserRoles_RoleId (RoleId)`
- `CONSTRAINT FK_AspNetUserRoles_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`
- `CONSTRAINT FK_AspNetUserRoles_Roles FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE RESTRICT`

### AspNetUserTokens

| Column | SQL declaration |
|---|---|
| `UserId` | `BIGINT NOT NULL` |
| `LoginProvider` | `VARCHAR(128) NOT NULL` |
| `Name` | `VARCHAR(128) NOT NULL` |
| `Value` | `LONGTEXT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (UserId, LoginProvider, Name)`
- `CONSTRAINT FK_AspNetUserTokens_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`

### StaffProfiles

| Column | SQL declaration |
|---|---|
| `StaffProfileId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `UserId` | `BIGINT NOT NULL` |
| `EmployeeNumber` | `VARCHAR(40) NULL` |
| `FirstName` | `VARCHAR(100) NOT NULL` |
| `MiddleName` | `VARCHAR(100) NULL` |
| `LastName` | `VARCHAR(100) NOT NULL` |
| `Suffix` | `VARCHAR(20) NULL` |
| `DisplayName` | `VARCHAR(220) NOT NULL` |
| `JobTitle` | `VARCHAR(120) NULL` |
| `IsActive` | `TINYINT(1) NOT NULL DEFAULT 1` |
| `CreatedAtUtc` | `DATETIME(6) NOT NULL` |
| `UpdatedAtUtc` | `DATETIME(6) NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (StaffProfileId)`
- `UNIQUE KEY UX_StaffProfiles_UserId (UserId)`
- `UNIQUE KEY UX_StaffProfiles_EmployeeNumber (EmployeeNumber)`
- `CONSTRAINT FK_StaffProfiles_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`

### Permissions

| Column | SQL declaration |
|---|---|
| `PermissionId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `PermissionCode` | `VARCHAR(120) NOT NULL` |
| `PermissionName` | `VARCHAR(160) NOT NULL` |
| `Description` | `VARCHAR(500) NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (PermissionId)`
- `UNIQUE KEY UX_Permissions_Code (PermissionCode)`

### RolePermissions

| Column | SQL declaration |
|---|---|
| `RoleId` | `BIGINT NOT NULL` |
| `PermissionId` | `BIGINT NOT NULL` |
| `GrantedAtUtc` | `DATETIME(6) NOT NULL` |
| `GrantedByUserId` | `BIGINT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (RoleId, PermissionId)`
- `KEY IX_RolePermissions_PermissionId (PermissionId)`
- `CONSTRAINT FK_RolePermissions_Roles FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE RESTRICT`
- `CONSTRAINT FK_RolePermissions_Permissions FOREIGN KEY (PermissionId) REFERENCES Permissions(PermissionId) ON DELETE RESTRICT`
- `CONSTRAINT FK_RolePermissions_GrantedBy FOREIGN KEY (GrantedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`

### UserPermissions

| Column | SQL declaration |
|---|---|
| `UserPermissionId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `UserId` | `BIGINT NOT NULL` |
| `PermissionId` | `BIGINT NOT NULL` |
| `Effect` | `VARCHAR(10) NOT NULL` |
| `Reason` | `VARCHAR(500) NOT NULL` |
| `GrantedAtUtc` | `DATETIME(6) NOT NULL` |
| `GrantedByUserId` | `BIGINT NOT NULL` |
| `ExpiresAtUtc` | `DATETIME(6) NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (UserPermissionId)`
- `UNIQUE KEY UX_UserPermissions_User_Permission (UserId, PermissionId)`
- `KEY IX_UserPermissions_PermissionId (PermissionId)`
- `CONSTRAINT FK_UserPermissions_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`
- `CONSTRAINT FK_UserPermissions_Permissions FOREIGN KEY (PermissionId) REFERENCES Permissions(PermissionId) ON DELETE RESTRICT`
- `CONSTRAINT FK_UserPermissions_GrantedBy FOREIGN KEY (GrantedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`
- `CONSTRAINT CK_UserPermissions_Effect CHECK (Effect IN ('Allow','Deny'))`

### ProfessionalCredentials

| Column | SQL declaration |
|---|---|
| `ProfessionalCredentialId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `StaffProfileId` | `BIGINT NOT NULL` |
| `CredentialType` | `VARCHAR(80) NOT NULL` |
| `CredentialNumber` | `VARCHAR(80) NOT NULL` |
| `IssuingAuthority` | `VARCHAR(160) NULL` |
| `IssuedOn` | `DATE NULL` |
| `ExpiresOn` | `DATE NULL` |
| `Status` | `VARCHAR(30) NOT NULL` |
| `IsActive` | `TINYINT(1) NOT NULL DEFAULT 1` |
| `CreatedAtUtc` | `DATETIME(6) NOT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (ProfessionalCredentialId)`
- `KEY IX_ProfessionalCredentials_Staff (StaffProfileId)`
- `KEY IX_ProfessionalCredentials_StatusDates (Status, ExpiresOn)`
- `CONSTRAINT FK_ProfessionalCredentials_Staff FOREIGN KEY (StaffProfileId) REFERENCES StaffProfiles(StaffProfileId) ON DELETE RESTRICT`

### StaffAuthorizations

| Column | SQL declaration |
|---|---|
| `StaffAuthorizationId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `StaffProfileId` | `BIGINT NOT NULL` |
| `AuthorizationType` | `VARCHAR(100) NOT NULL` |
| `ScopeCode` | `VARCHAR(100) NOT NULL` |
| `AuthorizedByStaffProfileId` | `BIGINT NULL` |
| `EffectiveFrom` | `DATE NOT NULL` |
| `EffectiveTo` | `DATE NULL` |
| `Status` | `VARCHAR(30) NOT NULL` |
| `EvidenceFileId` | `BIGINT NULL` |
| `Notes` | `VARCHAR(1000) NULL` |
| `CreatedAtUtc` | `DATETIME(6) NOT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (StaffAuthorizationId)`
- `KEY IX_StaffAuthorizations_StaffDates (StaffProfileId, EffectiveFrom, EffectiveTo)`
- `KEY IX_StaffAuthorizations_AuthorizedBy (AuthorizedByStaffProfileId)`
- `CONSTRAINT FK_StaffAuthorizations_Staff FOREIGN KEY (StaffProfileId) REFERENCES StaffProfiles(StaffProfileId) ON DELETE RESTRICT`
- `CONSTRAINT FK_StaffAuthorizations_AuthorizedBy FOREIGN KEY (AuthorizedByStaffProfileId) REFERENCES StaffProfiles(StaffProfileId) ON DELETE RESTRICT`

### Patients

| Column | SQL declaration |
|---|---|
| `PatientId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `PatientNumber` | `VARCHAR(32) NOT NULL` |
| `FirstName` | `VARCHAR(100) NOT NULL` |
| `MiddleName` | `VARCHAR(100) NULL` |
| `LastName` | `VARCHAR(100) NOT NULL` |
| `Suffix` | `VARCHAR(20) NULL` |
| `DateOfBirth` | `DATE NULL` |
| `SexAtRegistration` | `VARCHAR(30) NULL` |
| `CivilStatus` | `VARCHAR(30) NULL` |
| `Nationality` | `VARCHAR(80) NULL` |
| `MobileNumber` | `VARCHAR(30) NULL` |
| `EmailAddress` | `VARCHAR(254) NULL` |
| `AddressLine1` | `VARCHAR(180) NULL` |
| `AddressLine2` | `VARCHAR(180) NULL` |
| `Barangay` | `VARCHAR(100) NULL` |
| `CityMunicipality` | `VARCHAR(100) NULL` |
| `Province` | `VARCHAR(100) NULL` |
| `PostalCode` | `VARCHAR(20) NULL` |
| `PatientStatus` | `VARCHAR(20) NOT NULL DEFAULT 'Active'` |
| `CanonicalPatientId` | `BIGINT NULL` |
| `RegisteredAtUtc` | `DATETIME(6) NOT NULL` |
| `RegisteredByUserId` | `BIGINT NOT NULL` |
| `UpdatedAtUtc` | `DATETIME(6) NULL` |
| `UpdatedByUserId` | `BIGINT NULL` |
| `RowVersion` | `BIGINT NOT NULL DEFAULT 1` |

**Declared keys and constraints**

- `PRIMARY KEY (PatientId)`
- `UNIQUE KEY UX_Patients_PatientNumber (PatientNumber)`
- `KEY IX_Patients_NameDob (LastName, FirstName, DateOfBirth)`
- `KEY IX_Patients_Canonical (CanonicalPatientId)`
- `CONSTRAINT FK_Patients_Canonical FOREIGN KEY (CanonicalPatientId) REFERENCES Patients(PatientId) ON DELETE RESTRICT`
- `CONSTRAINT FK_Patients_RegisteredBy FOREIGN KEY (RegisteredByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`
- `CONSTRAINT FK_Patients_UpdatedBy FOREIGN KEY (UpdatedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`

### PatientMatchingRules

| Column | SQL declaration |
|---|---|
| `PatientMatchingRuleId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `RuleCode` | `VARCHAR(80) NOT NULL` |
| `RuleName` | `VARCHAR(120) NOT NULL` |
| `Weight` | `DECIMAL(8,4) NOT NULL` |
| `IsActive` | `TINYINT(1) NOT NULL DEFAULT 1` |
| `ConfigurationJson` | `LONGTEXT NULL` |
| `CreatedAtUtc` | `DATETIME(6) NOT NULL` |
| `UpdatedAtUtc` | `DATETIME(6) NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (PatientMatchingRuleId)`
- `UNIQUE KEY UX_PatientMatchingRules_Code (RuleCode)`

### PatientDuplicateCandidates

| Column | SQL declaration |
|---|---|
| `PatientDuplicateCandidateId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `PatientId1` | `BIGINT NOT NULL` |
| `PatientId2` | `BIGINT NOT NULL` |
| `MatchScore` | `DECIMAL(8,4) NOT NULL` |
| `MatchedByRuleSummary` | `VARCHAR(1000) NULL` |
| `Status` | `VARCHAR(30) NOT NULL` |
| `FirstDetectedAtUtc` | `DATETIME(6) NOT NULL` |
| `LastDetectedAtUtc` | `DATETIME(6) NOT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (PatientDuplicateCandidateId)`
- `UNIQUE KEY UX_PatientDuplicateCandidates_Pair (PatientId1, PatientId2)`
- `KEY IX_PatientDuplicateCandidates_Status (Status, LastDetectedAtUtc)`
- `CONSTRAINT FK_PatientDuplicateCandidates_P1 FOREIGN KEY (PatientId1) REFERENCES Patients(PatientId) ON DELETE RESTRICT`
- `CONSTRAINT FK_PatientDuplicateCandidates_P2 FOREIGN KEY (PatientId2) REFERENCES Patients(PatientId) ON DELETE RESTRICT`
- `CONSTRAINT CK_PatientDuplicateCandidates_Order CHECK (PatientId1 < PatientId2)`

### PatientDuplicateReviews

| Column | SQL declaration |
|---|---|
| `PatientDuplicateReviewId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `PatientDuplicateCandidateId` | `BIGINT NOT NULL` |
| `Decision` | `VARCHAR(30) NOT NULL` |
| `ReviewNotes` | `VARCHAR(1000) NULL` |
| `ReviewedByUserId` | `BIGINT NOT NULL` |
| `ReviewedAtUtc` | `DATETIME(6) NOT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (PatientDuplicateReviewId)`
- `KEY IX_PatientDuplicateReviews_CandidateDate (PatientDuplicateCandidateId, ReviewedAtUtc)`
- `CONSTRAINT FK_PatientDuplicateReviews_Candidate FOREIGN KEY (PatientDuplicateCandidateId) REFERENCES PatientDuplicateCandidates(PatientDuplicateCandidateId) ON DELETE RESTRICT`
- `CONSTRAINT FK_PatientDuplicateReviews_Reviewer FOREIGN KEY (ReviewedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`

### PatientMergeEvents

| Column | SQL declaration |
|---|---|
| `PatientMergeEventId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `SourcePatientId` | `BIGINT NOT NULL` |
| `DestinationPatientId` | `BIGINT NOT NULL` |
| `MergeReason` | `VARCHAR(1000) NOT NULL` |
| `RequestedByUserId` | `BIGINT NOT NULL` |
| `RequestedAtUtc` | `DATETIME(6) NOT NULL` |
| `ApprovedByUserId` | `BIGINT NULL` |
| `ApprovedAtUtc` | `DATETIME(6) NULL` |
| `ExecutedByUserId` | `BIGINT NULL` |
| `ExecutedAtUtc` | `DATETIME(6) NULL` |
| `ReversedByUserId` | `BIGINT NULL` |
| `ReversedAtUtc` | `DATETIME(6) NULL` |
| `ReversalReason` | `VARCHAR(1000) NULL` |
| `Status` | `VARCHAR(30) NOT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (PatientMergeEventId)`
- `KEY IX_PatientMergeEvents_Source (SourcePatientId)`
- `KEY IX_PatientMergeEvents_Destination (DestinationPatientId)`
- `CONSTRAINT FK_PatientMergeEvents_Source FOREIGN KEY (SourcePatientId) REFERENCES Patients(PatientId) ON DELETE RESTRICT`
- `CONSTRAINT FK_PatientMergeEvents_Destination FOREIGN KEY (DestinationPatientId) REFERENCES Patients(PatientId) ON DELETE RESTRICT`
- `CONSTRAINT FK_PatientMergeEvents_Requester FOREIGN KEY (RequestedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`
- `CONSTRAINT FK_PatientMergeEvents_Approver FOREIGN KEY (ApprovedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`
- `CONSTRAINT FK_PatientMergeEvents_Executor FOREIGN KEY (ExecutedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`
- `CONSTRAINT FK_PatientMergeEvents_Reverser FOREIGN KEY (ReversedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`
- `CONSTRAINT CK_PatientMergeEvents_Different CHECK (SourcePatientId <> DestinationPatientId)`

### PatientFieldChangePolicies

| Column | SQL declaration |
|---|---|
| `PatientFieldChangePolicyId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `FieldCode` | `VARCHAR(80) NOT NULL` |
| `RequiresApproval` | `TINYINT(1) NOT NULL DEFAULT 0` |
| `IsActive` | `TINYINT(1) NOT NULL DEFAULT 1` |
| `EffectiveFrom` | `DATE NOT NULL` |
| `EffectiveTo` | `DATE NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (PatientFieldChangePolicyId)`
- `UNIQUE KEY UX_PatientFieldChangePolicies_FieldFrom (FieldCode, EffectiveFrom)`

### PatientChangeRequests

| Column | SQL declaration |
|---|---|
| `PatientChangeRequestId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `PatientId` | `BIGINT NOT NULL` |
| `FieldCode` | `VARCHAR(80) NOT NULL` |
| `OldValue` | `LONGTEXT NULL` |
| `ProposedValue` | `LONGTEXT NULL` |
| `Reason` | `VARCHAR(1000) NOT NULL` |
| `RequestedByUserId` | `BIGINT NOT NULL` |
| `RequestedAtUtc` | `DATETIME(6) NOT NULL` |
| `Status` | `VARCHAR(30) NOT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (PatientChangeRequestId)`
- `KEY IX_PatientChangeRequests_PatientStatus (PatientId, Status)`
- `CONSTRAINT FK_PatientChangeRequests_Patient FOREIGN KEY (PatientId) REFERENCES Patients(PatientId) ON DELETE RESTRICT`
- `CONSTRAINT FK_PatientChangeRequests_Requester FOREIGN KEY (RequestedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`

### PatientChangeApprovals

| Column | SQL declaration |
|---|---|
| `PatientChangeApprovalId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `PatientChangeRequestId` | `BIGINT NOT NULL` |
| `Decision` | `VARCHAR(30) NOT NULL` |
| `Comments` | `VARCHAR(1000) NULL` |
| `ApprovedByUserId` | `BIGINT NOT NULL` |
| `ApprovedAtUtc` | `DATETIME(6) NOT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (PatientChangeApprovalId)`
- `UNIQUE KEY UX_PatientChangeApprovals_Request (PatientChangeRequestId)`
- `CONSTRAINT FK_PatientChangeApprovals_Request FOREIGN KEY (PatientChangeRequestId) REFERENCES PatientChangeRequests(PatientChangeRequestId) ON DELETE RESTRICT`
- `CONSTRAINT FK_PatientChangeApprovals_Approver FOREIGN KEY (ApprovedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`

### PatientDemographicHistory

| Column | SQL declaration |
|---|---|
| `PatientDemographicHistoryId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `PatientId` | `BIGINT NOT NULL` |
| `FieldCode` | `VARCHAR(80) NOT NULL` |
| `OldValue` | `LONGTEXT NULL` |
| `NewValue` | `LONGTEXT NULL` |
| `ChangeReason` | `VARCHAR(1000) NULL` |
| `ChangedByUserId` | `BIGINT NOT NULL` |
| `ChangedAtUtc` | `DATETIME(6) NOT NULL` |
| `PatientChangeRequestId` | `BIGINT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (PatientDemographicHistoryId)`
- `KEY IX_PatientDemographicHistory_PatientDate (PatientId, ChangedAtUtc)`
- `CONSTRAINT FK_PatientDemographicHistory_Patient FOREIGN KEY (PatientId) REFERENCES Patients(PatientId) ON DELETE RESTRICT`
- `CONSTRAINT FK_PatientDemographicHistory_Actor FOREIGN KEY (ChangedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`
- `CONSTRAINT FK_PatientDemographicHistory_Request FOREIGN KEY (PatientChangeRequestId) REFERENCES PatientChangeRequests(PatientChangeRequestId) ON DELETE RESTRICT`

### TestCategories

| Column | SQL declaration |
|---|---|
| `TestCategoryId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `CategoryCode` | `VARCHAR(50) NOT NULL` |
| `CategoryName` | `VARCHAR(120) NOT NULL` |
| `Description` | `VARCHAR(500) NULL` |
| `DisplayOrder` | `INT NOT NULL DEFAULT 0` |
| `IsActive` | `TINYINT(1) NOT NULL DEFAULT 1` |

**Declared keys and constraints**

- `PRIMARY KEY (TestCategoryId)`
- `UNIQUE KEY UX_TestCategories_Code (CategoryCode)`
- `UNIQUE KEY UX_TestCategories_Name (CategoryName)`

### TestMethods

| Column | SQL declaration |
|---|---|
| `TestMethodId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `MethodCode` | `VARCHAR(50) NOT NULL` |
| `MethodName` | `VARCHAR(160) NOT NULL` |
| `Description` | `VARCHAR(500) NULL` |
| `IsActive` | `TINYINT(1) NOT NULL DEFAULT 1` |

**Declared keys and constraints**

- `PRIMARY KEY (TestMethodId)`
- `UNIQUE KEY UX_TestMethods_Code (MethodCode)`

### Units

| Column | SQL declaration |
|---|---|
| `UnitId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `UnitCode` | `VARCHAR(50) NOT NULL` |
| `UnitName` | `VARCHAR(120) NOT NULL` |
| `Symbol` | `VARCHAR(40) NULL` |
| `IsActive` | `TINYINT(1) NOT NULL DEFAULT 1` |

**Declared keys and constraints**

- `PRIMARY KEY (UnitId)`
- `UNIQUE KEY UX_Units_Code (UnitCode)`

### TestDefinitions

| Column | SQL declaration |
|---|---|
| `TestDefinitionId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `TestCategoryId` | `BIGINT NOT NULL` |
| `TestCode` | `VARCHAR(50) NOT NULL` |
| `TestName` | `VARCHAR(180) NOT NULL` |
| `Description` | `VARCHAR(1000) NULL` |
| `IsActive` | `TINYINT(1) NOT NULL DEFAULT 1` |
| `CreatedAtUtc` | `DATETIME(6) NOT NULL` |
| `UpdatedAtUtc` | `DATETIME(6) NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (TestDefinitionId)`
- `UNIQUE KEY UX_TestDefinitions_Code (TestCode)`
- `KEY IX_TestDefinitions_CategoryActive (TestCategoryId, IsActive)`
- `CONSTRAINT FK_TestDefinitions_Category FOREIGN KEY (TestCategoryId) REFERENCES TestCategories(TestCategoryId) ON DELETE RESTRICT`

### TestDefinitionVersions

| Column | SQL declaration |
|---|---|
| `TestDefinitionVersionId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `TestDefinitionId` | `BIGINT NOT NULL` |
| `VersionNumber` | `INT NOT NULL` |
| `VersionStatus` | `VARCHAR(30) NOT NULL` |
| `EffectiveFrom` | `DATE NULL` |
| `EffectiveTo` | `DATE NULL` |
| `DefaultMethodId` | `BIGINT NULL` |
| `Instructions` | `VARCHAR(2000) NULL` |
| `PublishedAtUtc` | `DATETIME(6) NULL` |
| `PublishedByUserId` | `BIGINT NULL` |
| `CreatedAtUtc` | `DATETIME(6) NOT NULL` |
| `CreatedByUserId` | `BIGINT NOT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (TestDefinitionVersionId)`
- `UNIQUE KEY UX_TestDefinitionVersions_Number (TestDefinitionId, VersionNumber)`
- `KEY IX_TestDefinitionVersions_StatusEffective (VersionStatus, EffectiveFrom)`
- `CONSTRAINT FK_TestDefinitionVersions_Definition FOREIGN KEY (TestDefinitionId) REFERENCES TestDefinitions(TestDefinitionId) ON DELETE RESTRICT`
- `CONSTRAINT FK_TestDefinitionVersions_DefaultMethod FOREIGN KEY (DefaultMethodId) REFERENCES TestMethods(TestMethodId) ON DELETE RESTRICT`
- `CONSTRAINT FK_TestDefinitionVersions_Publisher FOREIGN KEY (PublishedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`
- `CONSTRAINT FK_TestDefinitionVersions_Creator FOREIGN KEY (CreatedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`

### TestParameterDefinitions

| Column | SQL declaration |
|---|---|
| `TestParameterDefinitionId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `TestDefinitionVersionId` | `BIGINT NOT NULL` |
| `ParameterCode` | `VARCHAR(60) NOT NULL` |
| `ParameterName` | `VARCHAR(160) NOT NULL` |
| `ValueType` | `VARCHAR(20) NOT NULL` |
| `UnitId` | `BIGINT NULL` |
| `DecimalPlaces` | `TINYINT UNSIGNED NULL` |
| `IsRequired` | `TINYINT(1) NOT NULL DEFAULT 1` |
| `DisplayOrder` | `INT NOT NULL DEFAULT 0` |
| `IsActive` | `TINYINT(1) NOT NULL DEFAULT 1` |

**Declared keys and constraints**

- `PRIMARY KEY (TestParameterDefinitionId)`
- `UNIQUE KEY UX_TestParameterDefinitions_Code (TestDefinitionVersionId, ParameterCode)`
- `KEY IX_TestParameterDefinitions_Unit (UnitId)`
- `CONSTRAINT FK_TestParameterDefinitions_Version FOREIGN KEY (TestDefinitionVersionId) REFERENCES TestDefinitionVersions(TestDefinitionVersionId) ON DELETE RESTRICT`
- `CONSTRAINT FK_TestParameterDefinitions_Unit FOREIGN KEY (UnitId) REFERENCES Units(UnitId) ON DELETE RESTRICT`
- `CONSTRAINT CK_TestParameterDefinitions_ValueType CHECK (ValueType IN ('Numeric','Qualitative','Text','Titer'))`

### ReferenceIntervals

| Column | SQL declaration |
|---|---|
| `ReferenceIntervalId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `TestParameterDefinitionId` | `BIGINT NOT NULL` |
| `SexApplicability` | `VARCHAR(30) NULL` |
| `AgeMinimumDays` | `INT UNSIGNED NULL` |
| `AgeMaximumDays` | `INT UNSIGNED NULL` |
| `LowerLimit` | `DECIMAL(18,6) NULL` |
| `UpperLimit` | `DECIMAL(18,6) NULL` |
| `ReferenceText` | `VARCHAR(500) NULL` |
| `UnitId` | `BIGINT NULL` |
| `TestMethodId` | `BIGINT NULL` |
| `EffectiveFrom` | `DATE NULL` |
| `EffectiveTo` | `DATE NULL` |
| `Priority` | `INT NOT NULL DEFAULT 0` |
| `IsActive` | `TINYINT(1) NOT NULL DEFAULT 1` |

**Declared keys and constraints**

- `PRIMARY KEY (ReferenceIntervalId)`
- `KEY IX_ReferenceIntervals_Selection (TestParameterDefinitionId, IsActive, EffectiveFrom, EffectiveTo)`
- `KEY IX_ReferenceIntervals_Method (TestMethodId)`
- `KEY IX_ReferenceIntervals_Unit (UnitId)`
- `CONSTRAINT FK_ReferenceIntervals_Parameter FOREIGN KEY (TestParameterDefinitionId) REFERENCES TestParameterDefinitions(TestParameterDefinitionId) ON DELETE RESTRICT`
- `CONSTRAINT FK_ReferenceIntervals_Unit FOREIGN KEY (UnitId) REFERENCES Units(UnitId) ON DELETE RESTRICT`
- `CONSTRAINT FK_ReferenceIntervals_Method FOREIGN KEY (TestMethodId) REFERENCES TestMethods(TestMethodId) ON DELETE RESTRICT`
- `CONSTRAINT CK_ReferenceIntervals_Age CHECK (AgeMaximumDays IS NULL OR AgeMinimumDays IS NULL OR AgeMaximumDays >= AgeMinimumDays)`
- `CONSTRAINT CK_ReferenceIntervals_Limits CHECK (LowerLimit IS NULL OR UpperLimit IS NULL OR UpperLimit >= LowerLimit)`

### ResultValueOptions

| Column | SQL declaration |
|---|---|
| `ResultValueOptionId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `TestParameterDefinitionId` | `BIGINT NOT NULL` |
| `OptionCode` | `VARCHAR(60) NOT NULL` |
| `OptionLabel` | `VARCHAR(160) NOT NULL` |
| `DisplayOrder` | `INT NOT NULL DEFAULT 0` |
| `IsAbnormal` | `TINYINT(1) NOT NULL DEFAULT 0` |
| `IsActive` | `TINYINT(1) NOT NULL DEFAULT 1` |

**Declared keys and constraints**

- `PRIMARY KEY (ResultValueOptionId)`
- `UNIQUE KEY UX_ResultValueOptions_Code (TestParameterDefinitionId, OptionCode)`
- `CONSTRAINT FK_ResultValueOptions_Parameter FOREIGN KEY (TestParameterDefinitionId) REFERENCES TestParameterDefinitions(TestParameterDefinitionId) ON DELETE RESTRICT`

### Examinations

| Column | SQL declaration |
|---|---|
| `ExaminationId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `ExaminationNumber` | `VARCHAR(40) NOT NULL` |
| `PatientId` | `BIGINT NOT NULL` |
| `ExaminationDate` | `DATE NOT NULL` |
| `Priority` | `VARCHAR(20) NOT NULL DEFAULT 'Routine'` |
| `Status` | `VARCHAR(30) NOT NULL DEFAULT 'Registered'` |
| `Notes` | `VARCHAR(2000) NULL` |
| `CreatedAtUtc` | `DATETIME(6) NOT NULL` |
| `CreatedByUserId` | `BIGINT NOT NULL` |
| `RowVersion` | `BIGINT NOT NULL DEFAULT 1` |

**Declared keys and constraints**

- `PRIMARY KEY (ExaminationId)`
- `UNIQUE KEY UX_Examinations_Number (ExaminationNumber)`
- `KEY IX_Examinations_PatientDate (PatientId, ExaminationDate)`
- `KEY IX_Examinations_StatusDate (Status, ExaminationDate)`
- `KEY IX_Examinations_PriorityDate (Priority, ExaminationDate)`
- `CONSTRAINT FK_Examinations_Patient FOREIGN KEY (PatientId) REFERENCES Patients(PatientId) ON DELETE RESTRICT`
- `CONSTRAINT FK_Examinations_Creator FOREIGN KEY (CreatedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`

### ExaminationTests

| Column | SQL declaration |
|---|---|
| `ExaminationTestId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `ExaminationId` | `BIGINT NOT NULL` |
| `TestDefinitionVersionId` | `BIGINT NOT NULL` |
| `TestInstanceNumber` | `SMALLINT UNSIGNED NOT NULL DEFAULT 1` |
| `Status` | `VARCHAR(30) NOT NULL DEFAULT 'Pending'` |
| `Priority` | `VARCHAR(20) NOT NULL DEFAULT 'Routine'` |
| `RequestedAtUtc` | `DATETIME(6) NOT NULL` |
| `RequestedByUserId` | `BIGINT NOT NULL` |
| `CompletedAtUtc` | `DATETIME(6) NULL` |
| `ActualMethodId` | `BIGINT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (ExaminationTestId)`
- `UNIQUE KEY UX_ExaminationTests_Instance (ExaminationId, TestDefinitionVersionId, TestInstanceNumber)`
- `KEY IX_ExaminationTests_Status (Status, Priority)`
- `KEY IX_ExaminationTests_Version (TestDefinitionVersionId)`
- `KEY IX_ExaminationTests_Method (ActualMethodId)`
- `CONSTRAINT FK_ExaminationTests_Examination FOREIGN KEY (ExaminationId) REFERENCES Examinations(ExaminationId) ON DELETE RESTRICT`
- `CONSTRAINT FK_ExaminationTests_Version FOREIGN KEY (TestDefinitionVersionId) REFERENCES TestDefinitionVersions(TestDefinitionVersionId) ON DELETE RESTRICT`
- `CONSTRAINT FK_ExaminationTests_Requester FOREIGN KEY (RequestedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`
- `CONSTRAINT FK_ExaminationTests_ActualMethod FOREIGN KEY (ActualMethodId) REFERENCES TestMethods(TestMethodId) ON DELETE RESTRICT`

### ExaminationStatusHistory

| Column | SQL declaration |
|---|---|
| `ExaminationStatusHistoryId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `ExaminationId` | `BIGINT NOT NULL` |
| `ExaminationTestId` | `BIGINT NULL` |
| `PreviousStatus` | `VARCHAR(30) NULL` |
| `NewStatus` | `VARCHAR(30) NOT NULL` |
| `Reason` | `VARCHAR(1000) NULL` |
| `ChangedByUserId` | `BIGINT NOT NULL` |
| `ChangedAtUtc` | `DATETIME(6) NOT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (ExaminationStatusHistoryId)`
- `KEY IX_ExaminationStatusHistory_ExamDate (ExaminationId, ChangedAtUtc)`
- `KEY IX_ExaminationStatusHistory_TestDate (ExaminationTestId, ChangedAtUtc)`
- `CONSTRAINT FK_ExaminationStatusHistory_Examination FOREIGN KEY (ExaminationId) REFERENCES Examinations(ExaminationId) ON DELETE RESTRICT`
- `CONSTRAINT FK_ExaminationStatusHistory_Test FOREIGN KEY (ExaminationTestId) REFERENCES ExaminationTests(ExaminationTestId) ON DELETE RESTRICT`
- `CONSTRAINT FK_ExaminationStatusHistory_Actor FOREIGN KEY (ChangedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`

### TestResults

| Column | SQL declaration |
|---|---|
| `TestResultId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `ExaminationTestId` | `BIGINT NOT NULL` |
| `TestParameterDefinitionId` | `BIGINT NOT NULL` |
| `CurrentRevisionId` | `BIGINT NULL` |
| `CreatedAtUtc` | `DATETIME(6) NOT NULL` |
| `CreatedByUserId` | `BIGINT NOT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (TestResultId)`
- `UNIQUE KEY UX_TestResults_TestParameter (ExaminationTestId, TestParameterDefinitionId)`
- `KEY IX_TestResults_CurrentRevision (CurrentRevisionId)`
- `CONSTRAINT FK_TestResults_ExaminationTest FOREIGN KEY (ExaminationTestId) REFERENCES ExaminationTests(ExaminationTestId) ON DELETE RESTRICT`
- `CONSTRAINT FK_TestResults_Parameter FOREIGN KEY (TestParameterDefinitionId) REFERENCES TestParameterDefinitions(TestParameterDefinitionId) ON DELETE RESTRICT`
- `CONSTRAINT FK_TestResults_Creator FOREIGN KEY (CreatedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`

### TestResultRevisions

| Column | SQL declaration |
|---|---|
| `TestResultRevisionId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `TestResultId` | `BIGINT NOT NULL` |
| `RevisionNumber` | `INT NOT NULL` |
| `ValueType` | `VARCHAR(20) NOT NULL` |
| `NumericValue` | `DECIMAL(18,6) NULL` |
| `TextValue` | `LONGTEXT NULL` |
| `ResultValueOptionId` | `BIGINT NULL` |
| `TiterValue` | `VARCHAR(80) NULL` |
| `UnitId` | `BIGINT NULL` |
| `ReferenceIntervalId` | `BIGINT NULL` |
| `UnitSnapshot` | `VARCHAR(120) NULL` |
| `ReferenceRangeSnapshot` | `VARCHAR(500) NULL` |
| `MethodSnapshot` | `VARCHAR(160) NULL` |
| `IsAbnormal` | `TINYINT(1) NOT NULL DEFAULT 0` |
| `IsCritical` | `TINYINT(1) NOT NULL DEFAULT 0` |
| `Interpretation` | `VARCHAR(2000) NULL` |
| `Status` | `VARCHAR(30) NOT NULL DEFAULT 'Draft'` |
| `RevisionReason` | `VARCHAR(1000) NULL` |
| `CreatedAtUtc` | `DATETIME(6) NOT NULL` |
| `CreatedByUserId` | `BIGINT NOT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (TestResultRevisionId)`
- `UNIQUE KEY UX_TestResultRevisions_Number (TestResultId, RevisionNumber)`
- `KEY IX_TestResultRevisions_StatusDate (Status, CreatedAtUtc)`
- `KEY IX_TestResultRevisions_Option (ResultValueOptionId)`
- `KEY IX_TestResultRevisions_Unit (UnitId)`
- `KEY IX_TestResultRevisions_Reference (ReferenceIntervalId)`
- `CONSTRAINT FK_TestResultRevisions_Result FOREIGN KEY (TestResultId) REFERENCES TestResults(TestResultId) ON DELETE RESTRICT`
- `CONSTRAINT FK_TestResultRevisions_Option FOREIGN KEY (ResultValueOptionId) REFERENCES ResultValueOptions(ResultValueOptionId) ON DELETE RESTRICT`
- `CONSTRAINT FK_TestResultRevisions_Unit FOREIGN KEY (UnitId) REFERENCES Units(UnitId) ON DELETE RESTRICT`
- `CONSTRAINT FK_TestResultRevisions_Reference FOREIGN KEY (ReferenceIntervalId) REFERENCES ReferenceIntervals(ReferenceIntervalId) ON DELETE RESTRICT`
- `CONSTRAINT FK_TestResultRevisions_Encoder FOREIGN KEY (CreatedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`
- `CONSTRAINT CK_TestResultRevisions_ValueType CHECK (ValueType IN ('Numeric','Qualitative','Text','Titer'))`

### ResultSubmissions

| Column | SQL declaration |
|---|---|
| `ResultSubmissionId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `ExaminationTestId` | `BIGINT NOT NULL` |
| `SubmissionNumber` | `INT NOT NULL` |
| `Status` | `VARCHAR(30) NOT NULL DEFAULT 'Submitted'` |
| `SubmittedAtUtc` | `DATETIME(6) NOT NULL` |
| `SubmittedByUserId` | `BIGINT NOT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (ResultSubmissionId)`
- `UNIQUE KEY UX_ResultSubmissions_Number (ExaminationTestId, SubmissionNumber)`
- `KEY IX_ResultSubmissions_StatusDate (Status, SubmittedAtUtc)`
- `CONSTRAINT FK_ResultSubmissions_Test FOREIGN KEY (ExaminationTestId) REFERENCES ExaminationTests(ExaminationTestId) ON DELETE RESTRICT`
- `CONSTRAINT FK_ResultSubmissions_Submitter FOREIGN KEY (SubmittedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`

### ResultSubmissionItems

| Column | SQL declaration |
|---|---|
| `ResultSubmissionItemId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `ResultSubmissionId` | `BIGINT NOT NULL` |
| `TestResultId` | `BIGINT NOT NULL` |
| `TestResultRevisionId` | `BIGINT NOT NULL` |
| `AddedAtUtc` | `DATETIME(6) NOT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (ResultSubmissionItemId)`
- `UNIQUE KEY UX_ResultSubmissionItems_Result (ResultSubmissionId, TestResultId)`
- `KEY IX_ResultSubmissionItems_Revision (TestResultRevisionId)`
- `CONSTRAINT FK_ResultSubmissionItems_Submission FOREIGN KEY (ResultSubmissionId) REFERENCES ResultSubmissions(ResultSubmissionId) ON DELETE RESTRICT`
- `CONSTRAINT FK_ResultSubmissionItems_Result FOREIGN KEY (TestResultId) REFERENCES TestResults(TestResultId) ON DELETE RESTRICT`
- `CONSTRAINT FK_ResultSubmissionItems_Revision FOREIGN KEY (TestResultRevisionId) REFERENCES TestResultRevisions(TestResultRevisionId) ON DELETE RESTRICT`

### ResultVerificationEvents

| Column | SQL declaration |
|---|---|
| `ResultVerificationEventId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `ResultSubmissionId` | `BIGINT NOT NULL` |
| `Decision` | `VARCHAR(30) NOT NULL` |
| `Comments` | `VARCHAR(2000) NULL` |
| `ReturnReason` | `VARCHAR(1000) NULL` |
| `VerifiedByUserId` | `BIGINT NOT NULL` |
| `VerifiedAtUtc` | `DATETIME(6) NOT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (ResultVerificationEventId)`
- `KEY IX_ResultVerificationEvents_SubmissionDate (ResultSubmissionId, VerifiedAtUtc)`
- `CONSTRAINT FK_ResultVerificationEvents_Submission FOREIGN KEY (ResultSubmissionId) REFERENCES ResultSubmissions(ResultSubmissionId) ON DELETE RESTRICT`
- `CONSTRAINT FK_ResultVerificationEvents_Verifier FOREIGN KEY (VerifiedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`

### CriticalResultEvents

| Column | SQL declaration |
|---|---|
| `CriticalResultEventId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `TestResultRevisionId` | `BIGINT NOT NULL` |
| `CommunicationStatus` | `VARCHAR(30) NOT NULL` |
| `RecipientName` | `VARCHAR(180) NULL` |
| `RecipientRole` | `VARCHAR(120) NULL` |
| `ContactMethod` | `VARCHAR(40) NOT NULL` |
| `ContactedAtUtc` | `DATETIME(6) NULL` |
| `ActorStaffProfileId` | `BIGINT NOT NULL` |
| `AcknowledgedAtUtc` | `DATETIME(6) NULL` |
| `AcknowledgmentDetails` | `VARCHAR(1000) NULL` |
| `Notes` | `VARCHAR(2000) NULL` |
| `CreatedAtUtc` | `DATETIME(6) NOT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (CriticalResultEventId)`
- `KEY IX_CriticalResultEvents_RevisionDate (TestResultRevisionId, CreatedAtUtc)`
- `CONSTRAINT FK_CriticalResultEvents_Revision FOREIGN KEY (TestResultRevisionId) REFERENCES TestResultRevisions(TestResultRevisionId) ON DELETE RESTRICT`
- `CONSTRAINT FK_CriticalResultEvents_Actor FOREIGN KEY (ActorStaffProfileId) REFERENCES StaffProfiles(StaffProfileId) ON DELETE RESTRICT`

### StoredFiles

| Column | SQL declaration |
|---|---|
| `StoredFileId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `FileCategory` | `VARCHAR(60) NOT NULL` |
| `OriginalFileName` | `VARCHAR(255) NOT NULL` |
| `StorageKey` | `VARCHAR(500) NOT NULL` |
| `MimeType` | `VARCHAR(120) NOT NULL` |
| `ByteSize` | `BIGINT NOT NULL` |
| `Sha256` | `CHAR(64) NOT NULL` |
| `MetadataJson` | `LONGTEXT NULL` |
| `CreatedAtUtc` | `DATETIME(6) NOT NULL` |
| `CreatedByUserId` | `BIGINT NOT NULL` |
| `IsArchived` | `TINYINT(1) NOT NULL DEFAULT 0` |

**Declared keys and constraints**

- `PRIMARY KEY (StoredFileId)`
- `UNIQUE KEY UX_StoredFiles_StorageKey (StorageKey)`
- `KEY IX_StoredFiles_CategoryDate (FileCategory, CreatedAtUtc)`
- `CONSTRAINT FK_StoredFiles_Creator FOREIGN KEY (CreatedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`

### ReportReleases

| Column | SQL declaration |
|---|---|
| `ReportReleaseId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `ExaminationId` | `BIGINT NOT NULL` |
| `ReleaseNumber` | `INT NOT NULL` |
| `ReportNumber` | `VARCHAR(50) NOT NULL` |
| `Status` | `VARCHAR(30) NOT NULL DEFAULT 'Draft'` |
| `PatientSnapshot` | `LONGTEXT NOT NULL` |
| `ExaminationSnapshot` | `LONGTEXT NOT NULL` |
| `GeneratedAtUtc` | `DATETIME(6) NULL` |
| `GeneratedByUserId` | `BIGINT NULL` |
| `ReleasedAtUtc` | `DATETIME(6) NULL` |
| `ReleasedByUserId` | `BIGINT NULL` |
| `SupersedesReportReleaseId` | `BIGINT NULL` |
| `ReleaseReason` | `VARCHAR(1000) NULL` |
| `CreatedAtUtc` | `DATETIME(6) NOT NULL` |
| `CreatedByUserId` | `BIGINT NOT NULL` |
| `RowVersion` | `BIGINT NOT NULL DEFAULT 1` |
| `ReleasedPdfFileId` | `BIGINT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (ReportReleaseId)`
- `UNIQUE KEY UX_ReportReleases_ExamRelease (ExaminationId, ReleaseNumber)`
- `UNIQUE KEY UX_ReportReleases_Number (ReportNumber)`
- `UNIQUE KEY UX_ReportReleases_Supersedes (SupersedesReportReleaseId)`
- `KEY IX_ReportReleases_StatusDate (Status, ReleasedAtUtc)`
- `CONSTRAINT FK_ReportReleases_Examination FOREIGN KEY (ExaminationId) REFERENCES Examinations(ExaminationId) ON DELETE RESTRICT`
- `CONSTRAINT FK_ReportReleases_Supersedes FOREIGN KEY (SupersedesReportReleaseId) REFERENCES ReportReleases(ReportReleaseId) ON DELETE RESTRICT`
- `CONSTRAINT FK_ReportReleases_Pdf FOREIGN KEY (ReleasedPdfFileId) REFERENCES StoredFiles(StoredFileId) ON DELETE RESTRICT`
- `CONSTRAINT FK_ReportReleases_Generator FOREIGN KEY (GeneratedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`
- `CONSTRAINT FK_ReportReleases_Releaser FOREIGN KEY (ReleasedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`
- `CONSTRAINT FK_ReportReleases_Creator FOREIGN KEY (CreatedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`

### ReportReleaseItems

| Column | SQL declaration |
|---|---|
| `ReportReleaseItemId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `ReportReleaseId` | `BIGINT NOT NULL` |
| `ExaminationTestId` | `BIGINT NOT NULL` |
| `ResultSubmissionId` | `BIGINT NOT NULL` |
| `DisplayOrder` | `INT NOT NULL` |
| `SectionTitleSnapshot` | `VARCHAR(180) NOT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (ReportReleaseItemId)`
- `UNIQUE KEY UX_ReportReleaseItems_Test (ReportReleaseId, ExaminationTestId)`
- `KEY IX_ReportReleaseItems_Submission (ResultSubmissionId)`
- `CONSTRAINT FK_ReportReleaseItems_Report FOREIGN KEY (ReportReleaseId) REFERENCES ReportReleases(ReportReleaseId) ON DELETE RESTRICT`
- `CONSTRAINT FK_ReportReleaseItems_Test FOREIGN KEY (ExaminationTestId) REFERENCES ExaminationTests(ExaminationTestId) ON DELETE RESTRICT`
- `CONSTRAINT FK_ReportReleaseItems_Submission FOREIGN KEY (ResultSubmissionId) REFERENCES ResultSubmissions(ResultSubmissionId) ON DELETE RESTRICT`

### ReportSignatures

| Column | SQL declaration |
|---|---|
| `ReportSignatureId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `ReportReleaseId` | `BIGINT NOT NULL` |
| `StaffProfileId` | `BIGINT NOT NULL` |
| `ProfessionalCredentialId` | `BIGINT NULL` |
| `StaffAuthorizationId` | `BIGINT NULL` |
| `SignatureRole` | `VARCHAR(60) NOT NULL` |
| `SignatoryNameSnapshot` | `VARCHAR(220) NOT NULL` |
| `PrcNumberSnapshot` | `VARCHAR(80) NULL` |
| `CredentialSnapshot` | `LONGTEXT NULL` |
| `SignatureMethod` | `VARCHAR(60) NOT NULL` |
| `SignatureFileId` | `BIGINT NULL` |
| `SignedAtUtc` | `DATETIME(6) NOT NULL` |
| `Status` | `VARCHAR(30) NOT NULL DEFAULT 'Signed'` |
| `AuthorizationSnapshot` | `LONGTEXT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (ReportSignatureId)`
- `UNIQUE KEY UX_ReportSignatures_Role (ReportReleaseId, StaffProfileId, SignatureRole)`
- `KEY IX_ReportSignatures_Credential (ProfessionalCredentialId)`
- `KEY IX_ReportSignatures_Authorization (StaffAuthorizationId)`
- `KEY IX_ReportSignatures_File (SignatureFileId)`
- `CONSTRAINT FK_ReportSignatures_Report FOREIGN KEY (ReportReleaseId) REFERENCES ReportReleases(ReportReleaseId) ON DELETE RESTRICT`
- `CONSTRAINT FK_ReportSignatures_Staff FOREIGN KEY (StaffProfileId) REFERENCES StaffProfiles(StaffProfileId) ON DELETE RESTRICT`
- `CONSTRAINT FK_ReportSignatures_Credential FOREIGN KEY (ProfessionalCredentialId) REFERENCES ProfessionalCredentials(ProfessionalCredentialId) ON DELETE RESTRICT`
- `CONSTRAINT FK_ReportSignatures_Authorization FOREIGN KEY (StaffAuthorizationId) REFERENCES StaffAuthorizations(StaffAuthorizationId) ON DELETE RESTRICT`
- `CONSTRAINT FK_ReportSignatures_File FOREIGN KEY (SignatureFileId) REFERENCES StoredFiles(StoredFileId) ON DELETE RESTRICT`

### ReportAmendments

| Column | SQL declaration |
|---|---|
| `ReportAmendmentId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `OriginalReportReleaseId` | `BIGINT NOT NULL` |
| `ReplacementReportReleaseId` | `BIGINT NOT NULL` |
| `AmendmentType` | `VARCHAR(60) NOT NULL` |
| `Reason` | `VARCHAR(1000) NOT NULL` |
| `RequestedByUserId` | `BIGINT NOT NULL` |
| `RequestedAtUtc` | `DATETIME(6) NOT NULL` |
| `ApprovedByUserId` | `BIGINT NULL` |
| `ApprovedAtUtc` | `DATETIME(6) NULL` |
| `ApprovalComments` | `VARCHAR(1000) NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (ReportAmendmentId)`
- `UNIQUE KEY UX_ReportAmendments_Replacement (ReplacementReportReleaseId)`
- `KEY IX_ReportAmendments_Original (OriginalReportReleaseId)`
- `CONSTRAINT FK_ReportAmendments_Original FOREIGN KEY (OriginalReportReleaseId) REFERENCES ReportReleases(ReportReleaseId) ON DELETE RESTRICT`
- `CONSTRAINT FK_ReportAmendments_Replacement FOREIGN KEY (ReplacementReportReleaseId) REFERENCES ReportReleases(ReportReleaseId) ON DELETE RESTRICT`
- `CONSTRAINT FK_ReportAmendments_Requester FOREIGN KEY (RequestedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`
- `CONSTRAINT FK_ReportAmendments_Approver FOREIGN KEY (ApprovedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`
- `CONSTRAINT CK_ReportAmendments_Different CHECK (OriginalReportReleaseId <> ReplacementReportReleaseId)`

### ReportDistributionEvents

| Column | SQL declaration |
|---|---|
| `ReportDistributionEventId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `ReportReleaseId` | `BIGINT NOT NULL` |
| `DistributionMethod` | `VARCHAR(40) NOT NULL` |
| `RecipientName` | `VARCHAR(180) NULL` |
| `RecipientRelationship` | `VARCHAR(120) NULL` |
| `RecipientContact` | `VARCHAR(120) NULL` |
| `DistributedByStaffProfileId` | `BIGINT NOT NULL` |
| `DistributedAtUtc` | `DATETIME(6) NOT NULL` |
| `AcknowledgmentDetails` | `VARCHAR(1000) NULL` |
| `Notes` | `VARCHAR(2000) NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (ReportDistributionEventId)`
- `KEY IX_ReportDistributionEvents_ReportDate (ReportReleaseId, DistributedAtUtc)`
- `CONSTRAINT FK_ReportDistributionEvents_Report FOREIGN KEY (ReportReleaseId) REFERENCES ReportReleases(ReportReleaseId) ON DELETE RESTRICT`
- `CONSTRAINT FK_ReportDistributionEvents_Staff FOREIGN KEY (DistributedByStaffProfileId) REFERENCES StaffProfiles(StaffProfileId) ON DELETE RESTRICT`

### AuditLogs

| Column | SQL declaration |
|---|---|
| `AuditLogId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `ActorUserId` | `BIGINT NULL` |
| `ActionType` | `VARCHAR(100) NOT NULL` |
| `EntityType` | `VARCHAR(100) NOT NULL` |
| `EntityId` | `BIGINT NULL` |
| `Description` | `VARCHAR(2000) NULL` |
| `Reason` | `VARCHAR(1000) NULL` |
| `BeforeData` | `LONGTEXT NULL` |
| `AfterData` | `LONGTEXT NULL` |
| `IpAddress` | `VARCHAR(45) NULL` |
| `UserAgent` | `VARCHAR(500) NULL` |
| `CorrelationId` | `VARCHAR(100) NULL` |
| `OccurredAtUtc` | `DATETIME(6) NOT NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (AuditLogId)`
- `KEY IX_AuditLogs_ActorTime (ActorUserId, OccurredAtUtc)`
- `KEY IX_AuditLogs_EntityTime (EntityType, EntityId, OccurredAtUtc)`
- `KEY IX_AuditLogs_ActionTime (ActionType, OccurredAtUtc)`
- `KEY IX_AuditLogs_Correlation (CorrelationId)`
- `CONSTRAINT FK_AuditLogs_Actor FOREIGN KEY (ActorUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`

### WorkflowEvents

| Column | SQL declaration |
|---|---|
| `WorkflowEventId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `EntityType` | `VARCHAR(100) NOT NULL` |
| `EntityId` | `BIGINT NOT NULL` |
| `EventType` | `VARCHAR(100) NOT NULL` |
| `PreviousStatus` | `VARCHAR(40) NULL` |
| `NewStatus` | `VARCHAR(40) NULL` |
| `Reason` | `VARCHAR(1000) NULL` |
| `Notes` | `VARCHAR(2000) NULL` |
| `ActorUserId` | `BIGINT NULL` |
| `OccurredAtUtc` | `DATETIME(6) NOT NULL` |
| `CorrelationId` | `VARCHAR(100) NULL` |

**Declared keys and constraints**

- `PRIMARY KEY (WorkflowEventId)`
- `KEY IX_WorkflowEvents_EntityTime (EntityType, EntityId, OccurredAtUtc)`
- `KEY IX_WorkflowEvents_EventTime (EventType, OccurredAtUtc)`
- `KEY IX_WorkflowEvents_ActorTime (ActorUserId, OccurredAtUtc)`
- `KEY IX_WorkflowEvents_Correlation (CorrelationId)`
- `CONSTRAINT FK_WorkflowEvents_Actor FOREIGN KEY (ActorUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT`

### NumberingCounters

| Column | SQL declaration |
|---|---|
| `NumberingCounterId` | `BIGINT NOT NULL AUTO_INCREMENT` |
| `NumberType` | `VARCHAR(30) NOT NULL` |
| `PeriodKey` | `VARCHAR(12) NOT NULL` |
| `LastNumber` | `BIGINT NOT NULL DEFAULT 0` |
| `UpdatedAtUtc` | `DATETIME(6) NOT NULL` |
| `RowVersion` | `BIGINT NOT NULL DEFAULT 1` |

**Declared keys and constraints**

- `PRIMARY KEY (NumberingCounterId)`
- `UNIQUE KEY UX_NumberingCounters_TypePeriod (NumberType, PeriodKey)`

### Deferred foreign-key additions

- `FK_TestResults_CurrentRevision`: `CurrentRevisionId` references `TestResultRevisions(TestResultRevisionId)`, `ON DELETE RESTRICT`; added by `ALTER TABLE` after the target table is created.
- `FK_StaffAuthorizations_EvidenceFile`: `EvidenceFileId` references `StoredFiles(StoredFileId)`, `ON DELETE RESTRICT`; added by `ALTER TABLE` after the target table is created.

## 5. Source-defined design characteristics

- Engine and default character set/collation are specified per table as InnoDB, utf8mb4, and utf8mb4_unicode_ci.
- IDs are declared as signed BIGINT in this source file; identity PKs generally use AUTO_INCREMENT.
- Timestamps use DATETIME(6) with UTC semantics stated in the SQL header/design assumptions; MariaDB DATETIME itself does not store timezone metadata.
- Operational dates use DATE.
- Boolean-like fields use TINYINT(1).
- JSON-like payloads are represented as LONGTEXT, not native JSON columns.
- Result numeric values and scoring/weight fields use DECIMAL declarations; consult the column inventory for exact precision.
- Referential actions explicitly shown in the SQL are ON DELETE RESTRICT.
- The source defines CHECK constraints on selected fields/pairs, listed with their tables above.

## 6. Reconciliation findings and review points

The following are design findings, not claims that the current SQL already enforces the recommended behavior. Resolve each item before freezing the schema:

1. **Deferred FK ordering:** `StaffAuthorizations.EvidenceFileId` is nullable and its FK is added after `StoredFiles` is created. Confirm install order and migration generation preserve this dependency.
2. **Current result revision:** `TestResults.CurrentRevisionId` is a pointer to a revision. Confirm the FK exists in the source and that the application guarantees the referenced revision belongs to the same `TestResultId`; a simple FK to revision PK alone cannot prove that pair relationship.
3. **Result/test compatibility:** Ensure each result parameter belongs to the exact `TestDefinitionVersionId` selected for the examination test. Enforce in service logic or add a schema design that can guarantee it.
4. **Submission membership:** Ensure each `ResultSubmissionItem` result/revision corresponds to the `ExaminationTestId` and examination being submitted. Individual FKs do not necessarily prove this multi-table invariant.
5. **Report consistency:** Ensure report release, report item, examination test, and result submission all belong to the same examination and appropriate revision. Individual FKs may permit mismatched relationships.
6. **Signatory validity:** Validate staff, credential, authorization ownership, effective dates, status, and scope at signing time. FK existence alone does not prove authorization.
7. **Patient merge integrity:** Application logic must prevent merge cycles, invalid canonical chains, repeated execution, and unauthorized reversal; the CHECK only prevents source and destination being identical where declared.
8. **Report amendment integrity:** Application logic must prevent amendment cycles and enforce valid original/replacement release relationships; the CHECK only prevents identical IDs where declared.
9. **Append-only history:** FKs and ordinary table definitions do not make audit, workflow, result revision, demographic history, signature, or distribution records immutable. Enforce through restricted application operations and database grants/operational controls.
10. **CHECK behavior:** Confirm all CHECK declarations are accepted/enforced by the target MariaDB 10.11 patch release and test invalid inserts/updates. Static parsing does not verify runtime enforcement.
11. **Identity mapping:** Confirm ASP.NET Core Identity model mappings for BIGINT keys, composite keys, lengths, indexes, and `LockoutEnd DATETIME(6)` against the selected EF Core provider.
12. **Concurrency:** `RowVersion BIGINT` defaults to 1 but does not auto-increment on update by SQL declaration alone. Application/EF configuration must increment it atomically and detect stale writes.
13. **LONGTEXT payloads:** Validate JSON schema/version in the application, cap payload size, redact sensitive values, and avoid storing secrets or unnecessary patient information.
14. **String status fields:** Many workflow/status/method/type columns are VARCHAR rather than ENUM or FK-backed lookup values. Define canonical code sets and validate them in the application; consider lookup tables only through a reviewed schema change.
15. **Uniqueness semantics:** Confirm case/collation behavior for usernames, patient numbers, codes, report numbers, storage keys, and other unique strings under utf8mb4_unicode_ci.
16. **Numbering:** `NumberingCounters` provides a counter uniqueness key, but transaction isolation, locking, retry behavior, formatting, period rollover, and gap policy remain application-level requirements.
17. **Patient duplicate pair:** The declared ordered-pair CHECK plus unique pair index supports normalized pair ordering only if MariaDB enforces the CHECK; service code should canonicalize IDs before insert.
18. **Nullable unique values:** MariaDB permits multiple NULL values in unique indexes. Confirm this is intended for nullable fields such as EmployeeNumber and optional supersession pointers.

## 7. Constraint classification

| Classification | Meaning | Examples in this project |
|---|---|---|
| Source-defined | Explicit SQL declaration | PK, UNIQUE KEY, KEY, FK, CHECK, NOT NULL, DEFAULT |
| Proposed | Recommended schema addition not present in source | Composite FK or additional uniqueness/check constraint after design review |
| Application-enforced | Requires business logic, transaction, or multi-row validation | Same-examination consistency, authorization at signing time, state transitions, merge/amendment cycle prevention |
| Operational | Requires deployment, permissions, backup, or procedures | Least-privilege DB grants, backup/restore, file retention, privileged access control |

## 8. Runtime validation still required

Before schema approval for implementation, execute the SQL only on a disposable MariaDB 10.11 test instance and record:
- Exact MariaDB server version and SQL mode.
- Successful execution of all seven batches in order.
- `SHOW TABLES` count and table names (expected 48).
- `SHOW CREATE TABLE` for all 48 tables, compared with this dictionary.
- `information_schema` counts and definitions for columns, indexes, FKs, and CHECK constraints.
- Positive and negative tests for each CHECK, FK, UNIQUE, and NOT NULL behavior.
- EF Core/Pomelo model mapping and migration comparison, including Identity and RowVersion.
- Transaction/concurrency tests for numbering, result revisions, submission, report release, and amendments.
- Backup and restore test including stored-file references and hashes.

Do not run this review SQL against production or any database containing real patient data.

## 9. Approval checklist

- [ ] I confirm the 48-table source inventory is the intended Release 1 baseline.
- [ ] I have reviewed the column declarations and nullable/default behavior.
- [ ] I have reviewed the declared keys, indexes, foreign keys, and CHECK constraints.
- [ ] I accept the distinction between source-defined, proposed, application-enforced, and operational rules.
- [ ] I accept that MariaDB runtime validation is still a separate mandatory gate.
- [ ] I approve SDD-03 version 0.2 as the design baseline (subject to recorded open technical validation items).

## 10. Status

**Current status: Draft — For Review.** This document is a static reconciliation against the SQL source file. User approval is requested; it does not claim runtime validation and does not authorize production SQL execution or application implementation.
