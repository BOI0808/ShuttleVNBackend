using ShuttleVNBackend.Core.Entities.System;
using ShuttleVNBackend.Core.Entities.System.Enums;

namespace ShuttleVNBackend.Application.DTOs.System;

public record AuditDto
{
    public int Id { get; set; }
    public ActorType ActorType { get; set; }
    public Guid? ActorId { get; set; }
    public string ActorName { get; set; } = "";
    public string Action { get; set; } = "";
    public string EntityName { get; set; } = "";
    public string EntityId { get; set; } = "";
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime CreatedAt { get; set; }

    public static AuditDto FromEntity(Audit audit, Guid? actorId, string actorName = "")
    {
        return new AuditDto
        {
            Id = audit.Id,
            ActorType = audit.ActorType,
            ActorId = actorId,
            ActorName = actorName,
            Action = audit.Action,
            EntityName = audit.EntityName,
            EntityId = audit.EntityId,
            OldValue = audit.OldValue,
            NewValue = audit.NewValue,
            CreatedAt = audit.CreatedAt
        };
    }
}