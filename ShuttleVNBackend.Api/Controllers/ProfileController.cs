using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShuttleVNBackend.Api.Extensions;
using ShuttleVNBackend.Application.DTOs.User;
using ShuttleVNBackend.Application.UseCases.User.Services;

namespace ShuttleVNBackend.Api.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController(ProfileService profileService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMyProfile()
        => Ok(await profileService.GetMyProfile(User.GetAccountId(), User.GetAccountType()));

    [HttpPut]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateMyProfileDto dto)
        => Ok(await profileService.UpdateMyProfile(User.GetAccountId(), User.GetAccountType(), dto));
}