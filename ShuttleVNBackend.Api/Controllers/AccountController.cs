using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShuttleVNBackend.Api.Common;
using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Application.DTOs.User;
using ShuttleVNBackend.Application.UseCases.User.Services;
using ShuttleVNBackend.Core.Entities.User.Enums;

namespace ShuttleVNBackend.Api.Controllers;

[ApiController]
[Route("accounts")]
public class AccountController(
    AccountService accountService,
    CustomerService customerService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "StaffOnly")]
    public async Task<IActionResult> ListAccounts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var page = new PageRequest(pageNumber, pageSize);
        var pagedResult = await accountService.GetAllAccounts(page);
        return Ok(ApiResponseFactory.Success(pagedResult));
    }

    [HttpPost("{accountId:guid}/lock")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<IActionResult> LockAccount([FromRoute] Guid accountId)
    {
        await accountService.UpdateAccountStatus(accountId, AccountStatus.Disabled);
        return Ok(ApiResponseFactory.Success(new
        {
            message = "Account locked"
        }));
    }

    [HttpPost("{accountId:guid}/unlock")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<IActionResult> UnlockAccount([FromRoute] Guid accountId)
    {
        await accountService.UpdateAccountStatus(accountId, AccountStatus.Active);
        return Ok(ApiResponseFactory.Success(new
        {
            message = "Account unlocked"
        }));
    }

    [HttpGet("customers")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<IActionResult> ListCustomers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var page = new PageRequest(pageNumber, pageSize);
        var pagedResult = await customerService.GetAllCustomers(page);
        return Ok(ApiResponseFactory.Success(pagedResult));
    }

    [HttpGet("customers/{id:guid}")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<IActionResult> GetCustomer([FromRoute] Guid id)
    {
        var customer = await customerService.GetCustomerById(id);
        if (customer is null) return NotFound(new ProblemDetails { Title = "Resource not found" });
        return Ok(ApiResponseFactory.Success(customer));
    }

    // Create customer (profile-only)
    [HttpPost("customers")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<IActionResult> CreateCustomer([FromBody] CustomerProfileDto dto)
    {
        var created = await customerService.CreateCustomer(dto);
        return CreatedAtAction(
            nameof(GetCustomer), 
            new { id = created.CustomerId },
            ApiResponseFactory.Success(created));
    }

    [HttpPut("customers/{id:guid}")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<IActionResult> UpdateCustomer([FromRoute] Guid id, [FromBody] CustomerProfileDto dto)
    {
        var updated = await customerService.UpdateCustomer(id, dto);
        return Ok(ApiResponseFactory.Success(updated));
    }
}
