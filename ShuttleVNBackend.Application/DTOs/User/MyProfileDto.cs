namespace ShuttleVNBackend.Application.DTOs.User;

public record MyProfileDto
{
    public Guid AccountId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}