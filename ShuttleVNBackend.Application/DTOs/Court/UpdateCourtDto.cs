namespace ShuttleVNBackend.Application.DTOs.Court;

public record UpdateCourtDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}