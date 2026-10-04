using ShuttleVNBackend.Core.Entities.Courts;

namespace ShuttleVNBackend.Application.DTOs.Courts;

public record CourtDto(
    int CourtId,
    string Name,
    string Description,
    string Status,
    bool IsInUse,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public static CourtDto FromEntity(BadmintonCourt c, bool isInUse)
    {
        return new CourtDto(
            c.CourtId, c.Name, c.Description, c.Status.ToString().ToUpperInvariant(),
            isInUse, c.CreatedAt, c.UpdatedAt);
    }
}