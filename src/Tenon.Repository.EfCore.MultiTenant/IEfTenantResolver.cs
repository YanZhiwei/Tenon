namespace Tenon.Repository.EfCore.MultiTenant;

/// <summary>
/// EF Core 租户解析器接口，组合租户解析与当前用户标识
/// </summary>
public interface IEfTenantResolver : ITenantResolver<long, long>
{
}
