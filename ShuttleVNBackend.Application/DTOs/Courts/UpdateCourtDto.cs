namespace ShuttleVNBackend.Application.DTOs.Courts;

public record UpdateCourtDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}