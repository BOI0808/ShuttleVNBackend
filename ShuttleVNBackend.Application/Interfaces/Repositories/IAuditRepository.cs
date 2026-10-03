using System.Linq.Expressions;
using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Core.Entities.System;

namespace ShuttleVNBackend.Application.Interfaces.Repositories;

public interface IAuditRepository
{
    public Task<PagedResult<Audit>> GetAllAsync(PageRequest page, CancellationToken ct = default);
    public Task<PagedResult<Audit>> FilterAsync(
        Expression<Func<Audit, bool>> filters,
        PageRequest page,
        CancellationToken ct = default);
}