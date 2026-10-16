using ShuttleVNBackend.Core.Entities.Bookings;
using ShuttleVNBackend.Core.Entities.Bookings.Enums;

namespace ShuttleVNBackend.Application.DTOs.Bookings;

public sealed record InvoiceDto(
    Guid InvoiceId,
    string InvoiceCode,
    InvoiceStatus Status,
    decimal TotalCost,
    PaymentMethod? PaymentMethod,
    DateTime IssuedAt,
    DateTime? PaidAt,
    DateTime? CancelledAt)
{
    public static InvoiceDto FromEntity(Invoice invoice)
    {
        return new InvoiceDto(
            invoice.InvoiceId,
            invoice.InvoiceCode,
            invoice.Status,
            invoice.TotalCost,
            invoice.PaymentMethod,
            invoice.IssuedAt,
            invoice.PaidAt,
            invoice.CancelledAt);
    }
}