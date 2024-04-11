using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Tenon.Extensions.System;

public static class TypeExtension
{
    public static bool IsTaskWithResult(this TypeInfo typeInfo)
    {
        if (typeInfo == null) throw new ArgumentNullException(nameof(typeInfo));

        return typeInfo.GetGenericTypeDefinition() == typeof(Task<>);
    }

    public static bool IsTask(this TypeInfo typeInfo)
    {
        if (typeInfo == null) throw new ArgumentNullException(nameof(typeInfo));
        return typeInfo.AsType() == typeof(Task);
    }
}