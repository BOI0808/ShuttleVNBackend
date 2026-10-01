using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShuttleVNBackend.Application.Interfaces.Repositories;
using ShuttleVNBackend.Application.Interfaces.User;
using ShuttleVNBackend.Application.UseCases.Authentication.Services;
using ShuttleVNBackend.Application.UseCases.User.Services;
using ShuttleVNBackend.Infrastructure.Persistence.Interceptors;
using ShuttleVNBackend.Infrastructure.Persistence.Repositories;
using ShuttleVNBackend.Infrastructure.User;

namespace ShuttleVNBackend.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ShuttleVnDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ShuttleVnDbContext>());

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddScoped<AppAuthService>();
        services.AddScoped<AccountService>();
        services.AddScoped<CustomerService>();
        services.AddScoped<EmployeeService>();
        services.AddScoped<ProfileService>();
        
        services.AddScoped<AuditInterceptor>();

        return services;
    }
}