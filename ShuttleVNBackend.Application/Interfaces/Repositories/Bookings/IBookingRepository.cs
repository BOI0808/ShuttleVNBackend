using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Core.Entities.Bookings;
using ShuttleVNBackend.Core.Entities.Courts;

namespace ShuttleVNBackend.Application.Interfaces.Repositories.Bookings;

public interface IBookingRepository
{
    Task<int> CountUpcomingByCourtAsync(int courtId, DateOnly fromDate, CancellationToken ct = default);

    Task<Booking?> GetByIdAsync(Guid bookingId, CancellationToken ct = default);
    Task<Booking?> GetByIdWithDetailsAsync(Guid bookingId, CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
    Task<PagedResult<Booking>> GetByCustomerAsync(Guid customerId, PageRequest page, CancellationToken ct = default);

    Task<CourtSchedule?> GetScheduleAsync(int courtId, int dayOfWeek, CancellationToken ct = default);
    Task<List<PricingRule>> GetPricingRulesAsync(int courtId, int dayOfWeek, CancellationToken ct = default);
}
