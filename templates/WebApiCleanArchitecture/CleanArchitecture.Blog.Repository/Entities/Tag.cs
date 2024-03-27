using Tenon.Repository.EfCore;

namespace CleanArchitecture.Blog.Repository.Entities;

public sealed class Tag : EfEntity
{
    /// <summary>
    ///     名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     别名
    /// </summary>
    public string Alias { get; set; }
}