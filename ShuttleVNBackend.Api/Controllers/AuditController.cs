using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShuttleVNBackend.Api.Common;
using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Application.DTOs.System;
using ShuttleVNBackend.Application.UseCases.System;

namespace ShuttleVNBackend.Api.Controllers;

[ApiController]
[Route("api/audits")]
[Authorize(Policy = "AdminOnly")]
public class AuditController(AuditService auditService): ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAudits([FromQuery] SearchAuditDto dto)
    {
        var pagedResult = await auditService.SearchAsync(dto);
        return Ok(ApiResponseFactory.Success(pagedResult));
    }
}