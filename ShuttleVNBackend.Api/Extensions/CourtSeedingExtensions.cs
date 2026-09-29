using Microsoft.EntityFrameworkCore;
using ShuttleVNBackend.Core.Entities.Court;
using ShuttleVNBackend.Core.Entities.Court.Enums;
using ShuttleVNBackend.Infrastructure.Persistence;

namespace ShuttleVNBackend.Api.Extensions;

public static class CourtSeedingExtensions
{
    public static async Task SeedCourtsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ShuttleVnDbContext>();

        if (await db.BadmintonCourts.AnyAsync()) return;

        var now = DateTime.UtcNow;
        for (var i = 1; i <= 4; i++)
        {
            var court = new BadmintonCourt
            {
                Name = $"Sân {i}",
                Description = $"Sân cầu lông số {i}",
                Status = CourtStatus.Active,
                CreatedAt = now,
                UpdatedAt = now
            };

            for (var day = 1; day <= 7; day++)
            {
                court.CourtSchedules.Add(new CourtSchedule
                {
                    DayOfWeek = day,
                    OpenTime = new TimeOnly(5, 0),
                    CloseTime = new TimeOnly(22, 0),
                    IsAvailable = true,
                    CreatedAt = now,
                    UpdatedAt = now
                });
                court.PricingRules.Add(new PricingRule
                {
                    DayOfWeek = day,
                    StartTime = new TimeOnly(5, 0),
                    EndTime = new TimeOnly(17, 0),
                    PricePerHour = 60_000m,
                    CreatedAt = now,
                    UpdatedAt = now
                });
                court.PricingRules.Add(new PricingRule
                {
                    DayOfWeek = day,
                    StartTime = new TimeOnly(17, 0),
                    EndTime = new TimeOnly(22, 0),
                    PricePerHour = 100_000m,
                    CreatedAt = now,
                    UpdatedAt = now
                });
            }

            db.BadmintonCourts.Add(court);
        }

        await db.SaveChangesAsync();
    }
}