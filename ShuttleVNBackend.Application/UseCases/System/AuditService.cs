using System.Linq.Expressions;
using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Application.DTOs.System;
using ShuttleVNBackend.Application.Interfaces.Repositories.System;
using ShuttleVNBackend.Application.Interfaces.Repositories.Users;
using ShuttleVNBackend.Core.Entities.System;

namespace ShuttleVNBackend.Application.UseCases.System;

public class AuditService(
    IAuditRepository auditRepository,
    ICustomerRepository customerRepository,
    IEmployeeRepository employeeRepository)
{
    public async Task<PagedResult<Audit>> GetAllAsync(PageRequest page, CancellationToken ct = default)
    {
        return await auditRepository.GetAllAsync(page, ct);
    }

    public async Task<PagedResult<AuditDto>> SearchAsync(
        SearchAuditDto dto,
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}