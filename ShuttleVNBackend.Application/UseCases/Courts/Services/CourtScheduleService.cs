using ShuttleVNBackend.Application.DTOs.Courts;
using ShuttleVNBackend.Application.Exceptions;
using ShuttleVNBackend.Application.Interfaces.Repositories;
using ShuttleVNBackend.Core.Entities.Courts;

namespace ShuttleVNBackend.Application.UseCases.Courts.Services;

public class CourtScheduleService(
    ICourtRepository courtRepository,
    IUnitOfWork unitOfWork,
    TimeProvider clock)
{
    public async Task<IReadOnlyList<CourtSchedule>> GetSchedulesAsync(int courtId, CancellationToken ct = default)
    {
        var schedules = await courtRepository.GetSchedulesAsync(courtId, ct);
        return schedules == null
            ? throw new NotFoundException($"Court with id {courtId} not found.")
            : schedules.OrderBy(s => s.DayOfWeek).ToList();
    }

    public async Task<CourtSchedule> UpdateScheduleAsync(
        int courtId, int dayOfWeek, UpdateCourtScheduleDto request, CancellationToken ct = default)
    {
        var errors = new Dictionary<string, string[]>();
        CourtValidation.ValidateDayOfWeek(dayOfWeek, errors);
        if (request.IsAvailable)
            CourtValidation.ValidateTimeRange(request.OpenTime, request.CloseTime, "CloseTime", errors);
        CourtValidation.ThrowIfAny(errors);

        var schedules = await courtRepository.GetSchedulesAsync(courtId, ct);
        if (schedules == null)
            throw new NotFoundException($"Court with id {courtId} not found.");

        var schedule = schedules.FirstOrDefault(s => s.DayOfWeek == dayOfWeek)
                       ?? throw new NotFoundException($"Schedule for day {dayOfWeek} of court {courtId} not found.");
        schedule.OpenTime = request.OpenTime;
        schedule.CloseTime = request.CloseTime;
        schedule.IsAvailable = request.IsAvailable;
        schedule.UpdatedAt = clock.GetUtcNow().UtcDateTime;

        await unitOfWork.SaveChangesAsync(ct);
        return schedule;
    }
}