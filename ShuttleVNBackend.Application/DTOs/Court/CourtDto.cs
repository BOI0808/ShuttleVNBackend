using ShuttleVNBackend.Core.Entities.Court;

namespace ShuttleVNBackend.Application.DTOs.Court;

public record CourtDto(
    int CourtId,
    string Name,
    string Description,
    string Status,
    bool IsInUse,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public static CourtDto FromEntity(BadmintonCourt c, bool isInUse) => new(
        c.CourtId, c.Name, c.Description, c.Status.ToString().ToUpperInvariant(),
        isInUse, c.CreatedAt, c.UpdatedAt);
}