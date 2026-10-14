using Microsoft.EntityFrameworkCore;
using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Application.Interfaces.Repositories.Bookings;
using ShuttleVNBackend.Core.Entities.Bookings;
using ShuttleVNBackend.Core.Entities.Bookings.Enums;
using ShuttleVNBackend.Core.Entities.Courts;

namespace ShuttleVNBackend.Infrastructure.Persistence.Repositories.Bookings;

public class BookingRepository(ShuttleVnDbContext dbContext) : IBookingRepository
{
    public async Task<int> CountUpcomingByCourtAsync(int courtId, DateOnly fromDate, CancellationToken ct = default)
    {
        return await dbContext.Bookings.CountAsync(b =>
            b.CourtId == courtId && b.Date >= fromDate
            && (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed), ct);
    }

    public async Task<Booking?> GetByIdAsync(Guid bookingId, CancellationToken ct = default)
    {
        return await dbContext.Bookings
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.BookingId == bookingId, ct);
    }

    public async Task<Booking?> GetByIdWithDetailsAsync(Guid bookingId, CancellationToken ct = default)
    {
        return await dbContext.Bookings
            .Include(b => b.Customer)
            .Include(b => b.Court)
            .Include(b => b.PriceSegments)
            .Include(b => b.Invoices)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId, ct);
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
    {
        return await dbContext.Bookings.AnyAsync(b => b.BookingCode == code, ct);
    }

    public async Task<PagedResult<Booking>> GetByCustomerAsync(
        Guid customerId, PageRequest page, CancellationToken ct = default)
    {
        var query = dbContext.Bookings
            .Include(b => b.Court)
            .Include(b => b.PriceSegments)
            .Include(b => b.Invoices)
            .Where(b => b.CustomerId == customerId)
            .OrderByDescending(b => b.Date)
            .ThenByDescending(b => b.StartTime)
            .ThenByDescending(b => b.CreatedAt)
            .ThenBy(b => b.BookingId);

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((page.PageNumber - 1) * page.PageSize)
            .Take(page.PageSize)
            .ToListAsync(ct);

        return new PagedResult<Booking>
        {
            Items = items,
            PageNumber = page.PageNumber,
            PageSize = page.PageSize,
            TotalCount = total
        };
    }

    public async Task<CourtSchedule?> GetScheduleAsync(int courtId, int dayOfWeek, CancellationToken ct = default)
    {
        return await dbContext.CourtSchedules
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.CourtId == courtId && s.DayOfWeek == dayOfWeek, ct);
    }

    public async Task<List<PricingRule>> GetPricingRulesAsync(int courtId, int dayOfWeek, CancellationToken ct = default)
    {
        return await dbContext.PricingRules
            .AsNoTracking()
            .Where(r => r.CourtId == courtId && r.DayOfWeek == dayOfWeek)
            .OrderBy(r => r.StartTime)
            .ToListAsync(ct);
    }
}
