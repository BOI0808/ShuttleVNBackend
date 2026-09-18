using Microsoft.Extensions.Options;
using ShuttleVNBackend.Api.Options;
using ShuttleVNBackend.Application.DTOs.User;
using ShuttleVNBackend.Application.Exceptions;
using ShuttleVNBackend.Application.UseCases.User.Services;

namespace ShuttleVNBackend.Api.Extensions;

public static class AdminSeedingExtensions
{
    public static async Task SeedAdminAccountAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var employeeService = scope.ServiceProvider.GetRequiredService<EmployeeService>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AdminAccountOptions>>().Value;
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        var dto = new CreateEmployeeDto
        {
            FullName = options.FullName,
            Phone = options.Phone,
            Email = options.Email,
            Password = options.Password,
            IsAdmin = true
        };

        try
        {
            await employeeService.CreateEmployee(dto);
            logger.LogInformation("Admin account created for {Email}.", options.Email);
        }
        catch (ConflictException)
        {
            logger.LogWarning("Admin account seed skipped (already exists).");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating admin account.");
        }
    }
}