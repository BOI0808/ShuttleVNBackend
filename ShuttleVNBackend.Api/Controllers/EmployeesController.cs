using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShuttleVNBackend.Api.Common;
using ShuttleVNBackend.Api.DTOs.User;
using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Application.DTOs.User;
using ShuttleVNBackend.Application.UseCases.User.Services;
using ShuttleVNBackend.Core.Entities.User.Enums;

namespace ShuttleVNBackend.Api.Controllers;

[ApiController]
[Route("api/employees")]
[Authorize(Policy = "StaffOnly")]
public class EmployeesController(EmployeeService employeeService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ListEmployees([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var result = await employeeService.GetAllEmployeeAccounts(new PageRequest(pageNumber, pageSize));
        var mapped = new PagedResult<AccountDto>
        {
            Items = result.Items.Select(AccountDto.FromEntity).ToList(),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
        return Ok(ApiResponseFactory.Success(mapped));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetEmployeeAccount([FromRoute] Guid id)
    {
        var account = await employeeService.GetEmployeeAccount(id);
        if (account is null) return NotFound(new ProblemDetails { Title = "Resource not found" });
        return Ok(ApiResponseFactory.Success(AccountDto.FromEntity(account)));
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto dto)
    {
        var created = await employeeService.CreateEmployee(dto);
        return CreatedAtAction(
            nameof(GetEmployeeAccount),
            new { id = created.Employee!.EmployeeId },
            ApiResponseFactory.Success(AccountDto.FromEntity(created)));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateEmployee([FromRoute] Guid id, [FromBody] CustomerProfileDto dto)
    {
        return Ok(ApiResponseFactory.Success(AccountDto.FromEntity(await employeeService.UpdateEmployee(id, dto))));
    }

    [HttpPost("{id:guid}/lock")]                    
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> LockEmployee([FromRoute] Guid id)
    {
        return Ok(ApiResponseFactory.Success(
            AccountDto.FromEntity(await employeeService.SetEmployeeAccountStatus(id, AccountStatus.Disabled))));
    }

    [HttpPost("{id:guid}/unlock")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UnlockEmployee([FromRoute] Guid id)
    {
        return Ok(ApiResponseFactory.Success(
            AccountDto.FromEntity(await employeeService.SetEmployeeAccountStatus(id, AccountStatus.Active))));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteEmployee([FromRoute] Guid id)
    {
        await employeeService.DeleteEmployee(id);
        return NoContent();
    }
}