using ShuttleVNBackend.Application.DTOs.Courts;
using ShuttleVNBackend.Application.Exceptions;
using ShuttleVNBackend.Application.Interfaces.Repositories;
using ShuttleVNBackend.Application.Interfaces.Repositories.Courts;
using ShuttleVNBackend.Core.Entities.Courts;

namespace ShuttleVNBackend.Application.UseCases.Courts.Services;

public class PricingRuleService(
    IPricingRuleRepository pricingRuleRepository,
    IUnitOfWork unitOfWork,
    TimeProvider clock)
{
    public async Task<IReadOnlyList<PricingRule>> GetPricingRulesAsync(int courtId, CancellationToken ct = default)
    {
        var rules = await pricingRuleRepository.GetByCourtAsync(courtId, null, ct);
        return rules == null
            ? throw new NotFoundException($"Court with id {courtId} not found.")
            : rules.OrderBy(r => r.DayOfWeek).ThenBy(r => r.StartTime).ToList();
    }

    public async Task<PricingRule> CreatePricingRuleAsync(
        int courtId, CreatePricingRuleDto request, CancellationToken ct = default)
    {
        Validate(request.DayOfWeek, request.StartTime, request.EndTime, request.PricePerHour);

        await EnsureNoOverlapAsync(courtId, request.DayOfWeek, request.StartTime, request.EndTime, null, ct);

        var now = clock.GetUtcNow().UtcDateTime;
        var rule = new PricingRule
        {
            CourtId = courtId,
            DayOfWeek = request.DayOfWeek,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            PricePerHour = request.PricePerHour,
            CreatedAt = now,
            UpdatedAt = now
        };

        await unitOfWork.AddAsync(rule, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return rule;
    }

    public async Task<PricingRule> UpdatePricingRuleAsync(
        int pricingRuleId, UpdatePricingRuleDto request, CancellationToken ct = default)
    {
        Validate(request.DayOfWeek, request.StartTime, request.EndTime, request.PricePerHour);

        var rule = await GetRuleOrThrowAsync(pricingRuleId, ct);
        await EnsureNoOverlapAsync(rule.CourtId, request.DayOfWeek, request.StartTime, request.EndTime, pricingRuleId,
            ct);

        rule.DayOfWeek = request.DayOfWeek;
        rule.StartTime = request.StartTime;
        rule.EndTime = request.EndTime;
        rule.PricePerHour = request.PricePerHour;
        rule.UpdatedAt = clock.GetUtcNow().UtcDateTime;

        await unitOfWork.SaveChangesAsync(ct);
        return rule;
    }

    public async Task DeletePricingRuleAsync(int pricingRuleId, CancellationToken ct = default)
    {
        var rule = await GetRuleOrThrowAsync(pricingRuleId, ct);

        unitOfWork.Remove(rule);
        await unitOfWork.SaveChangesAsync(ct);
    }

    // ---------------------------------------------------------------

    private static void Validate(int dayOfWeek, TimeOnly start, TimeOnly end, decimal price)
    {
        var errors = new Dictionary<string, string[]>();
        CourtValidation.ValidateDayOfWeek(dayOfWeek, errors);
        CourtValidation.ValidateTimeRange(start, end, "EndTime", errors);
        if (price <= 0)
            errors["PricePerHour"] = ["PricePerHour must be greater than 0."];
        CourtValidation.ThrowIfAny(errors);
    }

    private async Task EnsureNoOverlapAsync(
        int courtId, int dayOfWeek, TimeOnly start, TimeOnly end, int? excludeRuleId, CancellationToken ct)
    {
        var sameDay = await pricingRuleRepository.GetByCourtAsync(courtId, dayOfWeek, ct);
        // There HAS to be at least 1 rule for every day of week
        if (sameDay is null)
            throw new NotFoundException($"Court with id {courtId} not found.");

        var clash = sameDay.FirstOrDefault(r =>
            r.PricingRuleId != excludeRuleId &&
            start < r.EndTime && end > r.StartTime);

        if (clash is not null)
            throw new ConflictException(
                $"Pricing rule overlaps with existing rule {clash.PricingRuleId} ({clash.StartTime}-{clash.EndTime}).");
    }

    private async Task<PricingRule> GetRuleOrThrowAsync(int pricingRuleId, CancellationToken ct)
    {
        return await pricingRuleRepository.GetByIdAsync(pricingRuleId, ct)
               ?? throw new NotFoundException($"PricingRule with id {pricingRuleId} not found.");
    }
}