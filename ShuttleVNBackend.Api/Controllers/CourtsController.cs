using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShuttleVNBackend.Api.Common;
using ShuttleVNBackend.Api.DTOs.Common;
using ShuttleVNBackend.Api.Extensions;
using ShuttleVNBackend.Application.DTOs.Courts;
using ShuttleVNBackend.Application.UseCases.Courts.Services;
using ShuttleVNBackend.Core.Entities.Courts;

namespace ShuttleVNBackend.Api.Controllers;

[ApiController]
[Route("api/courts")]
public class CourtsController(CourtService courtService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<BadmintonCourt>>>> ListCourts(
        CancellationToken ct = default)
    {
        return Ok(ApiResponseFactory.Success(await courtService.GetCourtsAsync(ct)));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<BadmintonCourt>>> GetCourt(
        [FromRoute] int id,
        CancellationToken ct = default)
    {
        var court = await courtService.GetCourtByIdAsync(id, ct);
        return court is null
            ? NotFound(new ProblemDetails { Title = "Resource not found" })
            : Ok(ApiResponseFactory.Success(court));
    }

    [HttpGet("grid")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<BadmintonCourt>>>> GetCourtGrid(
        [FromQuery] DateOnly? date,
        CancellationToken ct = default)
    {
        var courtGrid = await courtService.GetCourtGridAsync(date, ct);
        return Ok(ApiResponseFactory.Success(courtGrid));
    }

    [HttpPost]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<ApiResponse<BadmintonCourt>>> CreateCourt(
        [FromBody] CreateCourtDto dto,
        CancellationToken ct = default)
    {
        var court = await courtService.CreateCourtAsync(dto, ct);
        return Ok(ApiResponseFactory.Success(court));
    }

    [HttpPatch("{id:int}")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<ApiResponse<BadmintonCourt>>> UpdateCourt(
        [FromRoute] int id,
        [FromBody] UpdateCourtDto dto,
        CancellationToken ct = default)
    {
        var court = await courtService.UpdateCourtAsync(id, dto, ct);
        return Ok(ApiResponseFactory.Success(court));
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateCourtStatus(
        [FromRoute] int id,
        [FromBody] UpdateCourtStatusDto dto,
        CancellationToken ct = default)
    {
        var actorId = User.GetAccountId();
        return Ok(ApiResponseFactory.Success(await courtService.UpdateCourtStatusAsync(id, dto, actorId, ct)));
    }
}