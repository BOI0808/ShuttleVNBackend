using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShuttleVNBackend.Api.Common;
using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Application.DTOs.Users;
using ShuttleVNBackend.Application.UseCases.Users.Services;
using ShuttleVNBackend.Core.Entities.Users.Enums;
using ShuttleVNBackend.Application.DTOs.Courts;
using ShuttleVNBackend.Core.Entities.Courts;

namespace ShuttleVNBackend.Api.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountController(
    AccountService accountService,
    CustomerService customerService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<ApiResponse<PagedResult<AccountDto>>>> ListAccounts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var page = new PageRequest(pageNumber, pageSize);
        var pagedResult = await accountService.GetAllAccounts(page);
        return Ok(ApiResponseFactory.Success(pagedResult));
    }

    [HttpPost("{accountId:guid}/lock")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<ApiResponse<dynamic>>> LockAccount([FromRoute] Guid accountId)
    {
        await accountService.UpdateAccountStatus(accountId, AccountStatus.Disabled);
        return Ok(ApiResponseFactory.Success(new
        {
            message = "Account locked"
        }));
    }

    [HttpPost("{accountId:guid}/unlock")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<ApiResponse<dynamic>>> UnlockAccount([FromRoute] Guid accountId)
    {
        await accountService.UpdateAccountStatus(accountId, AccountStatus.Active);
        return Ok(ApiResponseFactory.Success(new
        {
            message = "Account unlocked"
        }));
    }

    [HttpGet("customers")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<ApiResponse<PagedResult<Customer>>>> ListCustomers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var page = new PageRequest(pageNumber, pageSize);
        var pagedResult = await customerService.GetAllAsync(page);
        return Ok(ApiResponseFactory.Success(pagedResult));
    }

    [HttpGet("customers/{id:guid}")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<ApiResponse<Customer>>> GetCustomer([FromRoute] Guid id)
    {
        var customer = await customerService.GetByIdAsync(id);
        if (customer is null) return NotFound(new ProblemDetails { Title = "Resource not found" });
        return Ok(ApiResponseFactory.Success(customer));
    }

    // Create customer (profile-only)
    [HttpPost("customers")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<ApiResponse<Customer>>> CreateCustomer([FromBody] CustomerProfileDto dto)
    {
        var created = await customerService.CreateCustomer(dto);
        return CreatedAtAction(
            nameof(GetCustomer),
            new { id = created.CustomerId },
            ApiResponseFactory.Success(created));
    }

    [HttpPut("customers/{id:guid}")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<ApiResponse<Customer>>> UpdateCustomer([FromRoute] Guid id, [FromBody] CustomerProfileDto dto)
    {
        var updated = await customerService.UpdateCustomer(id, dto);
        return Ok(ApiResponseFactory.Success(updated));
    }
}