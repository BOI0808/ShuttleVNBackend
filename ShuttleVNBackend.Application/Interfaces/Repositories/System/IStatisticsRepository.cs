using ShuttleVNBackend.Core.Entities.Bookings;
using ShuttleVNBackend.Core.Entities.Courts;

namespace ShuttleVNBackend.Application.Interfaces.Repositories.System;

public interface IStatisticsRepository
{
    Task<IReadOnlyList<BadmintonCourt>> GetCourtsWithSchedulesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Booking>> GetBookingsInRangeAsync(DateOnly from, DateOnly to, CancellationToken ct = default);
}