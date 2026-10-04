namespace ShuttleVNBackend.Application.Interfaces.Repositories;

public interface IBookingRepository
{
    Task<int> CountUpcomingByCourtAsync(int courtId, DateOnly fromDate, CancellationToken ct = default);
}