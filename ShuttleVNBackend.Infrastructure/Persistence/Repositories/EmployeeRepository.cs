using Microsoft.EntityFrameworkCore;
using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Application.Interfaces.Repositories;
using ShuttleVNBackend.Core.Entities.User;
using ShuttleVNBackend.Core.Entities.User.Enums;

namespace ShuttleVNBackend.Infrastructure.Persistence.Repositories;

public class EmployeeRepository(ShuttleVnDbContext dbContext) : IEmployeeRepository
{
    public async Task<PagedResult<UserAccount>> GetAllEmployeeAccounts(PageRequest page, CancellationToken ct = default)
    {
        var query = dbContext.UserAccounts
        .Include(a => a.Employee)
        .Where(a => a.AccountType == AccountType.Employee && a.Status != AccountStatus.Deleted)
        .OrderBy(a => a.CreatedAt);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page.PageNumber - 1) * page.PageSize)
            .Take(page.PageSize)
            .ToListAsync(ct);

        return new PagedResult<UserAccount>
        {
            Items = items, PageNumber = page.PageNumber, PageSize = page.PageSize, TotalCount = totalCount
        };
    }

    public async Task<UserAccount?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await dbContext.UserAccounts
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.Employee != null && a.Employee.EmployeeId == id, ct);

    public async Task<UserAccount?> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default)
        => await dbContext.UserAccounts
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.AccountId == accountId && a.Employee != null, ct);

    public async Task<Employee?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await dbContext.Employees.FirstOrDefaultAsync(e => e.Email == email, ct);

    public async Task<int> GetNextDeletedSequenceAsync(CancellationToken ct = default)
    {
        var maskedEmails = await dbContext.UserAccounts
            .Where(a => a.LoginEmail.StartsWith("DELETED_"))
            .Select(a => a.LoginEmail)
            .ToListAsync(ct);

        var maxSeq = maskedEmails
            .Select(e => e["DELETED_".Length..])
            .Where(suffix => int.TryParse(suffix, out _))
            .Select(int.Parse)
            .DefaultIfEmpty(0)
            .Max();

        return maxSeq + 1;
    }
}