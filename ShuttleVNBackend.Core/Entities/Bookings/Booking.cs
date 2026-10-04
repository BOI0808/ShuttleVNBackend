using ShuttleVNBackend.Core.Entities.Bookings.Enums;
using ShuttleVNBackend.Core.Entities.Courts;
using ShuttleVNBackend.Core.Entities.Users;

namespace ShuttleVNBackend.Core.Entities.Bookings;

public class Booking
{
    public Guid BookingId { get; set; }
    public string BookingCode { get; set; } = null!;
    public Guid CustomerId { get; set; }
    public int CourtId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public BookingStatus Status { get; set; }
    public decimal TotalCost { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;
    public virtual BadmintonCourt Court { get; set; } = null!;
}