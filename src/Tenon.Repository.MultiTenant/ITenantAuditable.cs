namespace Tenon.Repository;

/// <summary>
/// 定义多租户审计实体接口
/// </summary>
/// <typeparam name="TUserKey">用户主键类型</typeparam>
/// <typeparam name="TTenantKey">租户主键类型</typeparam>
public interface ITenantAuditable<TUserKey, TTenantKey> : IFullAuditable<TUserKey>, ITenant<TTenantKey>
    where TUserKey : struct
{
}
