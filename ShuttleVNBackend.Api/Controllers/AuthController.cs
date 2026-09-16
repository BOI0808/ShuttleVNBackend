using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShuttleVNBackend.Api.Common;
using ShuttleVNBackend.Api.DTOs.User;
using ShuttleVNBackend.Application.DTOs.Authentication;
using ShuttleVNBackend.Application.UseCases.Authentication.Services;
using ShuttleVNBackend.Application.UseCases.User.Services;
using ShuttleVNBackend.Application.Interfaces.Repositories;
using ShuttleVNBackend.Core.Entities.User.Enums;

namespace ShuttleVNBackend.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    AppAuthService appAuthService,
    AccountService accountService,
    IEmployeeRepository employeeRepository) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var account = await accountService.Register(dto);
        return Created("", ApiResponseFactory.Success(new
        {
            accountId = account.AccountId
        }));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var account = await appAuthService.VerifyLogin(dto);

        var role = account.AccountType == AccountType.Customer ? "Customer" : "Employee";
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
            new(ClaimTypes.Email, account.LoginEmail),
            new(ClaimTypes.Role, role)
        };

        if (account.AccountType == AccountType.Employee)
        {
            var employee = await employeeRepository.GetEmployeeByAccountId(account.AccountId);
            claims.Add(new Claim("IsAdmin", (employee?.IsAdmin ?? false).ToString().ToLowerInvariant()));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true });

        return Ok(ApiResponseFactory.Success(AccountDto.FromEntity(account)));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(ApiResponseFactory.Success(new
        {
            message = "Logged out"
        }));
    }

    [HttpPost("issue-code")]
    [AllowAnonymous]
    public async Task<IActionResult> IssueCode([FromBody] CodeRequestDto dto)
    {
        var code = await appAuthService.IssueCode(dto.Email, dto.Type);
        // return for testing. Implement EmailService later
        return Ok(ApiResponseFactory.Success(code));
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        await appAuthService.ResetPassword(dto);
        return Ok(ApiResponseFactory.Success(new
        {
            message = "Password reset successfully"
        }));
    }
}
