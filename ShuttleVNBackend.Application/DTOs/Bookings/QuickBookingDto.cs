namespace ShuttleVNBackend.Application.DTOs.Bookings;

public sealed record QuickBookingDto(
    string FullName,
    string Phone,
    string Email,
    int CourtId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    decimal? TotalCost,
    string VerificationCode
): CreateBookingDto(
    CourtId,
    Date,
    StartTime,
    EndTime,
    TotalCost
);
