using ShuttleVNBackend.Application.DTOs.User;
using ShuttleVNBackend.Application.Exceptions;
using ShuttleVNBackend.Application.Interfaces.Repositories;
using ShuttleVNBackend.Core.Entities.User.Enums;

namespace ShuttleVNBackend.Application.UseCases.User.Services;

public class ProfileService(
    ICustomerRepository customerRepository,
    IEmployeeRepository employeeRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<MyProfileDto> GetMyProfile(Guid accountId, AccountType accountType)
    {
        if (accountType == AccountType.Customer)
        {
            var customer = await customerRepository.GetCustomerByAccountId(accountId)
                          ?? throw new NotFoundException("Profile not found");
            return Map(accountId, customer.FullName, customer.Phone, customer.Email, "Customer");
        }

        var employee = await employeeRepository.GetEmployeeByAccountId(accountId)
                      ?? throw new NotFoundException("Profile not found");
        return Map(accountId, employee.FullName, employee.Phone, employee.Email, employee.IsAdmin ? "Admin" : "Employee");
    }

    public async Task<MyProfileDto> UpdateMyProfile(Guid accountId, AccountType accountType, UpdateMyProfileDto dto)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(dto.FullName)) errors["FullName"] = ["Full name is required"];
        if (string.IsNullOrWhiteSpace(dto.Phone)) errors["Phone"] = ["Phone number is required"];
        if (errors.Count > 0) throw new ValidationException(errors: errors);

        if (accountType == AccountType.Customer)
        {
            var customer = await customerRepository.GetCustomerByAccountId(accountId)
                          ?? throw new NotFoundException("Profile not found");
            customer.FullName = dto.FullName;
            customer.Phone = dto.Phone;
            customer.UpdatedAt = DateTime.UtcNow;
            await unitOfWork.SaveChangesAsync();
            return Map(accountId, customer.FullName, customer.Phone, customer.Email, "Customer");
        }

        var employee = await employeeRepository.GetEmployeeByAccountId(accountId)
                      ?? throw new NotFoundException("Profile not found");
        employee.FullName = dto.FullName;
        employee.Phone = dto.Phone;
        employee.UpdatedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync();
        return Map(accountId, employee.FullName, employee.Phone, employee.Email, employee.IsAdmin ? "Admin" : "Employee");
    }

    private static MyProfileDto Map(Guid accountId, string fullName, string phone, string email, string role)
        => new() { AccountId = accountId, FullName = fullName, Phone = phone, Email = email, Role = role };
}