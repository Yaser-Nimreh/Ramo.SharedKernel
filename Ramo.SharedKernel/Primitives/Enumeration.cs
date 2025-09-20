using System.Reflection;

namespace SharedKernel.Primitives;

public abstract class Enumeration<TEnum> : IEquatable<Enumeration<TEnum>>, IComparable<Enumeration<TEnum>>
    where TEnum : Enumeration<TEnum>
{
    private static readonly string EnumerationName = typeof(TEnum).Name;
    private static readonly Lazy<Dictionary<int, TEnum>> Enumerations =
        new(() => GetAllEnumerationOptions().ToDictionary(e => e.Value));

    protected Enumeration(int value, string name)
    {
        Value = value;
        Name = name;
    }

    protected Enumeration()
    {
        Value = default;
        Name = string.Empty;
    }

    public int Value { get; protected init; }
    public string Name { get; protected init; }

    public static IReadOnlyCollection<TEnum> GetValues() =>
        Enumerations.Value.Values.ToList().AsReadOnly();

    public static TEnum? FromValue(int value) =>
        Enumerations.Value.TryGetValue(value, out var enumeration)
            ? enumeration
            : throw new InvalidOperationException($"{EnumerationName} with value {value} not found.");

    public static TEnum? FromName(string name) =>
        Enumerations.Value.Values.SingleOrDefault(e =>
            string.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase));

    public static bool ContainsValue(int value) => Enumerations.Value.ContainsKey(value);

    public bool Equals(Enumeration<TEnum>? other) =>
        other is not null && GetType() == other.GetType() && other.Value.Equals(Value);

    public override bool Equals(object? obj) =>
        obj is Enumeration<TEnum> other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public int CompareTo(Enumeration<TEnum>? other) =>
        other is null ? 1 : Value.CompareTo(other.Value);

    public static bool operator ==(Enumeration<TEnum>? first, Enumeration<TEnum>? second) =>
        first is not null && second is not null && Equals(first, second);

    public static bool operator !=(Enumeration<TEnum>? first, Enumeration<TEnum>? second) =>
        !(first == second);

    public static bool operator <(Enumeration<TEnum>? first, Enumeration<TEnum>? second) =>
        first is null ? second is not null : first.CompareTo(second) < 0;

    public static bool operator <=(Enumeration<TEnum>? first, Enumeration<TEnum>? second) =>
        first is null || first.CompareTo(second) <= 0;

    public static bool operator >(Enumeration<TEnum>? first, Enumeration<TEnum>? second) =>
        first is not null && first.CompareTo(second) > 0;

    public static bool operator >=(Enumeration<TEnum>? first, Enumeration<TEnum>? second) =>
        first is null ? second is null : first.CompareTo(second) >= 0;

    public override string ToString() => Name;

    private static IEnumerable<TEnum> GetAllEnumerationOptions()
    {
        var enumType = typeof(TEnum);
        return Assembly
            .GetAssembly(enumType)!
            .GetTypes()
            .Where(type => enumType.IsAssignableFrom(type))
            .SelectMany(GetFieldsOfType<TEnum>);
    }

    private static IEnumerable<TFieldType> GetFieldsOfType<TFieldType>(Type type) =>
        type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fieldInfo => type.IsAssignableFrom(fieldInfo.FieldType))
            .Select(fieldInfo => (TFieldType)fieldInfo.GetValue(null)!);
}