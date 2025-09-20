namespace SharedKernel.Abstractions.Primitives;

public interface ISoftDeletableEntity
{
    bool IsDeleted { get; }
    DateTime? DeletedAt { get; }
    Guid? DeletedById { get; }
    string? DeletedByName { get; }

    void Delete(Guid? deletedById = null, string? deletedByName = null);
    void UnDelete();
}