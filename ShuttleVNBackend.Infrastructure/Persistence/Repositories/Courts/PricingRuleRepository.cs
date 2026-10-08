using Microsoft.EntityFrameworkCore;
using ShuttleVNBackend.Application.Interfaces.Repositories.Courts;
using ShuttleVNBackend.Core.Entities.Courts;

namespace ShuttleVNBackend.Infrastructure.Persistence.Repositories.Courts;

public class PricingRuleRepository(ShuttleVnDbContext dbContext) : IPricingRuleRepository
{
    public async Task<List<PricingRule>> GetByCourtAsync(
        int courtId, int? dayOfWeek, CancellationToken ct = default)
    {
        var query = dbContext.PricingRules
            .AsNoTracking()
            .Where(r => r.CourtId == courtId);

        if (dayOfWeek.HasValue)
            query = query.Where(r => r.DayOfWeek == dayOfWeek.Value);

        return await query
            .OrderBy(r => r.DayOfWeek)
            .ThenBy(r => r.StartTime)
            .ToListAsync(ct);
    }

    public async Task<List<PricingRule>> ReplaceDayAsync(
        int courtId,
        int dayOfWeek,
        IReadOnlyCollection<PricingRule> pricingRules,
        CancellationToken ct = default)
    {
        var oldRules = await dbContext.PricingRules
            .Where(r => r.CourtId == courtId && r.DayOfWeek == dayOfWeek)
            .ToListAsync(ct);

        dbContext.PricingRules.RemoveRange(oldRules);
        dbContext.PricingRules.AddRange(pricingRules);
        await dbContext.SaveChangesAsync(ct);

        return pricingRules.OrderBy(r => r.StartTime).ToList();
    }
}