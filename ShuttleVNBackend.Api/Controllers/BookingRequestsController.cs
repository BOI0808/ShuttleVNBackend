using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShuttleVNBackend.Api.Common;
using ShuttleVNBackend.Api.DTOs.Common;
using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Application.DTOs.Bookings;
using ShuttleVNBackend.Application.UseCases.Bookings.Services;

namespace ShuttleVNBackend.Api.Controllers;

[ApiController]
[Route("api/booking-requests")]
[Authorize(Policy = "StaffOnly")]
public class BookingRequestsController(BookingRequestService bookingRequestService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<BookingListItemDto>>>> GetPending(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var page = new PageRequest(pageNumber, pageSize);
        return Ok(ApiResponseFactory.Success(await bookingRequestService.GetPendingAsync(page, ct)));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<BookingDetailDto>>> GetDetail(
        [FromRoute] Guid id,
        CancellationToken ct = default)
    {
        return Ok(ApiResponseFactory.Success(await bookingRequestService.GetDetailAsync(id, ct)));
    }
}