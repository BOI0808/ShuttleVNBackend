using Microsoft.EntityFrameworkCore;
using ShuttleVNBackend.Application.Interfaces.Repositories;
using ShuttleVNBackend.Core.Entities.Booking;
using ShuttleVNBackend.Core.Entities.Booking.Enums;
using ShuttleVNBackend.Core.Entities.Court;

namespace ShuttleVNBackend.Infrastructure.Persistence.Repositories;

public class StatisticsRepository(ShuttleVnDbContext dbContext) : IStatisticsRepository
{
    public async Task<IReadOnlyList<BadmintonCourt>> GetCourtsWithSchedulesAsync(CancellationToken ct = default)
        => await dbContext.BadmintonCourts.AsNoTracking()
            .Include(c => c.CourtSchedules)
            .OrderBy(c => c.CourtId)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Booking>> GetBookingsInRangeAsync(
        DateOnly from, DateOnly to, CancellationToken ct = default)
        => await dbContext.Bookings.AsNoTracking()
            .Where(b => b.Date >= from && b.Date <= to && b.Status != BookingStatus.Pending)
            .ToListAsync(ct);
}