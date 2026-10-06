using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShuttleVNBackend.Api.Common;
using ShuttleVNBackend.Api.DTOs.Common;
using ShuttleVNBackend.Application.DTOs.Courts;
using ShuttleVNBackend.Application.UseCases.Courts.Services;
using ShuttleVNBackend.Core.Entities.Courts;

namespace ShuttleVNBackend.Api.Controllers;

[ApiController]
[Route("api/courts")]
public class PricingRuleController(PricingRuleService pricingRuleService) : ControllerBase
{
    [HttpGet("{id:int}/pricing-rules")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<ApiResponse<List<PricingRule>>>> GetPricingRules(
        [FromRoute] int id,
        CancellationToken ct = default)
    {
        var pricingRules = await pricingRuleService.GetPricingRulesAsync(id, ct);
        return Ok(ApiResponseFactory.Success(pricingRules));
    }

    [HttpPost("{id:int}/pricing-rules")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<PricingRule>>> CreatePricingRule(
        [FromRoute] int id,
        [FromBody] CreatePricingRuleDto dto,
        CancellationToken ct = default)
    {
        var pricingRule = await pricingRuleService.CreatePricingRuleAsync(id, dto, ct);
        return Ok(ApiResponseFactory.Success(pricingRule));
    }

    [HttpPut("{id:int}/pricing-rules/{pricingRuleId:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<PricingRule>>> UpdatePricingRule(
        [FromRoute] int id,
        [FromRoute] int pricingRuleId,
        [FromBody] UpdatePricingRuleDto dto,
        CancellationToken ct = default)
    {
        var pricingRule = await pricingRuleService.UpdatePricingRuleAsync(id, pricingRuleId, dto, ct);
        return Ok(ApiResponseFactory.Success(pricingRule));
    }
    
    [HttpDelete("{id:int}/pricing-rules/{pricingRuleId:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<dynamic>>> DeletePricingRule(
        [FromRoute] int id,
        [FromRoute] int pricingRuleId,
        CancellationToken ct = default)
    {
        await pricingRuleService.DeletePricingRuleAsync(id, pricingRuleId, ct);
        return NotFound();
    }
}