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
    public async Task PatientReadsAndDemographicEdits_AreDeniedUntilRecordScopeIsImplemented()
    {
        using var factory = new PatientApiFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });

        var anonymous = await client.GetAsync("/api/v1/patients");
        Assert.Equal(HttpStatusCode.Unauthorized, anonymous.StatusCode);

        SetIdentity(client, "patients.read");
        var withoutScope = await client.GetAsync("/api/v1/patients");
        Assert.Equal(HttpStatusCode.Forbidden, withoutScope.StatusCode);

        // A caller-supplied/broad all-record scope claim must not bypass the missing object-scope model.
        SetIdentity(client, "patients.read", scope: "all");
        var broadScopeList = await client.GetAsync("/api/v1/patients");
        Assert.Equal(HttpStatusCode.Forbidden, broadScopeList.StatusCode);
        var broadScopeDetail = await client.GetAsync("/api/v1/patients/1");
        Assert.Equal(HttpStatusCode.Forbidden, broadScopeDetail.StatusCode);

        SetIdentity(client, "patients.demographics.update", scope: "all");
        using var patch = new HttpRequestMessage(HttpMethod.Patch, "/api/v1/patients/1")
        {
            Content = JsonContent.Create(new { mobileNumber = "09999999999" })
        };
        var update = await client.SendAsync(patch);
        Assert.Equal(HttpStatusCode.Forbidden, update.StatusCode);
    }

    [Fact]
    public async Task Register_RequiresCsrfAndReturnsOnlyMinimalReceipt()
    {
        using var factory = new PatientApiFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });
        SetIdentity(client, "patients.register");

        var missingCsrf = await client.PostAsJsonAsync("/api/v1/patients", new
        {
            firstName = "Ana",
            lastName = "Reyes",
            mobileNumber = "09170000000"
        });
        Assert.Equal(HttpStatusCode.BadRequest, missingCsrf.StatusCode);

        var (cookie, token) = await GetAntiforgeryAsync(client);
        using var register = new HttpRequestMessage(HttpMethod.Post, "/api/v1/patients")
        {
            Content = JsonContent.Create(new
            {
                firstName = "Ana",
                lastName = "Reyes",
                mobileNumber = "09170000000",
                addressLine1 = "123 Example Street"
            })
        };
        AddCsrf(register, cookie, token);
        var created = await client.SendAsync(register);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        Assert.True(created.Headers.Location is not null);
        var receipt = await created.Content.ReadFromJsonAsync<JsonElement>();
        Assert.False(string.IsNullOrWhiteSpace(receipt.GetProperty("id").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(receipt.GetProperty("patientNumber").GetString()));
        Assert.True(receipt.TryGetProperty("eTag", out _));
        Assert.False(receipt.TryGetProperty("mobileNumber", out _));
        Assert.False(receipt.TryGetProperty("addressLine1", out _));
        Assert.False(receipt.TryGetProperty("dateOfBirth", out _));
        Assert.Equal("\"1\"", created.Headers.ETag?.ToString());
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
                var databaseName = Guid.NewGuid().ToString("N");
                services.AddDbContext<LabTrackDbContext>(options =>
                    options.UseInMemoryDatabase(databaseName));

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
