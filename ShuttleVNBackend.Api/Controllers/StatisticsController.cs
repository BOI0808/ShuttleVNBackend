using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShuttleVNBackend.Api.Common;
using ShuttleVNBackend.Application.UseCases.Statistics.Services;

namespace ShuttleVNBackend.Api.Controllers;

[ApiController]
[Route("api/admin/statistics")]
[Authorize(Policy = "AdminOnly")]
public class StatisticsController(StatisticsService statisticsService) : ControllerBase
{
    [HttpGet("court-usage")]
    public async Task<IActionResult> GetCourtUsage(
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        CancellationToken ct = default)
        => Ok(ApiResponseFactory.Success(await statisticsService.GetCourtUsageAsync(fromDate, toDate, ct)));
}