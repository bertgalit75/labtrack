namespace LabTrack.Application.Abstractions;

public interface ICurrentUser
{
    long? UserId { get; }
    bool IsAuthenticated { get; }
}

public sealed record AuditEntry(long? ActorUserId, string ActionCode, string TargetType, string TargetId, string OutcomeCode, string? Reason = null);

public interface IAuditWriter
{
    Task WriteAsync(AuditEntry entry, CancellationToken cancellationToken);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
