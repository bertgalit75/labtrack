using LabTrack.Infrastructure.Identity;
using LabTrack.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseWindowsService();
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
var mariaDbConnectionString = builder.Configuration.GetConnectionString("LabTrack");

if (persistenceMode == "MariaDb")
{
    if (string.IsNullOrWhiteSpace(mariaDbConnectionString))
    {
        throw new InvalidOperationException(
            "Persistence:Mode is MariaDb, but ConnectionStrings:LabTrack is not configured. " +
            "Supply it through a secret store or the ConnectionStrings__LabTrack environment variable.");
    }

    // Keep the provider and server version explicit: the qualified target is MariaDB 10.11.
    // Do not use ServerVersion.AutoDetect here; startup must not infer a different provider target.
    builder.Services.AddDbContext<LabTrackDbContext>(options =>
        options.UseMySql(
            mariaDbConnectionString,
            new MariaDbServerVersion(new Version(10, 11, 6))));
}
else if (builder.Environment.IsDevelopment() && persistenceMode == "DevelopmentInMemory")
{
    builder.Services.AddDbContext<LabTrackDbContext>(options =>
        options.UseInMemoryDatabase("LabTrack-DEV-ONLY"));
}
else
{
    throw new InvalidOperationException(
        $"Unsupported persistence configuration '{persistenceMode}' for environment '{builder.Environment.EnvironmentName}'. " +
        "Use DevelopmentInMemory only in Development, or configure MariaDb with a secret ConnectionStrings:LabTrack value.");
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

builder.Services.AddAuthorization();
var app = builder.Build();

if (persistenceMode == "MariaDb")
{
    // Fail closed when the database is unavailable or the schema has not been deployed.
    // Application startup deliberately does not run migrations: the runtime DB account
    // must not need DDL privileges. Apply reviewed migrations through a separate release step.
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<LabTrackDbContext>();

    if (!await db.Database.CanConnectAsync())
    {
        throw new InvalidOperationException(
            "LabTrack could not connect to the configured MariaDB database. Verify the endpoint, credentials, network, and database availability.");
    }

    var pendingMigrations = (await db.Database.GetPendingMigrationsAsync()).ToArray();
    if (pendingMigrations.Length > 0)
    {
        throw new InvalidOperationException(
            "The configured MariaDB database has pending EF Core migrations. Apply a reviewed database release before starting LabTrack. " +
            $"Pending migrations: {string.Join(", ", pendingMigrations)}");
    }
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
if (app.Environment.IsDevelopment()) app.MapOpenApi();
app.MapGet("/health/live", () => Results.Ok(new { status = "live" }));
app.MapGet("/health/ready", async (LabTrackDbContext db, CancellationToken cancellationToken) =>
{
    try
    {
        return await db.Database.CanConnectAsync(cancellationToken)
            ? Results.Ok(new { status = "ready" })
            : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }
    catch
    {
        return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }
});
app.MapFallback(async context =>
{
    if (context.Request.Path.StartsWithSegments("/api"))
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        return;
    }

    var webRoot = app.Environment.WebRootPath ?? Path.Combine(app.Environment.ContentRootPath, "wwwroot");
    var indexPath = Path.Combine(webRoot, "index.html");
    if (!File.Exists(indexPath))
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        return;
    }

    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync(indexPath);
});
app.Run();
