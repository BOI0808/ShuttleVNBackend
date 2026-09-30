using ShuttleVNBackend.Core.Entities.Booking;
using ShuttleVNBackend.Core.Entities.Court;

namespace ShuttleVNBackend.Application.Interfaces.Repositories;

public interface IStatisticsRepository
{
    Task<IReadOnlyList<BadmintonCourt>> GetCourtsWithSchedulesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Booking>> GetBookingsInRangeAsync(DateOnly from, DateOnly to, CancellationToken ct = default);
}