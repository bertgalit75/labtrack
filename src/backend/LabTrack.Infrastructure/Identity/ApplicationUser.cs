using Microsoft.AspNetCore.Identity;

namespace LabTrack.Infrastructure.Identity;

// AspNetUsers columns are kept aligned with the approved SDD-03 SQL baseline.
// Staff display names belong to StaffProfiles, not this Identity row.
public sealed class ApplicationUser : IdentityUser<long>
{
    public bool IsActive { get; set; } = true;
}
