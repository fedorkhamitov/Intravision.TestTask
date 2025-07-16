namespace Intravision.TestTask.Domain.Shared;

public abstract class ValueObject
{
    private int? _cachedHashCode;

    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is null || GetUnproxiedType(this) != GetUnproxiedType(obj))
            return false;

        var valueObject = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        if (_cachedHashCode.HasValue)
            return _cachedHashCode.Value;

        int hash = 1;
        foreach (var component in GetEqualityComponents())
        {
            hash = hash * 23 + (component?.GetHashCode() ?? 0);
        }
        _cachedHashCode = hash;
        return hash;
    }

    public static bool operator ==(ValueObject? a, ValueObject? b)
    {
        if (ReferenceEquals(a, b))
            return true;
        if (a is null || b is null)
            return false;
        return a.Equals(b);
    }

    public static bool operator !=(ValueObject? a, ValueObject? b) => !(a == b);
    
    internal static Type GetUnproxiedType(object obj)
    {
        var type = obj.GetType();
        var typeName = type.ToString();
        if ((typeName.Contains("Castle.Proxies.") || typeName.EndsWith("Proxy")) && type.BaseType != null)
            return type.BaseType;
        return type;
    }
}