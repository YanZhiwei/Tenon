using System;
using System.Linq;
using System.Reflection;

namespace Tenon.Extensions.System;

public static class AssemblyExtension
{
    public static Type[] GetSubInterfaces(this Assembly assembly, Type interfaceType)
    {
        if (assembly == null) throw new ArgumentNullException(nameof(assembly));
        if (interfaceType == null) throw new ArgumentNullException(nameof(interfaceType));
        var serviceTypes = assembly.GetExportedTypes()
            .Where(type => type.IsInterface && type.IsAssignableTo(interfaceType))?.ToArray();
        return serviceTypes;
    }
}