namespace ShuttleVNBackend.Application.DTOs.Bookings;

public record CreateBookingDto(
    int CourtId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    decimal? TotalCost
);
