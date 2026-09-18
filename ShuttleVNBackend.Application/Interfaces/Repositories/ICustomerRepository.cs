using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Core.Entities.User;

namespace ShuttleVNBackend.Application.Interfaces.Repositories;

public interface ICustomerRepository
{
    public Task<PagedResult<Customer>> GetAllAsync(PageRequest page, CancellationToken ct = default);
    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default);
    public Task<Customer?> GetByEmailAsync(string email, CancellationToken ct = default);
    public Task<Customer?> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default);
}