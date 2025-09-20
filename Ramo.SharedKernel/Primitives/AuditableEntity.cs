using SharedKernel.Abstractions.Primitives;

namespace SharedKernel.Primitives;

public abstract class AuditableEntity<TId> : Entity<TId>, IAuditableEntity
{
    protected AuditableEntity(TId id) : base(id) { }

    protected AuditableEntity() : base() { }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public Guid? CreatedById { get; private set; }
    public string? CreatedByName { get; private set; }
    public DateTime? LastUpdatedAt { get; private set; }
    public Guid? LastUpdatedById { get; private set; }
    public string? LastUpdatedByName { get; private set; }
    public string ItemType => GetType().Name;

    public void SetCreated(Guid createdById, string createdByName)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedById = createdById;
        CreatedByName = createdByName;
    }

    public void SetUpdated(Guid updatedById, string updatedByName)
    {
        LastUpdatedAt = DateTime.UtcNow;
        LastUpdatedById = updatedById;
        LastUpdatedByName = updatedByName;
    }

    public void RestoreAudit(DateTime createdAt, DateTime? lastUpdatedAt)
    {
        CreatedAt = createdAt;
        LastUpdatedAt = lastUpdatedAt;
    }
}

public abstract class AuditableEntity : AuditableEntity<Guid>, IAuditableEntity
{
    protected AuditableEntity() : base() { }
    protected AuditableEntity(Guid id) : base(id) { }
}