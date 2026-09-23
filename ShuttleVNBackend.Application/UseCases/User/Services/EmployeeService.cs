using Microsoft.AspNetCore.Identity;
using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Application.DTOs.User;
using ShuttleVNBackend.Application.Exceptions;
using ShuttleVNBackend.Application.Interfaces.Repositories;
using ShuttleVNBackend.Core.Entities.User;
using ShuttleVNBackend.Core.Entities.User.Enums;

namespace ShuttleVNBackend.Application.UseCases.User.Services;

public class EmployeeService(
    IEmployeeRepository employeeRepository,
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork)
{
    private readonly PasswordHasher<UserAccount> _hasher = new();

    public async Task<PagedResult<UserAccount>> GetAllEmployeeAccounts(PageRequest page)
        => await employeeRepository.GetAllEmployeeAccounts(page);

    public async Task<UserAccount?> GetEmployeeAccount(Guid id)
        => await employeeRepository.GetByIdAsync(id);

    public async Task<UserAccount> CreateEmployee(CreateEmployeeDto dto)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(dto.FullName)) errors["FullName"] = ["Full name is required"];
        if (string.IsNullOrWhiteSpace(dto.Phone)) errors["Phone"] = ["Phone number is required"];
        if (string.IsNullOrWhiteSpace(dto.Email)) errors["Email"] = ["Email is required"];
        if (string.IsNullOrWhiteSpace(dto.Password)) errors["Password"] = ["Password is required"];
        if (errors.Count > 0) throw new ValidationException(errors: errors);

        if (await accountRepository.GetByEmailAsync(dto.Email) is not null)
            throw new ConflictException("Email is already registered");

        var now = DateTime.UtcNow;
        var account = new UserAccount
        {
            AccountId = Guid.NewGuid(),
            LoginEmail = dto.Email,
            AccountType = AccountType.Employee,
            Status = AccountStatus.Active,
            CreatedAt = now,
            UpdatedAt = now
        };
        account.PasswordHash = _hasher.HashPassword(account, dto.Password);
        await unitOfWork.AddAsync(account);

        var employee = new Employee
        {
            EmployeeId = Guid.NewGuid(),
            AccountId = account.AccountId,
            FullName = dto.FullName,
            Phone = dto.Phone,
            Email = dto.Email,
            IsAdmin = dto.IsAdmin,
            CreatedAt = now,
            UpdatedAt = now
        };
        await unitOfWork.AddAsync(employee);
        await unitOfWork.SaveChangesAsync();
        account.Employee = employee;
        return account;
    }

    public async Task<UserAccount> UpdateEmployee(Guid id, CustomerProfileDto dto)
    {
        var account = await employeeRepository.GetByIdAsync(id) ?? throw new NotFoundException("Employee not found");
        var employee = account.Employee ?? throw new NotFoundException("Employee not found");

        if (!string.IsNullOrWhiteSpace(dto.FullName)) employee.FullName = dto.FullName;
        if (!string.IsNullOrWhiteSpace(dto.Phone)) employee.Phone = dto.Phone;

        employee.UpdatedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync();
        return account;
    }

    public async Task<UserAccount> GrantAdminRole(Guid id)   
    {
        var account = await employeeRepository.GetByIdAsync(id) ?? throw new NotFoundException("Employee not found");
        var employee = account.Employee ?? throw new NotFoundException("Employee not found");

        employee.IsAdmin = true;
        employee.UpdatedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync();
        return account;
    }

    public async Task<UserAccount> SetEmployeeAccountStatus(Guid employeeId, AccountStatus status)
    {
        var account = await employeeRepository.GetByIdAsync(employeeId) ?? throw new NotFoundException("Employee not found");
        account.Status = status;
        account.UpdatedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync();
        return account;
    }

    public async Task DeleteEmployee(Guid id)
    {
        var account = await employeeRepository.GetByIdAsync(id) ?? throw new NotFoundException("Employee not found");
        var employee = account.Employee ?? throw new NotFoundException("Employee not found");

        employee.AccountId = null;
        employee.Email = null;
        employee.UpdatedAt = DateTime.UtcNow;

        unitOfWork.Remove(account);
        await unitOfWork.SaveChangesAsync();
    }
}