using CleanArchitecture.Blog.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tenon.Repository.EfCore;

public sealed class BlogDbContextConfiguration : AbstractDbContextConfiguration
{
    public override void OnModifiedEntity(EntityEntry<EfBasicAuditEntity> modifiedEntity)
    {
    }

    public override void OnAddedEntity(EntityEntry<EfBasicAuditEntity> addedEntity)
    {
    }

    public override void SetTableName(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().ToTable("categories");
        modelBuilder.Entity<Post>().ToTable("posts");
        modelBuilder.Entity<FriendLink>().ToTable("friendLinks");
        modelBuilder.Entity<Tag>().ToTable("tags");
    }

    public override void SetComment(ModelBuilder modelBuilder)
    {
    }
}