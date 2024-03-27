using Tenon.Repository;
using Tenon.Repository.EfCore;

namespace CleanArchitecture.Blog.Repository.Entities;

public sealed class Category : EfEntity, ISoftDelete
{
    /// <summary>
    ///     名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     别名
    /// </summary>
    public string Alias { get; set; }

    public bool IsDeleted { get; set; }
}