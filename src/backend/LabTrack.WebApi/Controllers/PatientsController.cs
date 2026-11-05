using System.Globalization;
using System.Security.Claims;
using LabTrack.Application.Patients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabTrack.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/patients")]
public sealed class PatientsController(IPatientService patients) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "patients.read")]
    public async Task<IActionResult> Search(
        [FromQuery] string? search, [FromQuery] string? status = null,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 25,
        [FromQuery] string? sort = null, CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize is < 1 or > 100)
            return ValidationProblem(new Dictionary<string, string[]> {
                ["pagination"] = ["Page must be at least 1 and pageSize must be between 1 and 100."]
            });
        if (search?.Length > 100 || status?.Length > 20)
            return ValidationProblem(new Dictionary<string, string[]> {
                ["filter"] = ["Search and status filters exceed their allowed length."]
            });
        if (sort is not null && !new[] { "lastName", "firstName", "patientNumber", "registeredAt" }.Contains(sort, StringComparer.OrdinalIgnoreCase))
            return ValidationProblem(new Dictionary<string, string[]> {
                ["sort"] = ["Sort must be one of: lastName, firstName, patientNumber, registeredAt."]
            });

        var result = await patients.SearchAsync(search, status, page, pageSize, sort, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "patients.register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        [FromBody] RegisterPatientRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetActorId(out var actorId)) return Unauthorized();
        var patient = await patients.RegisterAsync(request, actorId, cancellationToken);
        Response.Headers["ETag"] = patient.ETag;
        return CreatedAtAction(nameof(GetById), new { patientId = patient.Id }, patient);
    }

    [HttpGet("{patientId}")]
    [Authorize(Policy = "patients.read")]
    public async Task<IActionResult> GetById(string patientId, CancellationToken cancellationToken)
    {
        if (!long.TryParse(patientId, NumberStyles.None, CultureInfo.InvariantCulture, out var id) || id <= 0)
            return BadRequest(new ProblemDetails { Title = "Invalid patient identifier.", Status = StatusCodes.Status400BadRequest });
        var patient = await patients.GetAsync(id, cancellationToken);
        if (patient is null) return NotFound();
        Response.Headers["ETag"] = patient.ETag;
        return Ok(patient);
    }

    [HttpPatch("{patientId}")]
    [Authorize(Policy = "patients.demographics.update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateDemographics(
        string patientId, [FromBody] UpdatePatientDemographicsRequest request,
        CancellationToken cancellationToken)
    {
        if (!long.TryParse(patientId, NumberStyles.None, CultureInfo.InvariantCulture, out var id) || id <= 0)
            return BadRequest(new ProblemDetails { Title = "Invalid patient identifier.", Status = StatusCodes.Status400BadRequest });
        if (!TryGetActorId(out var actorId)) return Unauthorized();

        var ifMatch = Request.Headers["If-Match"].ToString();
        if (string.IsNullOrWhiteSpace(ifMatch))
            return StatusCode(StatusCodes.Status428PreconditionRequired,
                new ProblemDetails { Title = "If-Match is required.", Status = StatusCodes.Status428PreconditionRequired });
        if (!ifMatch.StartsWith('"') || !ifMatch.EndsWith('"') ||
            !long.TryParse(ifMatch.Trim('"'), NumberStyles.None, CultureInfo.InvariantCulture, out var rowVersion) || rowVersion <= 0)
            return BadRequest(new ProblemDetails { Title = "Invalid If-Match value.", Status = StatusCodes.Status400BadRequest });

        try
        {
            var patient = await patients.UpdateDemographicsAsync(id, request, actorId, rowVersion, cancellationToken);
            if (patient is null) return NotFound();
            Response.Headers["ETag"] = patient.ETag;
            return Ok(patient);
        }
        catch (DbUpdateConcurrencyException)
        {
            return StatusCode(StatusCodes.Status412PreconditionFailed,
                new ProblemDetails { Title = "The patient record has changed. Reload before updating.", Status = StatusCodes.Status412PreconditionFailed });
        }
    }

    private bool TryGetActorId(out long actorId) =>
        long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), NumberStyles.None,
            CultureInfo.InvariantCulture, out actorId) && actorId > 0;
}
