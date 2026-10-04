namespace ShuttleVNBackend.Application.DTOs.Users;

public record UpdateProfileDto
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}