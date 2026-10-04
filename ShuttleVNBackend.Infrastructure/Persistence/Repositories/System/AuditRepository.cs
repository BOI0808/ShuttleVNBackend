using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Application.Interfaces.Repositories.System;
using ShuttleVNBackend.Core.Entities.System;

namespace ShuttleVNBackend.Infrastructure.Persistence.Repositories.System;

public class AuditRepository(ShuttleVnDbContext dbContext) : IAuditRepository
{
    public async Task<PagedResult<Audit>> GetAllAsync(PageRequest page, CancellationToken ct = default)
    {
        var query = dbContext.Audits.OrderBy(x => x.CreatedAt);
        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page.PageNumber - 1) * page.PageSize)
            .Take(page.PageSize)
            .ToListAsync(ct);

        return new PagedResult<Audit>
        {
            Items = items,
            PageNumber = page.PageNumber,
            PageSize = page.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResult<Audit>> SearchAsync(
        Expression<Func<Audit, bool>> filters,
        PageRequest page,
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}