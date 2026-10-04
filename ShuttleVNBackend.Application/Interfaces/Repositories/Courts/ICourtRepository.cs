using ShuttleVNBackend.Core.Entities.Bookings;
using ShuttleVNBackend.Core.Entities.Courts;

namespace ShuttleVNBackend.Application.Interfaces.Repositories;

public record CourtGridSource(BadmintonCourt Court, IReadOnlyList<Booking> Bookings);

public interface ICourtRepository
{
    Task<List<BadmintonCourt>> GetAllAsync(CancellationToken ct = default);
    Task<BadmintonCourt?> GetByIdAsync(int courtId, CancellationToken ct = default);

    Task<IReadOnlyList<CourtGridSource>> GetAllForGridAsync(DateOnly date, int isoDayOfWeek,
        CancellationToken ct = default);

    Task<List<CourtSchedule>?> GetSchedulesAsync(int courtId, CancellationToken ct = default);
    Task<HashSet<int>> GetCourtIdsInUseAsync(DateOnly date, TimeOnly time, CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, int? excludeCourtId, CancellationToken ct = default);
}