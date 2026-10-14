using ShuttleVNBackend.Core.Entities.Bookings.Enums;

namespace ShuttleVNBackend.Application.DTOs.Bookings;

public sealed record BookingDto(
    Guid BookingId,
    string BookingCode,
    Guid CustomerId,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    int CourtId,
    string CourtName,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    BookingStatus Status,
    decimal TotalCost,
    DateTime CreatedAt,
    IReadOnlyList<BookingPriceSegmentDto> PriceSegments,
    string? InvoiceCode,
    InvoiceStatus? InvoiceStatus
);

public sealed record BookingPriceSegmentDto(
    int SegmentNo,
    TimeOnly PriceStartTime,
    TimeOnly SegmentStartTime,
    TimeOnly SegmentEndTime,
    decimal PricePerHour
);
