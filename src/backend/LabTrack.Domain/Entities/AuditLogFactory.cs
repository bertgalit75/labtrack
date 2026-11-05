namespace LabTrack.Domain.Entities;

public sealed partial class AuditLog
{
    public static AuditLog Create(long? actorUserId, string actionType, string entityType,
        long? entityId, string? description = null) =>
        new()
        {
            ActorUserId = actorUserId,
            ActionType = actionType,
            EntityType = entityType,
            EntityId = entityId,
            Description = description,
            OccurredAtUtc = DateTime.UtcNow
        };
}
