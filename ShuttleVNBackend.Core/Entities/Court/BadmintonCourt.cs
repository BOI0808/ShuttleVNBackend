using ShuttleVNBackend.Core.Entities.Court.Enums;

namespace ShuttleVNBackend.Core.Entities.Court;

public class BadmintonCourt
{
    public int CourtId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CourtStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public virtual ICollection<CourtSchedule> CourtSchedules { get; set; } = new HashSet<CourtSchedule>();
    public virtual ICollection<PricingRule> PricingRules { get; set; } = new HashSet<PricingRule>();
}