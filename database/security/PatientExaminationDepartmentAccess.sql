-- Review-only schema extension for examination + department patient access.
-- Target: MariaDB 10.11.x / InnoDB. Do not execute in production until provider qualification,
-- migration review, backup/restore rehearsal, and access-administration workflow approval.
SET NAMES utf8mb4 COLLATE utf8mb4_unicode_ci;

ALTER TABLE Examinations
 ADD UNIQUE KEY UX_Examinations_IdPatient (ExaminationId, PatientId);

CREATE TABLE Departments (
 DepartmentId BIGINT NOT NULL AUTO_INCREMENT,
 DepartmentCode VARCHAR(40) NOT NULL,
 DepartmentName VARCHAR(120) NOT NULL,
 IsActive TINYINT(1) NOT NULL DEFAULT 1,
 CreatedAtUtc DATETIME(6) NOT NULL,
 PRIMARY KEY (DepartmentId),
 UNIQUE KEY UX_Departments_Code (DepartmentCode)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE UserDepartmentMemberships (
 UserDepartmentMembershipId BIGINT NOT NULL AUTO_INCREMENT,
 UserId BIGINT NOT NULL,
 DepartmentId BIGINT NOT NULL,
 IsActive TINYINT(1) NOT NULL DEFAULT 1,
 EffectiveFromUtc DATETIME(6) NOT NULL,
 EffectiveToUtc DATETIME(6) NULL,
 PRIMARY KEY (UserDepartmentMembershipId),
 KEY IX_UserDepartmentMemberships_UserDepartmentActive (UserId, DepartmentId, IsActive),
 KEY IX_UserDepartmentMemberships_Effective (UserId, DepartmentId, EffectiveFromUtc, EffectiveToUtc),
 CONSTRAINT FK_UserDepartmentMemberships_User FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT,
 CONSTRAINT FK_UserDepartmentMemberships_Department FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId) ON DELETE RESTRICT,
 CONSTRAINT CK_UserDepartmentMemberships_Dates CHECK (EffectiveToUtc IS NULL OR EffectiveToUtc > EffectiveFromUtc)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE PatientExaminationAssignments (
 PatientExaminationAssignmentId BIGINT NOT NULL AUTO_INCREMENT,
 ExaminationId BIGINT NOT NULL,
 PatientId BIGINT NOT NULL,
 UserId BIGINT NOT NULL,
 DepartmentId BIGINT NOT NULL,
 IsActive TINYINT(1) NOT NULL DEFAULT 1,
 EffectiveFromUtc DATETIME(6) NOT NULL,
 EffectiveToUtc DATETIME(6) NULL,
 AssignedByUserId BIGINT NOT NULL,
 CreatedAtUtc DATETIME(6) NOT NULL,
 PRIMARY KEY (PatientExaminationAssignmentId),
 KEY IX_PatientExamAssignments_UserExamDept (UserId, ExaminationId, DepartmentId, IsActive),
 KEY IX_PatientExamAssignments_PatientUserDept (PatientId, UserId, DepartmentId),
 CONSTRAINT FK_PatientExamAssignments_Patient FOREIGN KEY (PatientId) REFERENCES Patients(PatientId) ON DELETE RESTRICT,
 CONSTRAINT FK_PatientExamAssignments_ExamPatient FOREIGN KEY (ExaminationId, PatientId)
  REFERENCES Examinations(ExaminationId, PatientId) ON DELETE RESTRICT,
 CONSTRAINT FK_PatientExamAssignments_User FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT,
 CONSTRAINT FK_PatientExamAssignments_Department FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId) ON DELETE RESTRICT,
 CONSTRAINT FK_PatientExamAssignments_AssignedBy FOREIGN KEY (AssignedByUserId) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT,
 CONSTRAINT CK_PatientExamAssignments_Dates CHECK (EffectiveToUtc IS NULL OR EffectiveToUtc > EffectiveFromUtc)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
