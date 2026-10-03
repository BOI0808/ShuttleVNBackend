using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShuttleVNBackend.Api.Common;
using ShuttleVNBackend.Api.Extensions;
using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Application.DTOs.Court;
using ShuttleVNBackend.Application.UseCases.Court.Services;
using ShuttleVNBackend.Core.Entities.Court.Enums;

namespace ShuttleVNBackend.Api.Controllers;

[ApiController]
[Route("api/courts")]
public class CourtsController(CourtService courtService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ListCourts(CancellationToken ct = default)
        => Ok(ApiResponseFactory.Success(await courtService.GetCourtsAsync(ct)));

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourt([FromRoute] int id, CancellationToken ct = default)
    {
        var court = await courtService.GetCourtByIdAsync(id, ct);
        return court is null
            ? NotFound(new ProblemDetails { Title = "Resource not found" })
            : Ok(ApiResponseFactory.Success(court));
    }

    [HttpGet("grid")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourtGrid([FromQuery] DateOnly? date, CancellationToken ct = default)
        => Ok(ApiResponseFactory.Success(await courtService.GetCourtGridAsync(date, ct)));

    [HttpPatch("{id:int}/status")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<IActionResult> UpdateCourtStatus(
        [FromRoute] int id, [FromBody] UpdateCourtStatusDto dto, CancellationToken ct = default)
    {
        var actorId = User.GetAccountId();
        return Ok(ApiResponseFactory.Success(await courtService.UpdateCourtStatusAsync(id, dto, actorId, ct)));
    }
}