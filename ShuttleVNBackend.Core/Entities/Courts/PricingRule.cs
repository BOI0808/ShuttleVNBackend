namespace ShuttleVNBackend.Core.Entities.Courts;

public class PricingRule
{
    public int PricingRuleId { get; set; }
    public int CourtId { get; set; }
    public int DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public decimal PricePerHour { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}