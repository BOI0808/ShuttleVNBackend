using ShuttleVNBackend.Core.Entities.Bookings.Enums;
using ShuttleVNBackend.Core.Entities.Users;

namespace ShuttleVNBackend.Core.Entities.Bookings;

public class Invoice
{
    public Guid InvoiceId { get; set; }
    public string InvoiceCode { get; set; } = null!;
    public Guid BookingId { get; set; }
    public decimal TotalCost { get; set; }
    public InvoiceStatus Status { get; set; }
    public Guid IssuedByEmployee { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public string Note { get; set; } = string.Empty;

    public virtual Booking Booking { get; set; } = null!;
    public virtual Employee Employee { get; set; } = null!;
}