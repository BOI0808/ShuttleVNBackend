namespace ShuttleVNBackend.Core.Entities.Bookings;

public class BookingPriceSegment
{
    public Guid BookingId { get; set; }
    public int SegmentNo { get; set; }
    public TimeOnly PriceStartTime { get; set; }
    public TimeOnly SegmentStartTime { get; set; }
    public TimeOnly SegmentEndTime { get; set; }
    public decimal PricePerHour { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
