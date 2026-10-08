namespace ShuttleVNBackend.Application.DTOs.Courts;

public sealed record SavePricingRuleDto
{
    public List<SavePricingRuleItemDto> PricingRules { get; init; } = [];
}

public sealed record SavePricingRuleItemDto
{
    public TimeOnly StartTime { get; init; }
    public decimal PricePerHour { get; init; }
}