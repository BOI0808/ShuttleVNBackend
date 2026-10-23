using ShuttleVNBackend.Application.UseCases.Bookings.Services;

namespace ShuttleVNBackend.Api.HostedServices;

public sealed class PendingBookingExpiryService(
    IServiceScopeFactory scopeFactory,
    TimeProvider clock,
    ILogger<PendingBookingExpiryService> logger) : BackgroundService
{
    private static readonly TimeZoneInfo VietnamTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var bookingRequests = scope.ServiceProvider.GetRequiredService<BookingRequestService>();
                await bookingRequests.ExpireStartedPendingBookingsAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed to expire started pending bookings.");
            }

            var now = clock.GetUtcNow();
            var localNow = TimeZoneInfo.ConvertTime(now, VietnamTimeZone);
            var nextBoundary = new DateTimeOffset(
                localNow.Year, localNow.Month, localNow.Day, localNow.Hour,
                localNow.Minute < 30 ? 30 : 0, 0, localNow.Offset);
            if (localNow.Minute >= 30)
                nextBoundary = nextBoundary.AddHours(1);

            var runAt = nextBoundary.AddMinutes(1);
            var delay = runAt - localNow;
            if (delay > TimeSpan.Zero)
                await Task.Delay(delay, clock, stoppingToken);
        }
    }
}
