using Microsoft.EntityFrameworkCore;
using ShuttleVNBackend.Application.DTOs.Statistics;
using ShuttleVNBackend.Application.Interfaces.Repositories;
using ShuttleVNBackend.Application.Interfaces.Repositories.System;
using ShuttleVNBackend.Core.Entities.Bookings;
using ShuttleVNBackend.Core.Entities.Bookings.Enums;
using ShuttleVNBackend.Core.Entities.Courts;

namespace ShuttleVNBackend.Infrastructure.Persistence.Repositories.System;

public class StatisticsRepository(ShuttleVnDbContext dbContext) : IStatisticsRepository
{
    public async Task<IReadOnlyList<BadmintonCourt>> GetCourtsWithSchedulesAsync(CancellationToken ct = default)
    {
        return await dbContext.BadmintonCourts.AsNoTracking()
            .Include(c => c.CourtSchedules)
            .OrderBy(c => c.CourtId)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Booking>> GetBookingsInRangeAsync(
        DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        return await dbContext.Bookings.AsNoTracking()
            .Where(b => b.Date >= from && b.Date <= to && b.Status != BookingStatus.Pending)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<PaidInvoiceRow>> GetPaidInvoicesAsync(
        DateTime fromUtc, DateTime toUtcExclusive, CancellationToken ct = default)
    {
        return await dbContext.Invoices.AsNoTracking()
            .Where(i => i.Status == InvoiceStatus.Paid
                        && i.PaidAt >= fromUtc && i.PaidAt < toUtcExclusive)
            .Select(i => new PaidInvoiceRow(i.PaidAt!.Value, i.TotalCost))
            .ToListAsync(ct);
    }
}