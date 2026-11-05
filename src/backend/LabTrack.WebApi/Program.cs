using LabTrack.Application.Patients;
using LabTrack.Infrastructure.Identity;
using LabTrack.Infrastructure.Patients;
using LabTrack.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "LabTrack.AntiForgery";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
builder.Services.AddRateLimiter(options => options.AddFixedWindowLimiter("login", limiter =>
{
    limiter.PermitLimit = 5;
    limiter.Window = TimeSpan.FromMinutes(1);
    limiter.QueueLimit = 0;
    limiter.AutoReplenishment = true;
}));

var persistenceMode = builder.Configuration["Persistence:Mode"] ?? "Unconfigured";
if (builder.Environment.IsDevelopment() && persistenceMode == "DevelopmentInMemory")
{
    builder.Services.AddDbContext<LabTrackDbContext>(options => options.UseInMemoryDatabase("LabTrack-DEV-ONLY"));
}
else
{
    throw new InvalidOperationException(
        "No qualified MariaDB EF Core provider is configured. The application intentionally refuses to start outside " +
        "DevelopmentInMemory mode until the MariaDB 10.11 provider and mappings pass the approved validation gates.");
}

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.User.RequireUniqueEmail = false;
    options.Password.RequiredLength = 12;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
})
.AddRoles<IdentityRole<long>>()
.AddEntityFrameworkStores<LabTrackDbContext>()
.AddSignInManager();
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "LabTrack.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.SlidingExpiration = false;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.LoginPath = "/login";
});

builder.Services.AddAuthorization(options =>
{
    // Permission grants are deliberately not seeded while SDD-09 role grants remain unapproved.
    // Read/update access also requires an explicit all-record scope claim; missing scope is denied.
    options.AddPolicy("patients.read", policy => policy.RequireAuthenticatedUser()
        .RequireClaim("permission", "patients.read").RequireClaim("patient_scope", "all"));
    options.AddPolicy("patients.register", policy => policy.RequireAuthenticatedUser()
        .RequireClaim("permission", "patients.register"));
    options.AddPolicy("patients.demographics.update", policy => policy.RequireAuthenticatedUser()
        .RequireClaim("permission", "patients.demographics.update").RequireClaim("patient_scope", "all"));
});
builder.Services.AddScoped<IPatientService, PatientService>();
var app = builder.Build();
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
if (app.Environment.IsDevelopment()) app.MapOpenApi();
app.MapGet("/health/live", () => Results.Ok(new { status = "live" }));
app.Run();
