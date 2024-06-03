using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tenon.Repository.EfCore.MySqlTests.Entities;

namespace Tenon.Repository.EfCore.MySqlTests;

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
        modelBuilder.Entity<Blog>().ToTable("blogs");
        modelBuilder.Entity<Post>().ToTable("posts");
    }

    public override void SetComment(ModelBuilder modelBuilder)
    {
    }
}