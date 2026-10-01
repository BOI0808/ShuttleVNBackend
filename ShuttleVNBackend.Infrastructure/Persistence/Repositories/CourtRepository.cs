using Microsoft.EntityFrameworkCore;
using ShuttleVNBackend.Application.Interfaces.Repositories;
using ShuttleVNBackend.Core.Entities.Court;

namespace ShuttleVNBackend.Infrastructure.Persistence.Repositories;

public class CourtRepository(ShuttleVnDbContext dbContext): ICourtRepository
{
    public async Task<List<BadmintonCourt>> GetAllAsync(CancellationToken ct = default)
    {
        return await dbContext.BadmintonCourts
            .AsNoTracking()
            .OrderBy(bc => bc.Name)
            .ToListAsync(ct);
    }

    public async Task<BadmintonCourt?> GetByIdAsync(int courtId, CancellationToken ct = default)
    {
        return await dbContext.BadmintonCourts
            .FirstOrDefaultAsync(bc => bc.CourtId == courtId, ct);
    }

    public Task<bool> NameExistsAsync(string name, int? excludeCourtId, CancellationToken ct = default)
    {
        return dbContext.BadmintonCourts.AnyAsync(bc =>
            bc.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase) &&
            (excludeCourtId == null || bc.CourtId != excludeCourtId), ct);
    }

    public async Task<List<CourtSchedule>?> GetSchedulesAsync(int courtId, CancellationToken ct = default)
    {
        return await dbContext.CourtSchedules
            .Where(s => s.CourtId == courtId)
            .OrderBy(s => s.DayOfWeek)
            .ToListAsync(ct);
    }
}