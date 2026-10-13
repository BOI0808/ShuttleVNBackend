using ShuttleVNBackend.Core.Entities.Bookings;

namespace ShuttleVNBackend.Application.Interfaces.Repositories.Bookings;

public interface IBookingRepository
{
    Task<int> CountUpcomingByCourtAsync(int courtId, DateOnly fromDate, CancellationToken ct = default);
    Task<Booking?> GetByIdForUpdateAsync(Guid bookingId, CancellationToken ct = default);
}