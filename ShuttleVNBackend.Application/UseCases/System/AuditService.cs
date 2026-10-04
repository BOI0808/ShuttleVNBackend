using System.Linq.Expressions;
using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Application.Interfaces.Repositories;
using ShuttleVNBackend.Core.Entities.System;

namespace ShuttleVNBackend.Application.UseCases.System;

public class AuditService(IAuditRepository auditRepository)
{
    public async Task<PagedResult<Audit>> GetAllAsync(PageRequest page, CancellationToken ct = default)
    {
        return await auditRepository.GetAllAsync(page, ct);
    }

    public async Task<PagedResult<Audit>> FilterAsync(
        PageRequest page,
        Expression<Func<Audit, bool>> filters,
        CancellationToken ct = default)
    {
        return await auditRepository.FilterAsync(filters, page, ct);
    }
}