using ShuttleVNBackend.Core.Entities.Courts.Enums;

namespace ShuttleVNBackend.Core.Entities.Courts;

public class BadmintonCourt
{
    public int CourtId { get; set; }
    public string? Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public CourtStatus Status { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    private ICollection<CourtSchedule>? _courtSchedules;
    public virtual ICollection<CourtSchedule> CourtSchedules
    {
        get => _courtSchedules ?? throw new InvalidOperationException("Uninitialized property: " + nameof(CourtSchedules));
        set => _courtSchedules = value;
    }

    private ICollection<PricingRule>? _pricingRules;
    public virtual ICollection<PricingRule> PricingRules
    {
        get => _pricingRules ?? throw new InvalidOperationException("Uninitialized property: " + nameof(PricingRules));
        set => _pricingRules = value;
    }
}