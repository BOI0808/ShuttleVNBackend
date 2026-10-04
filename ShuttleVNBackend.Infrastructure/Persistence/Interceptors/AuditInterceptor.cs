using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ShuttleVNBackend.Application.Interfaces.User;
using ShuttleVNBackend.Core.Attributes;
using ShuttleVNBackend.Core.Entities.System;

namespace ShuttleVNBackend.Infrastructure.Persistence.Interceptors;

public class AuditInterceptor(ICurrentUser currentUser) : SaveChangesInterceptor
{
    private record Pending(
        string Action,
        string EntityName,
        string? EntityId,
        string? Old,
        string? New,
        EntityEntry? EntryForKey
    );

    private readonly List<Pending> _pending = [];

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData e,
        InterceptionResult<int> result)
    {
        Capture(e.Context);
        return base.SavingChanges(e, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData e,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        Capture(e.Context);
        return base.SavingChangesAsync(e, result, ct);
    }

    private void Capture(DbContext? context)
    {
        _pending.Clear();
        if (context is null) return;

        context.ChangeTracker.DetectChanges();

        foreach (var entry in context.ChangeTracker.Entries().ToList())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted))
                continue;
            if (entry.Metadata.FindPrimaryKey() is null) continue;
            if (entry.Entity is Audit || entry.Metadata.ClrType.IsDefined(typeof(NoAuditAttribute), true)) continue;

            var props = entry.Properties
                .Where(p => !p.Metadata.PropertyInfo?.IsDefined(typeof(NoAuditAttribute), true) ?? true)
                .Where(p => entry.State != EntityState.Modified || p.IsModified)
                .ToList();

            if (entry.State == EntityState.Modified && props.Count == 0) continue;

            var oldEntry = entry.State == EntityState.Added ? null
                : Serialize(props.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue));
            var newEntry = entry.State == EntityState.Deleted ? null
                : Serialize(props.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue));

            _pending.Add(new Pending(
                entry.State.ToString(),
                entry.Metadata.ClrType.Name,
                entry.State == EntityState.Added ? null : KeyOf(entry),  // Added: key not generated yet
                oldEntry, newEntry,
                entry.State == EntityState.Added ? entry : null));
        }
    }
    
    // Entity key only exists after saved to the database so we write log here
    public override int SavedChanges(SaveChangesCompletedEventData e, int result)
    {
        var logs = BuildLogs();
        if (logs.Count <= 0 || e.Context is null)
            return base.SavedChanges(e, result);
        
        e.Context.Set<Audit>().AddRange(logs);
        e.Context.SaveChanges();
        return base.SavedChanges(e, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData e, int result, CancellationToken ct = default)
    {
        var logs = BuildLogs();
        if (logs.Count <= 0 || e.Context is null)
            return await base.SavedChangesAsync(e, result, ct);
        
        e.Context.Set<Audit>().AddRange(logs);
        await e.Context.SaveChangesAsync(ct);
        return await base.SavedChangesAsync(e, result, ct);
    }

    public override void SaveChangesFailed(DbContextErrorEventData e) => _pending.Clear();
    public override Task SaveChangesFailedAsync(DbContextErrorEventData e, CancellationToken ct = default)
    {
        _pending.Clear();
        return Task.CompletedTask;
    }

    private List<Audit> BuildLogs()
    {
        var now = DateTime.UtcNow;
        var logs = _pending.Select(p => new Audit
        {
            ActorType  = currentUser.ActorType,
            ActorId    = currentUser.ActorId,
            Action     = p.Action,
            EntityName = p.EntityName,
            EntityId   = p.EntityId ?? KeyOf(p.EntryForKey!),
            OldValue   = p.Old,
            NewValue   = p.New,
            CreatedAt  = now
        }).ToList();

        _pending.Clear();   // clear BEFORE the nested SaveChanges so it doesn't re-log
        return logs;
    }

    private static string KeyOf(EntityEntry entry) =>
        string.Join(",", entry.Metadata.FindPrimaryKey()!.Properties
            .Select(p => entry.Property(p.Name).CurrentValue));

    private static string Serialize(Dictionary<string, object?> d) => JsonSerializer.Serialize(d);
}