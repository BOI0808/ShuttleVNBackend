namespace ShuttleVNBackend.Application.DTOs.Courts;

public record UpdateCourtScheduleDto
{
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
    public bool IsAvailable { get; set; }
}