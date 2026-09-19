using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Core.Entities.User;

namespace ShuttleVNBackend.Application.Interfaces.Repositories;

public interface IEmployeeRepository
{
    Task<PagedResult<UserAccount>> GetAllEmployees(PageRequest page, CancellationToken ct = default);
    Task<UserAccount?> GetEmployeeAccountById(Guid employeeId, CancellationToken ct = default);
    Task<Employee?> GetEmployeeById(Guid id, CancellationToken ct = default);
    Task<Employee?> GetEmployeeByAccountId(Guid accountId, CancellationToken ct = default);
    Task<Employee?> GetEmployeeByEmail(string email, CancellationToken ct = default);
}