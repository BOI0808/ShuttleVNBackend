using ShuttleVNBackend.Core.Entities.Courts;

namespace ShuttleVNBackend.Application.UseCases.Courts.Helpers;

internal static class PricingRuleNormalizer
{
    // Mutates the surviving first rule, returns the rules that must be deleted.
    public static IReadOnlyList<PricingRule> Normalize(
        IReadOnlyCollection<PricingRule> rules,
        TimeOnly open,
        TimeOnly close,
        DateTime now)
    {
        if (rules.Count == 0) return [];

        var sorted = rules.OrderBy(r => r.StartTime).ToList();
        // rule in effect at opening (falls back to the earliest if open is before all rules)
        var active = sorted.LastOrDefault(r => r.StartTime <= open) ?? sorted[0];
        var removed = sorted
            .Where(r => r != active && (r.StartTime <= open || r.StartTime >= close))
            .ToList();

        if (active.StartTime == open) return removed;
        
        active.StartTime = open;
        active.UpdatedAt = now;
        return removed;
    }
}