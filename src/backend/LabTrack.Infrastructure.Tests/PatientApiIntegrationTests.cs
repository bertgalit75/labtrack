using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using LabTrack.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace LabTrack.Infrastructure.Tests;

public sealed class PatientApiIntegrationTests
{
    [Fact]
    public async Task PatientRead_RequiresAuthenticationPermissionAndAllRecordScope()
    {
        using var factory = new PatientApiFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });

        var anonymous = await client.GetAsync("/api/v1/patients");
        Assert.Equal(HttpStatusCode.Unauthorized, anonymous.StatusCode);

        SetIdentity(client, "patients.read");
        var missingScope = await client.GetAsync("/api/v1/patients");
        Assert.Equal(HttpStatusCode.Forbidden, missingScope.StatusCode);

        SetIdentity(client, "patients.read", scope: "all");
        var allowed = await client.GetAsync("/api/v1/patients");
        Assert.Equal(HttpStatusCode.OK, allowed.StatusCode);
    }

    [Fact]
    public async Task RegisterAndUpdate_RequireCsrfAndEnforceIfMatch()
    {
        using var factory = new PatientApiFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });
        SetIdentity(client, "patients.register");

        var missingCsrf = await client.PostAsJsonAsync("/api/v1/patients", new
        {
            firstName = "Ana",
            lastName = "Reyes"
        });
        Assert.Equal(HttpStatusCode.BadRequest, missingCsrf.StatusCode);

        var (cookie, token) = await GetAntiforgeryAsync(client);
        using var register = new HttpRequestMessage(HttpMethod.Post, "/api/v1/patients")
        {
            Content = JsonContent.Create(new
            {
                firstName = "Ana",
                lastName = "Reyes",
                mobileNumber = "09170000000"
            })
        };
        AddCsrf(register, cookie, token);
        var created = await client.SendAsync(register);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        Assert.True(created.Headers.Location is not null);
        var createdBody = await created.Content.ReadFromJsonAsync<JsonElement>();
        var patientId = createdBody.GetProperty("id").GetString();
        Assert.False(string.IsNullOrWhiteSpace(patientId));

        SetIdentity(client, "patients.read", scope: "all");
        var read = await client.GetAsync($"/api/v1/patients/{patientId}");
        Assert.Equal(HttpStatusCode.OK, read.StatusCode);
        Assert.Equal("Ana", (await read.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("firstName").GetString());

        SetIdentity(client, "patients.demographics.update", scope: "all");
        var etag = created.Headers.ETag?.ToString();
        Assert.Equal("\"1\"", etag);

        using var update = new HttpRequestMessage(HttpMethod.Patch, $"/api/v1/patients/{patientId}")
        {
            Content = JsonContent.Create(new { civilStatus = "Married" })
        };
        update.Headers.TryAddWithoutValidation("If-Match", etag);
        AddCsrf(update, cookie, token);
        var updated = await client.SendAsync(update);
        Assert.Equal(HttpStatusCode.OK, updated.StatusCode);
        Assert.Equal("\"2\"", updated.Headers.ETag?.ToString());
        var updatedBody = await updated.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Ana", updatedBody.GetProperty("firstName").GetString());
        Assert.Equal("Married", updatedBody.GetProperty("civilStatus").GetString());

        using var staleUpdate = new HttpRequestMessage(HttpMethod.Patch, $"/api/v1/patients/{patientId}")
        {
            Content = JsonContent.Create(new { nationality = "Filipino" })
        };
        staleUpdate.Headers.TryAddWithoutValidation("If-Match", "\"1\"");
        AddCsrf(staleUpdate, cookie, token);
        var stale = await client.SendAsync(staleUpdate);
        Assert.Equal(HttpStatusCode.PreconditionFailed, stale.StatusCode);
    }

    private static void SetIdentity(HttpClient client, string permissions, string? scope = null)
    {
        client.DefaultRequestHeaders.Remove("X-Test-Auth");
        client.DefaultRequestHeaders.Remove("X-Test-Permissions");
        client.DefaultRequestHeaders.Remove("X-Test-Scope");
        client.DefaultRequestHeaders.Add("X-Test-Auth", "true");
        client.DefaultRequestHeaders.Add("X-Test-Permissions", permissions);
        if (scope is not null) client.DefaultRequestHeaders.Add("X-Test-Scope", scope);
    }

    private static async Task<(string Cookie, string Token)> GetAntiforgeryAsync(HttpClient client)
    {
        using var response = await client.GetAsync("/api/v1/auth/antiforgery");
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        var setCookie = response.Headers.GetValues("Set-Cookie")
            .Single(value => value.StartsWith("LabTrack.AntiForgery=", StringComparison.Ordinal));
        return (setCookie.Split(';', 2)[0], body.GetProperty("requestToken").GetString()!);
    }

    private static void AddCsrf(HttpRequestMessage request, string cookie, string token)
    {
        request.Headers.TryAddWithoutValidation("Cookie", cookie);
        request.Headers.TryAddWithoutValidation("X-CSRF-TOKEN", token);
    }

    private sealed class PatientApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configuration) =>
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Persistence:Mode"] = "DevelopmentInMemory"
                }));
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<LabTrackDbContext>>();
                services.RemoveAll<LabTrackDbContext>();
                services.AddDbContext<LabTrackDbContext>(options =>
                    options.UseInMemoryDatabase(Guid.NewGuid().ToString("N")));

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                    options.DefaultForbidScheme = "Test";
                }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
            });
        }
    }

    private sealed class TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("X-Test-Auth"))
                return Task.FromResult(AuthenticateResult.NoResult());

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, "7"),
                new(ClaimTypes.Name, "integration-test")
            };
            claims.AddRange(Request.Headers["X-Test-Permissions"].ToString()
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(permission => new Claim("permission", permission)));
            var scope = Request.Headers["X-Test-Scope"].ToString();
            if (!string.IsNullOrWhiteSpace(scope))
                claims.Add(new Claim("patient_scope", scope));

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(
                new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name)));
        }
    }
}
