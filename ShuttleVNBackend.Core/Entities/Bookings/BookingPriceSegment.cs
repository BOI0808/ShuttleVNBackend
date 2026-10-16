using ShuttleVNBackend.Core.Attributes;

namespace ShuttleVNBackend.Core.Entities.Bookings;

[NoAudit]
public class BookingPriceSegment
{
    public Guid BookingId { get; set; }
    public int SegmentNo { get; set; }
    public TimeOnly PriceStartTime { get; set; }
    public TimeOnly SegmentStartTime { get; set; }
    public TimeOnly SegmentEndTime { get; set; }
    public decimal PricePerHour { get; set; }

    private Booking? _booking;
    public virtual Booking Booking
    {
        get => _booking ?? throw new InvalidOperationException("Uninitialized property: " + nameof(Booking));
        set => _booking = value;
    }
}
