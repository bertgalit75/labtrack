namespace LabTrack.Domain.Entities;

// A grant ties one user to one examination and the department responsible for that examination.
// PatientId is stored as a consistency guard and must match Examinations.PatientId at query time.
public sealed class PatientExaminationAssignment
{
    public long PatientExaminationAssignmentId { get; set; }
    public long ExaminationId { get; set; }
    public long PatientId { get; set; }
    public long UserId { get; set; }
    public long DepartmentId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime EffectiveFromUtc { get; set; }
    public DateTime? EffectiveToUtc { get; set; }
    public long AssignedByUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public sealed class UserDepartmentMembership
{
    public long UserDepartmentMembershipId { get; set; }
    public long UserId { get; set; }
    public long DepartmentId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime EffectiveFromUtc { get; set; }
    public DateTime? EffectiveToUtc { get; set; }
}
