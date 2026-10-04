using Microsoft.EntityFrameworkCore;
using ShuttleVNBackend.Application.Interfaces.Repositories;
using ShuttleVNBackend.Application.Interfaces.Repositories.Bookings;
using ShuttleVNBackend.Core.Entities.Bookings.Enums;

namespace ShuttleVNBackend.Infrastructure.Persistence.Repositories.Bookings;

public class BookingRepository(ShuttleVnDbContext dbContext) : IBookingRepository
{
    public async Task<int> CountUpcomingByCourtAsync(int courtId, DateOnly fromDate, CancellationToken ct = default)
    {
        return await dbContext.Bookings.CountAsync(b =>
            b.CourtId == courtId && b.Date >= fromDate
                                 && (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed), ct);
    }
}