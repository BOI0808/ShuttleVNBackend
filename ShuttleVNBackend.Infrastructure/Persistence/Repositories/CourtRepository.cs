using Microsoft.EntityFrameworkCore;
using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Application.Interfaces.Repositories;
using ShuttleVNBackend.Core.Entities.Booking;
using ShuttleVNBackend.Core.Entities.Booking.Enums;
using ShuttleVNBackend.Core.Entities.Court;
using ShuttleVNBackend.Core.Entities.Court.Enums;

namespace ShuttleVNBackend.Infrastructure.Persistence.Repositories;

public class CourtRepository(ShuttleVnDbContext dbContext) : ICourtRepository
{
    public async Task<PagedResult<BadmintonCourt>> GetAllAsync(
        PageRequest page, CourtStatus? status, string? search, CancellationToken ct = default)
    {
        var query = dbContext.BadmintonCourts.AsNoTracking();

        if (status is { } s)
            query = query.Where(c => c.Status == s);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(c => EF.Functions.ILike(c.Name, pattern));
        }

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderBy(c => c.CourtId)
            .Skip((page.PageNumber - 1) * page.PageSize)
            .Take(page.PageSize)
            .ToListAsync(ct);

        return new PagedResult<BadmintonCourt>
        {
            Items = items,
            PageNumber = page.PageNumber,
            PageSize = page.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<BadmintonCourt?> GetByIdAsync(int courtId, CancellationToken ct = default)
        => await dbContext.BadmintonCourts.FirstOrDefaultAsync(c => c.CourtId == courtId, ct);

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

    public async Task<int> CountUpcomingBookingsAsync(int courtId, DateOnly fromDate, CancellationToken ct = default)
    => await dbContext.Bookings.CountAsync(b =>
        b.CourtId == courtId && b.Date >= fromDate
        && (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed), ct);
}