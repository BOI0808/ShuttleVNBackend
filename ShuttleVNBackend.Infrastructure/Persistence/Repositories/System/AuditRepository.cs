using Microsoft.EntityFrameworkCore;
using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Application.DTOs.System;
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

    public async Task<PagedResult<AuditDto>> SearchAsync(
        SearchAuditDto dto,
        CancellationToken ct = default)
    {
        var query = dbContext.Audits.AsNoTracking().AsQueryable();

        if (dto.ActorType is not null)
            query = query.Where(a => a.ActorType == dto.ActorType);
        if (!string.IsNullOrWhiteSpace(dto.Action))
            query = query.Where(a => a.Action == dto.Action);
        if (!string.IsNullOrWhiteSpace(dto.EntityName))
            query = query.Where(a => a.EntityName == dto.EntityName);
        if (dto.FromDate is not null)
            query = query.Where(a => a.CreatedAt >= dto.FromDate);
        if (dto.ToDate is not null)
            query = query.Where(a => a.CreatedAt < dto.ToDate); // exclusive

        // search term: EntityId, EntityName, actor id, actor name,
        var term = dto.SearchTerm?.Trim();
        if (!string.IsNullOrEmpty(term))
        {
            Guid? guid = Guid.TryParse(term, out var g) ? g : null;
            var pattern = $"%{term.Replace("\\", @"\\").Replace("%", "\\%").Replace("_", "\\_")}%";

            query = query.Where(a =>
                a.EntityId == term
                || EF.Functions.ILike(a.EntityName, pattern)
                || (guid != null && (a.EmployeeId == guid || a.CustomerId == guid))
                || (a.Employee != null && EF.Functions.ILike(a.Employee.FullName, pattern))
                || (a.Customer != null && EF.Functions.ILike(a.Customer.FullName, pattern)));
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .ThenByDescending(a => a.Id)
            .Skip((dto.PageNumber - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .Select(a => new AuditDto
            {
                Id = a.Id,
                ActorType = a.ActorType,
                ActorId = a.EmployeeId ?? a.CustomerId,
                ActorName = a.Employee != null ? a.Employee.FullName
                    : a.Customer != null ? a.Customer.FullName
                    : "System",
                Action = a.Action,
                EntityName = a.EntityName,
                EntityId = a.EntityId,
                OldValue = a.OldValue,
                NewValue = a.NewValue,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync(ct);

        return new PagedResult<AuditDto>
        {
            Items = items,
            PageNumber = dto.PageNumber,
            PageSize = dto.PageSize,
            TotalCount = total
        };
    }
}