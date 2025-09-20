namespace SharedKernel.Primitives;

public abstract class ValueObject<TValue> : IEquatable<ValueObject<TValue>>
    where TValue : ValueObject<TValue>
{
    protected abstract IEnumerable<object?> GetAtomicValues();

    private bool ValuesAreEqual(ValueObject<TValue> other) =>
        GetAtomicValues().SequenceEqual(other.GetAtomicValues());

    public bool Equals(ValueObject<TValue>? other) =>
        other is not null && GetType() == other.GetType() && ValuesAreEqual(other);

    public override bool Equals(object? obj) =>
        obj is ValueObject<TValue> other && Equals(other);

    public override int GetHashCode() =>
        GetAtomicValues()
            .Aggregate(1, (hash, component) =>
                HashCode.Combine(hash, component?.GetHashCode() ?? 0));

    public static bool operator ==(ValueObject<TValue>? first, ValueObject<TValue>? second) =>
        ReferenceEquals(first, second) || first is not null && first.Equals(second);

    public static bool operator !=(ValueObject<TValue>? first, ValueObject<TValue>? second) =>
        !(first == second);
}