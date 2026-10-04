using Microsoft.EntityFrameworkCore;
using ShuttleVNBackend.Application.Interfaces.Repositories;
using ShuttleVNBackend.Core.Entities.Bookings.Enums;
using ShuttleVNBackend.Core.Entities.Courts;

namespace ShuttleVNBackend.Infrastructure.Persistence.Repositories.Courts;

public class CourtRepository(ShuttleVnDbContext dbContext) : ICourtRepository
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
        return await dbContext.BadmintonCourts.FirstOrDefaultAsync(c => c.CourtId == courtId, ct);
    }

    public async Task<bool> NameExistsAsync(string name, int? excludeCourtId, CancellationToken ct = default)
    {
        return await dbContext.BadmintonCourts.AnyAsync(bc =>
            EF.Functions.ILike(bc.Name, name) &&
            (excludeCourtId == null || bc.CourtId != excludeCourtId), ct);
    }

    public async Task<List<CourtSchedule>?> GetSchedulesAsync(int courtId, CancellationToken ct = default)
    {
        return await dbContext.CourtSchedules
            .Where(s => s.CourtId == courtId)
            .OrderBy(s => s.DayOfWeek)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CourtGridSource>> GetAllForGridAsync(
        DateOnly date, int isoDayOfWeek, CancellationToken ct = default)
    {
        var courts = await dbContext.BadmintonCourts.AsNoTracking()
            .Include(c => c.CourtSchedules.Where(s => s.DayOfWeek == isoDayOfWeek))
            .Include(c => c.PricingRules.Where(p => p.DayOfWeek == isoDayOfWeek))
            .OrderBy(c => c.CourtId)
            .ToListAsync(ct);

        var bookings = await dbContext.Bookings.AsNoTracking()
            .Where(b => b.Date == date && b.Status != BookingStatus.Cancelled)
            .ToListAsync(ct);

        var byCourt = bookings.ToLookup(b => b.CourtId);
        return courts
            .Select(c => new CourtGridSource(c, byCourt[c.CourtId].ToList()))
            .ToList();
    }

    public async Task<HashSet<int>> GetCourtIdsInUseAsync(
        DateOnly date, TimeOnly time, CancellationToken ct = default)
    {
        var ids = await dbContext.Bookings.AsNoTracking()
            .Where(b => b.Date == date
                        && (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed)
                        && b.StartTime <= time && time < b.EndTime)
            .Select(b => b.CourtId)
            .Distinct()
            .ToListAsync(ct);
        return [.. ids];
    }
}