using ShuttleVNBackend.Core.Entities.System.Enums;

namespace ShuttleVNBackend.Application.DTOs.System;

public record SearchAuditDto
{
    public string? SearchTerm { get; set; }
    public ActorType? ActorType { get; set; }
    public string? EntityName { get; set; } = null!;
    public string? Action { get; set; } = null!;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}