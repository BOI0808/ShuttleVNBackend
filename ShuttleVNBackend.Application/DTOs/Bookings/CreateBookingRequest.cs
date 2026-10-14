namespace ShuttleVNBackend.Application.DTOs.Bookings;

public sealed record CreateBookingRequest(
    int CourtId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    decimal? ConfirmedTotalCost
);
