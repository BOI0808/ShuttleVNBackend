using ShuttleVNBackend.Core.Entities.Court;

namespace ShuttleVNBackend.Application.Interfaces.Repositories;

public interface ICourtRepository
{
    public Task<List<BadmintonCourt>> GetAllAsync(CancellationToken ct = default);
    public Task<BadmintonCourt?> GetByIdAsync(int courtId, CancellationToken ct = default);
    public Task<bool> NameExistsAsync(string name, int? excludeCourtId, CancellationToken ct = default);
    public Task<List<CourtSchedule>?> GetSchedulesAsync(int courtId, CancellationToken ct = default);
}