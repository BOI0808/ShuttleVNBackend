using ShuttleVNBackend.Core.Entities.Users;
using ShuttleVNBackend.Core.Entities.Users.Enums;

namespace ShuttleVNBackend.Application.Interfaces.Repositories.Users;

public interface IVerificationCodeRepository
{
    Task<VerificationCode?> GetActiveAsync(string email, CodeType type, CancellationToken ct = default);
    Task DeleteExistingAsync(string email, CodeType type);
}