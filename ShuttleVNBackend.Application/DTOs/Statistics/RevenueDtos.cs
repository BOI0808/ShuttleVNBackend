namespace ShuttleVNBackend.Application.DTOs.Statistics;

public record RevenuePeriodDto(DateOnly PeriodStart, int InvoiceCount, decimal Revenue);

public record RevenueSummaryDto(
    DateOnly FromDate,
    DateOnly ToDate,
    RevenueGroupBy GroupBy,
    int TotalInvoices,
    decimal TotalRevenue,
    IReadOnlyList<RevenuePeriodDto> Periods);