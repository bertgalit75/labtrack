using Microsoft.AspNetCore.RateLimiting;
using LabTrack.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LabTrack.WebApi.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager) : ControllerBase
{
    public sealed record LoginRequest(string Username, string Password);

    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrEmpty(request.Password))
            return Unauthorized(new ProblemDetails { Title = "Authentication failed.", Status = StatusCodes.Status401Unauthorized });

        var user = await userManager.FindByNameAsync(request.Username);
        if (user is null || !user.IsActive)
            return Unauthorized(new ProblemDetails { Title = "Authentication failed.", Status = StatusCodes.Status401Unauthorized });

        var result = await signInManager.PasswordSignInAsync(user, request.Password, isPersistent: false, lockoutOnFailure: true);
        if (!result.Succeeded)
            return Unauthorized(new ProblemDetails { Title = "Authentication failed.", Status = StatusCodes.Status401Unauthorized });

        var roles = await userManager.GetRolesAsync(user);
        // Additional privileged-user verification and permission resolution remain governed by SDD-07/SDD-11 open details.
        return Ok(new { user = new { id = user.Id.ToString(), displayName = user.UserName, roles, permissions = Array.Empty<string>() } });
    }

    [Authorize]
    [ValidateAntiForgeryToken]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null || !user.IsActive) return Unauthorized();
        var roles = await userManager.GetRolesAsync(user);
        return Ok(new { user = new { id = user.Id.ToString(), displayName = user.UserName, roles, permissions = Array.Empty<string>() } });
    }
}
