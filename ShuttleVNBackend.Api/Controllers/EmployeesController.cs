using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShuttleVNBackend.Application.Common;
using ShuttleVNBackend.Application.DTOs.User;
using ShuttleVNBackend.Application.UseCases.User.Services;

namespace ShuttleVNBackend.Api.Controllers;

[ApiController]
[Route("api/employees")]
[Authorize(Policy = "StaffOnly")]
public class EmployeesController(EmployeeService employeeService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ListEmployees([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        => Ok(await employeeService.GetAllEmployees(new PageRequest(pageNumber, pageSize)));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetEmployee([FromRoute] Guid id)
    {
        var employee = await employeeService.GetEmployeeById(id);
        if (employee is null) return NotFound(new ProblemDetails { Title = "Resource not found" });
        return Ok(employee);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto dto)
    {
        var created = await employeeService.CreateEmployee(dto);
        return CreatedAtAction(nameof(GetEmployee), new { id = created.EmployeeId }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateEmployee([FromRoute] Guid id, [FromBody] EmployeeProfileDto dto)
        => Ok(await employeeService.UpdateEmployee(id, dto));

    [HttpPost("{id:guid}/set-admin")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> SetAdminRole([FromRoute] Guid id, [FromBody] SetAdminRoleDto dto)
        => Ok(await employeeService.SetAdminRole(id, dto.IsAdmin));
}