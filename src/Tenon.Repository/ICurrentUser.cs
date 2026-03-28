namespace Tenon.Repository;

/// <summary>
/// 当前用户上下文接口，提供审计所需的用户标识
/// </summary>
/// <typeparam name="TKey">用户主键类型</typeparam>
public interface ICurrentUser<TKey>
{
    /// <summary>
    /// 当前用户主键
    /// </summary>
    TKey UserId { get; }
}
