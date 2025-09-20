namespace SharedKernel.Abstractions.Primitives;

public interface IAuditableEntity
{
    DateTime CreatedAt { get; }
    Guid? CreatedById { get; }
    string? CreatedByName { get; }
    DateTime? LastUpdatedAt { get; }
    Guid? LastUpdatedById { get; }
    string? LastUpdatedByName { get; }
    string ItemType { get; }

    void SetCreated(Guid createdById, string createdByName);
    void SetUpdated(Guid updatedById, string updatedByName);
}