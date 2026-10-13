using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Application.DTOs.Bookings;
using ShuttleVNBackend.Application.Exceptions;
using ShuttleVNBackend.Application.Interfaces.Repositories.Bookings;
using ShuttleVNBackend.Application.UseCases.Bookings.Helpers;

namespace ShuttleVNBackend.Application.UseCases.Bookings.Services;

public class BookingRequestService(IBookingRepository bookingRepository)
{
    public async Task<PagedResult<BookingListItemDto>> GetPendingAsync(PageRequest page, CancellationToken ct = default)
    {
        var rows = await bookingRepository.GetPendingAsync(page, ct);
        return new PagedResult<BookingListItemDto>
        {
            Items = rows.Items.Select(r => r.ToListItem()).ToList(),
            PageNumber = rows.PageNumber,
            PageSize = rows.PageSize,
            TotalCount = rows.TotalCount
        };
    }

    public async Task<BookingDetailDto> GetDetailAsync(Guid bookingId, CancellationToken ct = default)
    {
        var row = await bookingRepository.GetRowByIdAsync(bookingId, ct)
                  ?? throw new NotFoundException("Booking not found");

        var invoice = await bookingRepository.GetValidInvoiceAsync(bookingId, ct);
        var history = await bookingRepository.GetStatusHistoryAsync(bookingId, ct);
        return row.ToDetail(invoice, history);
    }
}