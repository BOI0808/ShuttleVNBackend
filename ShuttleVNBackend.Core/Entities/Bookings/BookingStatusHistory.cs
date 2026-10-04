using ShuttleVNBackend.Core.Entities.Bookings.Enums;
using ShuttleVNBackend.Core.Entities.Users;

namespace ShuttleVNBackend.Core.Entities.Bookings;

public class BookingStatusHistory
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public BookingStatus? OldStatus { get; set; }
    public BookingStatus NewStatus { get; set; }
    public Guid? ChangedByEmployee { get; set; }
    public DateTime ChangedAt { get; set; }
    public string Reason { get; set; } = string.Empty;

    public virtual Booking Booking { get; set; } = null!;
    public virtual Employee? Employee { get; set; }
}