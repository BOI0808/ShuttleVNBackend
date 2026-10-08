using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShuttleVNBackend.Api.Common;
using ShuttleVNBackend.Api.DTOs.Common;
using ShuttleVNBackend.Application.DTOs.Courts;
using ShuttleVNBackend.Application.UseCases.Courts.Services;
using ShuttleVNBackend.Core.Entities.Courts;

namespace ShuttleVNBackend.Api.Controllers;

[ApiController]
[Route("api/courts/{id:int}/schedules")]
public class CourtScheduleController(CourtScheduleService scheduleService) : ControllerBase
{
    [HttpGet("")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<ApiResponse<List<CourtSchedule>>>> GetCourtSchedules(
        [FromRoute] int id,
        CancellationToken ct = default)
    {
        var schedules = await scheduleService.GetSchedulesAsync(id, ct);
        return Ok(ApiResponseFactory.Success(schedules));
    }

    [HttpPut("{dayOfWeek:int}")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<ApiResponse<CourtSchedule>>> UpdateCourtSchedule(
        [FromRoute] int id,
        [FromRoute] int dayOfWeek,
        [FromBody] UpdateCourtScheduleDto dto,
        CancellationToken ct = default)
    {
        var schedule = await scheduleService.UpdateScheduleAsync(id, dayOfWeek, dto, ct);
        return Ok(ApiResponseFactory.Success(schedule));
    }
}