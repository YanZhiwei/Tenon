using CleanArchitecture.Blog.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tenon.Repository.EfCore;
using Tenon.Repository.EfCore.Sqlite;

namespace CleanArchitecture.Blog.Repository;

public class BlogDbContext(DbContextOptions options)
    : SqliteDbContext(options)
{
    public DbSet<Category> Categories { get; set; }

    public DbSet<FriendLink> FriendLinks { get; set; }

    public DbSet<Post> Posts { get; set; }

    public DbSet<Tag> Tags { get; set; }
    protected override void OnModifiedEntity(EntityEntry<EfBasicAuditEntity> modifiedEntity)
    {
        
    }

    protected override void OnAddedEntity(EntityEntry<EfBasicAuditEntity> addedEntity)
    {
       
    }
}