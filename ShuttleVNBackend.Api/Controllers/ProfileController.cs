using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShuttleVNBackend.Api.Common;
using ShuttleVNBackend.Api.DTOs.Users;
using ShuttleVNBackend.Api.Extensions;
using ShuttleVNBackend.Application.DTOs.Users;
using ShuttleVNBackend.Application.UseCases.Users.Services;
using ShuttleVNBackend.Application.DTOs.Courts;
using ShuttleVNBackend.Core.Entities.Courts;

namespace ShuttleVNBackend.Api.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController(
    ProfileService profileService,
    AccountService accountService) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult<ApiResponse<AccountDto>>> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var account = await profileService.UpdateProfile(User.GetAccountId(), User.GetAccountType(), dto);
        return Ok(ApiResponseFactory.Success(AccountDto.FromEntity(account)));
    }

    [HttpPut("change-password")]
    public async Task<ActionResult<ApiResponse<dynamic>>> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        await accountService.ChangePassword(User.GetAccountId(), dto);
        return Ok(ApiResponseFactory.Success(new { message = "Password updated successfully" }));
    }
}