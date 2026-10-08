using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShuttleVNBackend.Api.Common;
using ShuttleVNBackend.Api.DTOs.Common;
using ShuttleVNBackend.Application.DTOs.Courts;
using ShuttleVNBackend.Application.UseCases.Courts.Services;
using ShuttleVNBackend.Core.Entities.Courts;

namespace ShuttleVNBackend.Api.Controllers;

[ApiController]
[Route("api/courts/{id:int}/pricing-rules")]
public class PricingRuleController(PricingRuleService pricingRuleService) : ControllerBase
{
    [HttpGet("")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PricingRule>>>> GetPricingRules(
        [FromRoute] int id,
        CancellationToken ct = default)
    {
        var pricingRules = await pricingRuleService.GetPricingRulesAsync(id, ct);
        return Ok(ApiResponseFactory.Success(pricingRules));
    }

    [HttpPut("{dayOfWeek:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PricingRule>>>> UpsertPricingRule(
        [FromRoute] int id,
        [FromRoute] int dayOfWeek,
        [FromBody] SavePricingRuleDto dto,
        CancellationToken ct = default)
    {
        var pricingRules = await pricingRuleService.SavePricingRulesAsync(id, dayOfWeek, dto, ct);
        return Ok(ApiResponseFactory.Success(pricingRules));
    }
}