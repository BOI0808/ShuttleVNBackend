using System.Security.Claims;
using ShuttleVNBackend.Core.Entities.User.Enums;

namespace ShuttleVNBackend.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetAccountId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(value, out var accountId))
            throw new UnauthorizedAccessException("Missing or invalid account identifier claim");
        return accountId;
    }

    public static AccountType GetAccountType(this ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.Role) == "Customer" ? AccountType.Customer : AccountType.Employee;
}