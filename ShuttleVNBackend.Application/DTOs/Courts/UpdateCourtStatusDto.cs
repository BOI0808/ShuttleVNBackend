using ShuttleVNBackend.Core.Entities.Courts.Enums;

namespace ShuttleVNBackend.Application.DTOs.Courts;

public record UpdateCourtStatusDto(CourtStatus Status, string? Reason);

public record UpdateCourtStatusResultDto(CourtDto Court, int AffectedUpcomingBookingsCount);