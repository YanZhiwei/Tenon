using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Tenon.Extensions.System;

public static class ReflectionExtension
{
    public static bool IsReturnTask(this MethodInfo methodInfo)
    {
        if (methodInfo == null) throw new ArgumentNullException(nameof(methodInfo));
        var returnType = methodInfo.ReturnType.GetTypeInfo();
        return returnType.IsTaskWithResult();
    }

 

    public static bool IsNotAbstractClass(this Type type, bool publicOnly)
    {
        if (type.IsSpecialName)
            return false;

        if (!type.IsClass || type.IsAbstract) return false;
        if (type.HasAttribute<CompilerGeneratedAttribute>())
            return false;

        if (publicOnly)
            return type.IsPublic || type.IsNestedPublic;

        return true;
    }

    public static bool HasAttribute(this Type type, Type attributeType)
    {
        return type.IsDefined(attributeType, true);
    }

    public static bool HasAttribute<T>(this Type type) where T : Attribute
    {
        return type.HasAttribute(typeof(T));
    }

    public static bool HasAttribute<T>(this Type type, Func<T, bool> predicate) where T : Attribute
    {
        return type.GetCustomAttributes<T>(true).Any(predicate);
    }
}