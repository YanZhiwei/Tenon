namespace Tenon.Repository.EfCore;

/// <summary>
/// EF Core 租户解析器接口
/// </summary>
public interface IEfTenantResolver : ITenantResolver<long, long>, IEfUserResolver
{
}
