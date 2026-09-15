using ShuttleVNBackend.Core.Entities.User;
using ShuttleVNBackend.Core.Entities.User.Enums;

namespace ShuttleVNBackend.Api.DTOs.User;

public class AccountDto
{
    public Guid AccountId { get; set; }
    public AccountType AccountType { get; set; }
    public AccountStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Customer? Customer { get; set; }
    public Employee? Employee { get; set; }

    public static AccountDto FromEntity(UserAccount entity)
    {
        return new AccountDto
        {
            AccountId = entity.AccountId,
            AccountType = entity.AccountType,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Customer = entity.Customer,
            Employee = entity.Employee
        };
    }
}