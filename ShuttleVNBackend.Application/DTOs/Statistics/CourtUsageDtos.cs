namespace ShuttleVNBackend.Application.DTOs.Statistics;

public record CourtUsageStatDto(
    int CourtId, string CourtName, string Status,
    double TotalAvailableHours, int TotalSlots,
    double BookedHours, int BookedSlots,
    int BookingCount, int ConfirmedCount, int CompletedCount, int CancelledCount,
    double OccupancyRate, decimal TotalRevenue);

public record HourlyUsageStatDto(string Hour, double BookedHours, int BookingCount);

public record CourtUsageSummaryDto(
    DateOnly FromDate, DateOnly ToDate,
    int TotalBookings, double TotalBookedHours, double TotalAvailableHours,
    double AverageOccupancyRate, string? MostUsedCourtName,
    IReadOnlyList<CourtUsageStatDto> Courts,
    IReadOnlyList<HourlyUsageStatDto> HourlyBreakdown);