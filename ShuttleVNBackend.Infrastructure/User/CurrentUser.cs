using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ShuttleVNBackend.Application.Interfaces.User;
using ShuttleVNBackend.Core.Entities.System.Enums;

namespace ShuttleVNBackend.Infrastructure.User;

public class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private ClaimsPrincipal? User => accessor.HttpContext?.User;

    public Guid? ActorId => Guid.TryParse(
        User?.FindFirst("EmployeeId")?.Value
        ?? User?.FindFirst("CustomerId")?.Value, out var id)
        ? id
        : null;

    public ActorType ActorType =>
        User?.FindFirst("EmployeeId") is not null ? ActorType.Employee
        : User?.FindFirst("CustomerId") is not null ? ActorType.Customer
        : accessor.HttpContext is not null ? ActorType.Customer // unauthenticated request = guest customer
        : ActorType.System;
}