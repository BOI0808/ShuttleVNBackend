using Microsoft.EntityFrameworkCore;
using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Application.Interfaces.Repositories;
using ShuttleVNBackend.Core.Entities.User;

namespace ShuttleVNBackend.Infrastructure.Persistence.Repositories;

public class EmployeeRepository(ShuttleVnDbContext dbContext) : IEmployeeRepository
{
    public async Task<PagedResult<Employee>> GetAllEmployees(PageRequest page, CancellationToken ct = default)
    {
        var query = dbContext.Employees.OrderBy(x => x.CreatedAt);
        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page.PageNumber - 1) * page.PageSize)
            .Take(page.PageSize)
            .ToListAsync(ct);

        return new PagedResult<Employee>
        {
            Items = items,
            PageNumber = page.PageNumber,
            PageSize = page.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<Employee?> GetEmployeeById(Guid id, CancellationToken ct = default)
        => await dbContext.Employees.FirstOrDefaultAsync(e => e.EmployeeId == id, ct);

    public async Task<Employee?> GetEmployeeByAccountId(Guid accountId, CancellationToken ct = default)
        => await dbContext.Employees.FirstOrDefaultAsync(e => e.AccountId == accountId, ct);

    public async Task<Employee?> GetEmployeeByEmail(string email, CancellationToken ct = default)
        => await dbContext.Employees.FirstOrDefaultAsync(e => e.Email == email, ct);
}