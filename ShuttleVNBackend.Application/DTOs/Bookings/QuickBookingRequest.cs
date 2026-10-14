namespace ShuttleVNBackend.Application.DTOs.Bookings;

public sealed record QuickBookingRequest(
    string FullName,
    string Phone,
    string Email,
    int CourtId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    decimal? ConfirmedTotalCost,
    string VerificationCode
);
