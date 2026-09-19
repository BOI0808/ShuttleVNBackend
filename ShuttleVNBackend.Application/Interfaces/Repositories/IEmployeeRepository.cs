using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Core.Entities.User;

namespace ShuttleVNBackend.Application.Interfaces.Repositories;

public interface IEmployeeRepository
{
    Task<PagedResult<UserAccount>> GetAllEmployees(PageRequest page, CancellationToken ct = default);
    Task<UserAccount?> GetEmployeeAccountById(Guid employeeId, CancellationToken ct = default);
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Employee?> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default);
    Task<Employee?> GetByEmailAsync(string email, CancellationToken ct = default);
}