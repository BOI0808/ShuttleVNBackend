namespace ShuttleVNBackend.Application.DTOs.Courts;

public record UpdatePricingRuleDto
{
    public int DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public decimal PricePerHour { get; set; }
}