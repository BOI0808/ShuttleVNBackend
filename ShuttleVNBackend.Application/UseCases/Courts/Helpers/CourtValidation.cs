using ShuttleVNBackend.Application.Exceptions;

namespace ShuttleVNBackend.Application.UseCases.Courts.Helpers;

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