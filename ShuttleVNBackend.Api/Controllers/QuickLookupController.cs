using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShuttleVNBackend.Api.Common;
using ShuttleVNBackend.Api.DTOs.Common;
using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Application.DTOs.Bookings;
using ShuttleVNBackend.Application.UseCases.Bookings.Services;

namespace ShuttleVNBackend.Api.Controllers;

[ApiController]
[Route("api/quick-lookup")]
[Authorize(Policy = "StaffOnly")]
public class QuickLookupController(QuickLookupService quickLookupService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<QuickLookupResultDto>>> Lookup(
        [FromQuery] string? keyword,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var page = new PageRequest(pageNumber, pageSize);
        return Ok(ApiResponseFactory.Success(await quickLookupService.LookupAsync(keyword, page, ct)));
    }
}