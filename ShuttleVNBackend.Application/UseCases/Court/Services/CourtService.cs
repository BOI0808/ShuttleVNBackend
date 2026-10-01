using ShuttleVNBackend.Application.DTOs.Court;
using ShuttleVNBackend.Application.Exceptions;
using ShuttleVNBackend.Application.Interfaces.Repositories;
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
        if (dayOfWeek is < 0 or > 6)
            errors["DayOfWeek"] = ["DayOfWeek must be between 0 and 6."];
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
    TimeProvider clock,
    IUnitOfWork unitOfWork)
{
    public async Task<BadmintonCourt?> GetCourtByIdAsync(int courtId, CancellationToken ct = default)
        => await courtRepository.GetByIdAsync(courtId, ct);

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

        var now = DateTime.UtcNow;
        var court = new BadmintonCourt
        {
            Name = name,
            Description = request.Description?.Trim() ?? string.Empty,
            Status = CourtStatus.Active,
            CreatedAt = now,
            UpdatedAt = now
        };

        // generate default schedules for 7 days (0-6)
        for (var day = 0; day < 7; day++)
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
        court.UpdatedAt = DateTime.UtcNow;
        
        await unitOfWork.SaveChangesAsync(ct);
    }
    
    
    //-----------------------------------------------------------------------------
    
    private async Task<BadmintonCourt> GetCourtOrThrowAsync(int courtId, CancellationToken ct)
        => await courtRepository.GetByIdAsync(courtId, ct)
           ?? throw new NotFoundException($"Court with id {courtId} not found.");
}