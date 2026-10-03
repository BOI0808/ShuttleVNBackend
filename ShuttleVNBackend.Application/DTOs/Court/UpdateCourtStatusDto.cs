using ShuttleVNBackend.Core.Entities.Court.Enums;

namespace ShuttleVNBackend.Application.DTOs.Court;

public record UpdateCourtStatusDto(CourtStatus Status, string? Reason);

public record UpdateCourtStatusResultDto(CourtDto Court, int AffectedUpcomingBookingsCount);