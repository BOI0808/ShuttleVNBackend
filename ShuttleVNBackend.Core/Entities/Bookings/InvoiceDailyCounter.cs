namespace ShuttleVNBackend.Core.Entities.Bookings;

public class InvoiceDailyCounter
{
    public DateOnly IssueDate { get; set; }
    public long LastNumber { get; set; }
}