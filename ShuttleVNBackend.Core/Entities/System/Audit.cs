using ShuttleVNBackend.Core.Entities.System.Enums;

namespace ShuttleVNBackend.Core.Entities.System;

public class Audit
{
    public int Id { get; set; }
    public Guid? ActorId { get; set; }
    public ActorType ActorType { get; set; }
    public string Action { get; set; } = null!;
    public string EntityName { get; set; } = null!;
    public string EntityId { get; set; } = null!;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime CreatedAt { get; set; }
}