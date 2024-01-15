namespace Tenon.Caching;

/// <summary>
/// reference https://github.com/FoundatioFx/Foundatio/blob/HEAD/src/Foundatio/Caching/CacheValue.cs
/// </summary>
public readonly struct CacheValue<T>(T value, bool hasValue)
{
    public bool HasValue { get; } = hasValue;

    public bool IsNull => Value == null;

    public T Value { get; } = value;

    public static CacheValue<T> Null { get; } = new(default, true);

    public static CacheValue<T> NoValue { get; } = new(default, false);

    public override string ToString()
    {
        return Value?.ToString() ?? "<null>";
    }
}