using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Application.DTOs.Court;
using ShuttleVNBackend.Application.Exceptions;
using ShuttleVNBackend.Application.Interfaces.Repositories;
using ShuttleVNBackend.Core.Entities.Booking;
using ShuttleVNBackend.Core.Entities.Court;
using ShuttleVNBackend.Core.Entities.Court.Enums;

namespace ShuttleVNBackend.Application.UseCases.Court.Services;

internal static class CourtValidation
{
    public static void ThrowIfAny(Dictionary<string, string[]> errors)
    {
        if (errors.Count > 0)
            throw new ValidationException(errors: errors);
    }
 
    public static void ValidateDayOfWeek(int dayOfWeek, Dictionary<string, string[]> errors)
    {
        if (dayOfWeek is < 1 or > 7)
            errors["DayOfWeek"] = ["DayOfWeek must be between 1 and 7."];
    }
 
    public static void ValidateTimeRange(
        TimeOnly start, TimeOnly end, string endField, Dictionary<string, string[]> errors)
    {
        if (start >= end)
            errors[endField] = [$"{endField} must be after the start time."];
    }
}

public class CourtService(
    ICourtRepository courtRepository,
    IBookingRepository bookingRepository,
    TimeProvider clock,
    IUnitOfWork unitOfWork)
{
    private const int SlotMinutes = 30;
    private const int SlotCount = 34;
    private static readonly TimeOnly GridStart = new(5, 0);

    public async Task<PagedResult<CourtDto>> GetCourtsAsync(CancellationToken ct = default)
    {
        var courts = await courtRepository.GetAllAsync(ct);
        var inUse = await GetInUseIdsAsync(ct);
        var dtos = courts.Select(c => ToDto(c, inUse)).ToList();

        return new PagedResult<CourtDto>
        {
            Items = dtos,
            PageNumber = 1,
            PageSize = dtos.Count,
            TotalCount = dtos.Count
        };
    }

    public async Task<CourtDto?> GetCourtByIdAsync(int courtId, CancellationToken ct = default)
    {
        var court = await courtRepository.GetByIdAsync(courtId, ct);
        return court is null ? null : ToDto(court, await GetInUseIdsAsync(ct));
    }

    public async Task<CourtGridResponseDto> GetCourtGridAsync(DateOnly? date, CancellationToken ct = default)
    {
        var now = clock.GetUtcNow().UtcDateTime;
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
            court.UpdatedAt = clock.GetUtcNow().UtcDateTime;

            await unitOfWork.SaveChangesAsync(ct);
        }

        var affected = dto.Status == CourtStatus.Active
            ? 0
            : await bookingRepository.CountUpcomingByCourtAsync(courtId, DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime), ct);

        return new UpdateCourtStatusResultDto(ToDto(court, await GetInUseIdsAsync(ct)), affected);
    }

    private async Task<HashSet<int>> GetInUseIdsAsync(CancellationToken ct)
    {
        var now = clock.GetUtcNow().UtcDateTime;
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

    public async Task<IReadOnlyList<BadmintonCourt>> GetAllCourtsAsync(CancellationToken ct = default)
        => await courtRepository.GetAllAsync(ct);

    public async Task<BadmintonCourt> CreateCourtAsync(CreateCourtDto request, CancellationToken ct = default)
    {
        var errors = new Dictionary<string, string[]>();

        var name = request.Name.Trim();
        if (name.Length == 0)
            errors["Name"] = ["Court name is required."];
        CourtValidation.ValidateTimeRange(
            request.DefaultOpenTime,
            request.DefaultCloseTime,
            "DefaultCloseTime",
            errors);
        if (request.DefaultPricePerHour <= 0)
            errors["DefaultPricePerHour"] = ["DefaultPricePerHour must be greater than 0."];
        CourtValidation.ThrowIfAny(errors);

        if (await courtRepository.NameExistsAsync(name, excludeCourtId: null, ct))
            throw new ConflictException($"Court with name '{name}' already exists.");

        var now = clock.GetUtcNow().UtcDateTime;
        var court = new BadmintonCourt
        {
            Name = name,
            Description = request.Description?.Trim() ?? string.Empty,
            Status = CourtStatus.Active,
            CreatedAt = now,
            UpdatedAt = now
        };

        for (var day = 1; day < 8; day++)
        {
            court.CourtSchedules.Add(new CourtSchedule
            {
                DayOfWeek = day,
                OpenTime = request.DefaultOpenTime,
                CloseTime = request.DefaultCloseTime,
                IsAvailable = true,
                CreatedAt = now,
                UpdatedAt = now
            });

            court.PricingRules.Add(new PricingRule
            {
                DayOfWeek = day,
                StartTime = request.DefaultOpenTime,
                EndTime = request.DefaultCloseTime,
                PricePerHour = request.DefaultPricePerHour,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        await unitOfWork.AddAsync(court, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return court;
    }

    public async Task<BadmintonCourt> UpdateCourtAsync(
        int courtId,
        UpdateCourtDto request,
        CancellationToken ct = default)
    {
        var name = request.Name.Trim();
        if (name.Length == 0)
            throw new ValidationException(errors: new Dictionary<string, string[]>
            {
                ["Name"] = ["Court name is required."]
            });

        var court = await GetCourtOrThrowAsync(courtId, ct);

        if (await courtRepository.NameExistsAsync(name, excludeCourtId: courtId, ct))
            throw new ConflictException($"Court with name '{name}' already exists.");

        court.Name = name;
        court.Description = request.Description?.Trim() ?? string.Empty;
        court.UpdatedAt = clock.GetUtcNow().UtcDateTime;

        await unitOfWork.SaveChangesAsync(ct);
        return court;
    }

    public async Task SetCourtStatusAsync(
        int courtId,
        CourtStatus status,
        CancellationToken ct = default)
    {
        var court = await GetCourtOrThrowAsync(courtId, ct);

        court.Status = status;
        court.UpdatedAt = clock.GetUtcNow().UtcDateTime;

        await unitOfWork.SaveChangesAsync(ct);
    }

    private async Task<BadmintonCourt> GetCourtOrThrowAsync(int courtId, CancellationToken ct)
        => await courtRepository.GetByIdAsync(courtId, ct)
           ?? throw new NotFoundException($"Court with id {courtId} not found.");
}