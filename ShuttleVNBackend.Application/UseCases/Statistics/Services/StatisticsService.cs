using ShuttleVNBackend.Application.DTOs.Statistics;
using ShuttleVNBackend.Application.Exceptions;
using ShuttleVNBackend.Application.Interfaces.Repositories;
using ShuttleVNBackend.Application.Interfaces.Repositories.System;
using ShuttleVNBackend.Core.Entities.Bookings.Enums;

namespace ShuttleVNBackend.Application.UseCases.Statistics.Services;

public class StatisticsService(
    IStatisticsRepository statisticsRepository,
    TimeProvider clock)
{
    private const int MaxRangeDays = 366;
    private const int FirstHour = 5;
    private const int HourBuckets = 17;

    public async Task<CourtUsageSummaryDto> GetCourtUsageAsync(
        DateOnly? fromDate, DateOnly? toDate, CancellationToken ct = default)
    {
        var now = clock.GetUtcNow().UtcDateTime;
        var today = DateOnly.FromDateTime(now);
        var from = fromDate ?? new DateOnly(today.Year, today.Month, 1);
        var to = toDate ?? today;

        if (from > to || to.DayNumber - from.DayNumber >= MaxRangeDays)
            throw new ValidationException(errors: new Dictionary<string, string[]>
            {
                ["DateRange"] = [$"fromDate must be <= toDate and the range at most {MaxRangeDays} days"]
            });

        var courts = await statisticsRepository.GetCourtsWithSchedulesAsync(ct);
        var bookings = await statisticsRepository.GetBookingsInRangeAsync(from, to, ct);

        var dayCounts = new int[8]; // index = ISO day 1..7
        for (var d = from; d <= to; d = d.AddDays(1))
            dayCounts[d.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)d.DayOfWeek]++;

        var stats = courts.Select(c =>
        {
            var own = bookings.Where(b => b.CourtId == c.CourtId).ToList();
            var used = own.Where(b => b.Status is BookingStatus.Confirmed or BookingStatus.Completed).ToList();

            var availableHours = c.CourtSchedules
                .Where(s => s.IsAvailable)
                .Sum(s => (s.CloseTime - s.OpenTime).TotalHours * dayCounts[s.DayOfWeek]);
            var bookedHours = used.Sum(b => (b.EndTime - b.StartTime).TotalHours);

            return new CourtUsageStatDto(
                c.CourtId, c.Name, c.Status.ToString(),
                Math.Round(availableHours, 2), (int)Math.Round(availableHours * 2),
                Math.Round(bookedHours, 2), (int)Math.Round(bookedHours * 2),
                used.Count,
                used.Count(b => b.Status == BookingStatus.Confirmed),
                used.Count(b => b.Status == BookingStatus.Completed),
                own.Count(b => b.Status == BookingStatus.Cancelled),
                availableHours > 0 ? Math.Round(bookedHours / availableHours * 100, 2) : 0,
                used.Sum(b => b.TotalCost));
        }).ToList();

        var usedAll = bookings.Where(b => b.Status is BookingStatus.Confirmed or BookingStatus.Completed).ToList();

        var hourly = Enumerable.Range(0, HourBuckets).Select(i =>
        {
            var bucketStart = new TimeOnly(FirstHour + i, 0);
            var bucketEnd = bucketStart.AddHours(1);
            double hours = 0;
            var count = 0;
            foreach (var b in usedAll)
            {
                var overlapStart = b.StartTime > bucketStart ? b.StartTime : bucketStart;
                var overlapEnd = b.EndTime < bucketEnd ? b.EndTime : bucketEnd;
                var minutes = (overlapEnd - overlapStart).TotalMinutes;
                if (minutes <= 0) continue;
                hours += minutes / 60;
                count++;
            }

            return new HourlyUsageStatDto(bucketStart.ToString("HH:mm"), Math.Round(hours, 2), count);
        }).ToList();

        var totalBooked = stats.Sum(s => s.BookedHours);
        var totalAvailable = stats.Sum(s => s.TotalAvailableHours);
        var mostUsed = stats.Where(s => s.BookedHours > 0).MaxBy(s => s.BookedHours)?.CourtName;

        return new CourtUsageSummaryDto(
            from, to, usedAll.Count,
            Math.Round(totalBooked, 2), Math.Round(totalAvailable, 2),
            totalAvailable > 0 ? Math.Round(totalBooked / totalAvailable * 100, 2) : 0,
            mostUsed, stats, hourly);
    }
}