namespace ShuttleVNBackend.Application.DTOs.Court;

public class CreateCourtDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public TimeOnly DefaultOpenTime { get; set; }
    public TimeOnly DefaultCloseTime { get; set; }
    public decimal DefaultPricePerHour { get; set; }
}