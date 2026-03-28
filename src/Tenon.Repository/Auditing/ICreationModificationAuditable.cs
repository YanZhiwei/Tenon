namespace Tenon.Repository;

/// <summary>
/// 创建与修改审计接口，用于跟踪实体的创建时间和最后更新时间。
/// </summary>
public interface ICreationModificationAuditable
{
    /// <summary>
    /// 获取或设置创建时间。
    /// 此属性应在实体创建时设置，之后不应修改。
    /// </summary>
    DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// 获取或设置更新时间。
    /// 此属性应在实体更新时自动设置，初始为 null。
    /// </summary>
    DateTimeOffset? UpdatedAt { get; set; }
}
