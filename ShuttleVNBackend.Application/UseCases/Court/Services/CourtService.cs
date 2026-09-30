using System.Text.Json;
using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Application.Exceptions;
using ShuttleVNBackend.Application.DTOs.Court;
using ShuttleVNBackend.Application.Interfaces.Repositories;
using ShuttleVNBackend.Core.Entities.Booking;
using ShuttleVNBackend.Core.Entities.Court;
using ShuttleVNBackend.Core.Entities.System;
using ShuttleVNBackend.Core.Entities.Court.Enums;

namespace ShuttleVNBackend.Application.UseCases.Court.Services;

public class CourtService(ICourtRepository courtRepository, IUnitOfWork unitOfWork)
{
    private const int SlotMinutes = 30;
    private const int SlotCount = 34;
    private static readonly TimeOnly GridStart = new(5, 0);

    private static DateTime VietnamNow => DateTime.UtcNow.AddHours(7);

    public async Task<PagedResult<CourtDto>> GetCourtsAsync(
        PageRequest page, CourtStatus? status, string? search, CancellationToken ct = default)
    {
        var result = await courtRepository.GetAllAsync(page, status, search, ct);
        var inUse = await GetInUseIdsAsync(ct);

        return new PagedResult<CourtDto>
        {
            Items = result.Items.Select(c => ToDto(c, inUse)).ToList(),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }

    public async Task<CourtDto?> GetCourtByIdAsync(int courtId, CancellationToken ct = default)
    {
        var court = await courtRepository.GetByIdAsync(courtId, ct);
        return court is null ? null : ToDto(court, await GetInUseIdsAsync(ct));
    }

    public async Task<CourtGridResponseDto> GetCourtGridAsync(DateOnly? date, CancellationToken ct = default)
    {
        var now = VietnamNow;
        var day = date ?? DateOnly.FromDateTime(now);
        var isoDay = day.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)day.DayOfWeek;

        var sources = await courtRepository.GetAllForGridAsync(day, isoDay, ct);
        var inUse = day == DateOnly.FromDateTime(now) ? await GetInUseIdsAsync(ct) : [];

        var items = sources.Select(s => new CourtGridItemDto(
            ToDto(s.Court, inUse),
            Enumerable.Range(0, SlotCount)
                .Select(i => BuildSlot(s, day, GridStart.AddMinutes(i * SlotMinutes)))
                .ToList())).ToList();

        return new CourtGridResponseDto(day, items);
    }

    private async Task<HashSet<int>> GetInUseIdsAsync(CancellationToken ct)
    {
        var now = VietnamNow;
        return await courtRepository.GetCourtIdsInUseAsync(
            DateOnly.FromDateTime(now), TimeOnly.FromDateTime(now), ct);
    }

    private static CourtDto ToDto(BadmintonCourt c, HashSet<int> inUse)
        => CourtDto.FromEntity(c, c.Status == CourtStatus.Active && inUse.Contains(c.CourtId));

    private static CourtSlotDto BuildSlot(CourtGridSource s, DateOnly date, TimeOnly start)
    {
        var end = start.AddMinutes(SlotMinutes);
        var court = s.Court;

        var isOpen = court.Status == CourtStatus.Active
                     && court.CourtSchedules.Any(x => x.IsAvailable && x.OpenTime <= start && end <= x.CloseTime); 

        var booking = s.Bookings.FirstOrDefault(b => b.StartTime < end && b.EndTime > start); 

        var status = !isOpen ? "CLOSED" : booking is not null ? "BOOKED" : "AVAILABLE";
        return new CourtSlotDto(
            court.CourtId, date, start.ToString("HH:mm"), end.ToString("HH:mm"), status,
            status == "BOOKED" ? booking!.BookingId : null,
            ResolvePricePerHour(court.PricingRules, start, end));
    }

    private static decimal ResolvePricePerHour(IEnumerable<PricingRule> rules, TimeOnly start, TimeOnly end)
    {
        decimal total = 0;
        var covered = 0;
        foreach (var r in rules)
        {
            var from = start > r.StartTime ? start : r.StartTime;
            var to = end < r.EndTime ? end : r.EndTime;
            var minutes = (int)(to - from).TotalMinutes;
            if (minutes <= 0) continue;
            total += r.PricePerHour * minutes;
            covered += minutes;
        }
        return covered == 0 ? 0 : Math.Round(total / covered, 2);
    }

    public async Task<UpdateCourtStatusResultDto> UpdateCourtStatusAsync(
    int courtId, UpdateCourtStatusDto dto, Guid? actorAccountId, CancellationToken ct = default)
    {
        if (!Enum.IsDefined(dto.Status))
            throw new ValidationException(errors: new Dictionary<string, string[]> { ["Status"] = ["Invalid court status"] });

        var court = await courtRepository.GetByIdAsync(courtId, ct)
                    ?? throw new NotFoundException("Court not found");

        var oldStatus = court.Status;
        if (oldStatus != dto.Status)
        {
            court.Status = dto.Status;
            court.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.AddAsync(new Audit
            {
                AccountId = actorAccountId,
                Action = "UPDATE_STATUS",
                EntityName = "Court",
                EntityId = courtId.ToString(),
                OldValue = JsonSerializer.Serialize(new { Status = oldStatus.ToString().ToUpperInvariant() }),
                NewValue = JsonSerializer.Serialize(new { Status = dto.Status.ToString().ToUpperInvariant(), dto.Reason }),
                CreatedAt = DateTime.UtcNow
            });
            await unitOfWork.SaveChangesAsync();
        }

        var affected = dto.Status == CourtStatus.Active
            ? 0
            : await courtRepository.CountUpcomingBookingsAsync(courtId, DateOnly.FromDateTime(VietnamNow), ct);

        return new UpdateCourtStatusResultDto(ToDto(court, await GetInUseIdsAsync(ct)), affected);
    }
}