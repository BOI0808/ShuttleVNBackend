using Microsoft.EntityFrameworkCore;
using ShuttleVNBackend.Application.Interfaces.Repositories;
using ShuttleVNBackend.Application.Interfaces.Repositories.Courts;
using ShuttleVNBackend.Core.Entities.Courts;

namespace ShuttleVNBackend.Infrastructure.Persistence.Repositories.Courts;

public class PricingRuleRepository(ShuttleVnDbContext dbContext) : IPricingRuleRepository
{
    public async Task<PricingRule?> GetByIdAsync(int pricingRuleId, CancellationToken ct = default)
    {
        return await dbContext.PricingRules
            .FirstOrDefaultAsync(r => r.PricingRuleId == pricingRuleId, ct);
    }

    public async Task<List<PricingRule>?> GetByCourtAsync(int courtId, int? dayOfWeek, CancellationToken ct = default)
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
}