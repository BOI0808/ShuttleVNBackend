using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShuttleVNBackend.Api.Common;
using ShuttleVNBackend.Api.DTOs.Common;
using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Application.DTOs.Bookings;
using ShuttleVNBackend.Application.UseCases.Bookings.Services;

namespace ShuttleVNBackend.Api.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingController(BookingService bookingService) : ControllerBase
{
    [HttpGet("price-preview")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<PricePreviewResponse>>> GetPricePreview(
        [FromQuery] int courtId,
        [FromQuery] DateOnly date,
        [FromQuery] TimeOnly startTime,
        [FromQuery] TimeOnly endTime,
        CancellationToken ct = default)
    {
        var result = await bookingService.GetPricePreviewAsync(
            new PricePreviewDto(courtId, date, startTime, endTime), ct);
        return Ok(ApiResponseFactory.Success(result));
    }

    [HttpPost]
    [Authorize(Policy = "CustomerOnly")]
    public async Task<ActionResult<ApiResponse<BookingDto>>> CreateBooking(
        [FromBody] CreateBookingDto dto,
        CancellationToken ct = default)
    {
        var result = await bookingService.CreateBookingAsync(dto, ct);
        return Ok(ApiResponseFactory.Success(result));
    }

    [HttpPost("quick")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<BookingDto>>> QuickBooking(
        [FromBody] QuickBookingDto dto,
        CancellationToken ct = default)
    {
        var result = await bookingService.QuickBookingAsync(dto, ct);
        return Ok(ApiResponseFactory.Success(result));
    }

    [HttpPatch("{bookingId:guid}/cancel")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<BookingDto>>> CancelBooking(
        [FromRoute] Guid bookingId,
        [FromBody] CancelBookingDto dto,
        CancellationToken ct = default)
    {
        var result = await bookingService.CancelBookingAsync(bookingId, dto, ct);
        return Ok(ApiResponseFactory.Success(result));
    }

    [HttpGet("me")]
    [Authorize(Policy = "CustomerOnly")]
    public async Task<ActionResult<ApiResponse<PagedResult<BookingDto>>>> GetMyBookings(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var page = new PageRequest(pageNumber, pageSize);
        var result = await bookingService.GetOwnBookingsAsync(page, ct);
        return Ok(ApiResponseFactory.Success(result));
    }

    [HttpGet("{bookingId:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<BookingDto>>> GetBookingById(
        [FromRoute] Guid bookingId,
        CancellationToken ct = default)
    {
        var result = await bookingService.GetBookingByIdAsync(bookingId, ct);
        return Ok(ApiResponseFactory.Success(result));
    }
    
    [HttpGet("{bookingId:guid}/invoice")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<InvoiceDto>>> GetInvoice(
        [FromRoute] Guid bookingId,
        CancellationToken ct = default)
    {
        var result = await bookingService.GetBookingInvoiceAsync(bookingId, ct);
        return Ok(ApiResponseFactory.Success(result));
    }
}