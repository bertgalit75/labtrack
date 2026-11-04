using Microsoft.AspNetCore.Identity;

namespace LabTrack.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<long>
{
    public bool IsActive { get; set; } = true;
    public DateTime? DeactivatedAtUtc { get; set; }
}
