namespace Tenon.Repository;

/// <summary>
/// 定义多租户实体接口
/// </summary>
/// <typeparam name="TKey">租户标识的类型</typeparam>
public interface ITenant<TKey>
{
    /// <summary>
    /// 获取或设置租户标识
    /// </summary>
    TKey TenantId { get; set; }
}
