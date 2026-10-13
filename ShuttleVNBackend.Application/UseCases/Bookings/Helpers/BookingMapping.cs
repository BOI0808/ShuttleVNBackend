using System.Globalization;
using ShuttleVNBackend.Core.Entities.Bookings.Enums;
using ShuttleVNBackend.Core.Entities.System.Enums;

namespace ShuttleVNBackend.Application.UseCases.Bookings.Helpers;

public static class BookingMapping
{
    public static string ToApi(this BookingStatus value) => value.ToString().ToUpperInvariant();
    public static string ToApi(this InvoiceStatus value) => value.ToString().ToUpperInvariant();
    public static string ToApi(this PaymentMethod value) => value.ToString().ToUpperInvariant();
    public static string ToApi(this ActorType value) => value.ToString().ToUpperInvariant();

    public static string? ToApi(this BookingStatus? value) => value?.ToApi();
    public static string? ToApi(this PaymentMethod? value) => value?.ToApi();

    public static string ToApiTime(this TimeOnly value) => value.ToString("HH:mm", CultureInfo.InvariantCulture);
    public static string ToApiDate(this DateOnly value) => value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
}