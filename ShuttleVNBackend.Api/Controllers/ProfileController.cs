using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShuttleVNBackend.Api.Common;
using ShuttleVNBackend.Api.DTOs.User;
using ShuttleVNBackend.Api.Extensions;
using ShuttleVNBackend.Application.DTOs.User;
using ShuttleVNBackend.Application.UseCases.User.Services;

namespace ShuttleVNBackend.Api.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController(
    ProfileService profileService,
    AccountService accountService) : ControllerBase
{
    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var account = await profileService.UpdateProfile(User.GetAccountId(), User.GetAccountType(), dto);
        return Ok(ApiResponseFactory.Success(AccountDto.FromEntity(account)));
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        await accountService.ChangePassword(User.GetAccountId(), dto);
        return Ok(ApiResponseFactory.Success(new { message = "Password updated successfully" }));
    }
}