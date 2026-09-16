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

    public async Task<PagedResult<Employee>> GetAllEmployees(PageRequest page)
        => await employeeRepository.GetAllEmployees(page);

    public async Task<Employee?> GetEmployeeById(Guid id)
        => await employeeRepository.GetEmployeeById(id);

    public async Task<Employee> CreateEmployee(CreateEmployeeDto dto)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(dto.FullName)) errors["FullName"] = ["Full name is required"];
        if (string.IsNullOrWhiteSpace(dto.Phone)) errors["Phone"] = ["Phone number is required"];
        if (string.IsNullOrWhiteSpace(dto.Email)) errors["Email"] = ["Email is required"];
        if (string.IsNullOrWhiteSpace(dto.Password)) errors["Password"] = ["Password is required"];
        if (errors.Count > 0) throw new ValidationException(errors: errors);

        if (await accountRepository.GetByEmailAsync(dto.Email) is not null)
            throw new ConflictException("Email is already registered");
        if (await employeeRepository.GetEmployeeByEmail(dto.Email) is not null)
            throw new ConflictException("Email is already used by another employee");

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
        return employee;
    }

    public async Task<Employee> UpdateEmployee(Guid id, EmployeeProfileDto dto)
    {
        var employee = await employeeRepository.GetEmployeeById(id)
                       ?? throw new NotFoundException("Employee not found");

        if (!string.IsNullOrWhiteSpace(dto.FullName)) employee.FullName = dto.FullName;
        if (!string.IsNullOrWhiteSpace(dto.Phone)) employee.Phone = dto.Phone;
        if (!string.IsNullOrWhiteSpace(dto.Email)) employee.Email = dto.Email;

        employee.UpdatedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync();
        return employee;
    }

    public async Task<Employee> SetAdminRole(Guid id, bool isAdmin)
    {
        var employee = await employeeRepository.GetEmployeeById(id)
                       ?? throw new NotFoundException("Employee not found");

        employee.IsAdmin = isAdmin;
        employee.UpdatedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync();
        return employee;
    }

      public async Task<Employee> SetEmployeeAccountStatus(Guid employeeId, AccountStatus status)  
    {
        var employee = await employeeRepository.GetEmployeeById(employeeId)
                      ?? throw new NotFoundException("Employee not found");

        var account = await accountRepository.GetByIdAsync(employee.AccountId)
                     ?? throw new NotFoundException("Account not found");

        account.Status = status;
        account.UpdatedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync();
        return employee;
    }
}