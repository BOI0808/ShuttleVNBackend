using ShuttleVNBackend.Core.Entities.Courts;

namespace ShuttleVNBackend.Application.Interfaces.Repositories.Courts;

public interface IPricingRuleRepository
{
    public Task<PricingRule?> GetByIdAsync(int pricingRuleId, CancellationToken ct = default);
    public Task<List<PricingRule>?> GetByCourtAsync(int courtId, int? dayOfWeek, CancellationToken ct = default);
}