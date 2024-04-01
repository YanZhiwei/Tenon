using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tenon.Repository.EfCore;

namespace CleanArchitecture.Identity.Repository;

public sealed class IdentityDbContextConfiguration : AbstractDbContextConfiguration
{
    public override void OnModifiedEntity(EntityEntry<EfBasicAuditEntity> modifiedEntity)
    {
    }

    public override void OnAddedEntity(EntityEntry<EfBasicAuditEntity> addedEntity)
    {
    }

    public override void SetTableName(ModelBuilder modelBuilder)
    {
    }

    public override void SetComment(ModelBuilder modelBuilder)
    {
    }
}