using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Core.Entities.Booking;
using ShuttleVNBackend.Core.Entities.Court;
using ShuttleVNBackend.Core.Entities.Court.Enums;

namespace ShuttleVNBackend.Application.Interfaces.Repositories;

public record CourtGridSource(BadmintonCourt Court, IReadOnlyList<Booking> Bookings);

public interface ICourtRepository
{
    Task<PagedResult<BadmintonCourt>> GetAllAsync(PageRequest page, CourtStatus? status, string? search, CancellationToken ct = default);
    Task<BadmintonCourt?> GetByIdAsync(int courtId, CancellationToken ct = default);
    Task<IReadOnlyList<CourtGridSource>> GetAllForGridAsync(DateOnly date, int isoDayOfWeek, CancellationToken ct = default);
    Task<HashSet<int>> GetCourtIdsInUseAsync(DateOnly date, TimeOnly time, CancellationToken ct = default);
}