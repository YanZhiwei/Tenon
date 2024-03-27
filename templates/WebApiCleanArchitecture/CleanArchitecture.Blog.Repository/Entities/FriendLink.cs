using Tenon.Repository.EfCore;

namespace CleanArchitecture.Blog.Repository.Entities;

public sealed class FriendLink : EfEntity
{
    /// <summary>
    ///     名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     网址
    /// </summary>
    public string Url { get; set; }
}