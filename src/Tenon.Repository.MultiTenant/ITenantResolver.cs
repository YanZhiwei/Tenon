namespace Tenon.Repository;

/// <summary>
/// 租户解析器接口
/// </summary>
/// <typeparam name="TUserKey">用户主键类型</typeparam>
/// <typeparam name="TTenantKey">租户主键类型</typeparam>
public interface ITenantResolver<TUserKey, TTenantKey> : ICurrentUser<TUserKey>
{
    /// <summary>
    /// 获取或设置当前租户标识
    /// </summary>
    TTenantKey TenantId { get; set; }
}
