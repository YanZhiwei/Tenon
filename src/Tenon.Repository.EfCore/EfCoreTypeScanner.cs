using System.Reflection;

namespace Tenon.Repository.EfCore;

/// <summary>
/// 扫描程序集中符合仓储约定的实体类型
/// </summary>
internal static class EfCoreTypeScanner
{
    /// <summary>
    /// 获取程序集中所有实现了 IEntity&lt;TKey&gt; 的非抽象具体实体类型
    /// </summary>
    internal static Type[] GetEntityTypes(Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(type =>
                type is { IsAbstract: false, IsInterface: false } &&
                type.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntity<>)))
            .ToArray();
    }
}
