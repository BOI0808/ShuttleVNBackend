using ShuttleVNBackend.Core.Entities.System.Enums;

namespace ShuttleVNBackend.Application.Interfaces.User;

public interface ICurrentUser
{
    ActorType ActorType { get; }
    Guid? ActorId { get; }
}