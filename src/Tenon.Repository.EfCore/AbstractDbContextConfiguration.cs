using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Tenon.Repository.EfCore;

public abstract class AbstractDbContextConfiguration
{
    public abstract void OnModifiedEntity(EntityEntry<EfBasicAuditEntity> modifiedEntity);
    public abstract void OnAddedEntity(EntityEntry<EfBasicAuditEntity> addedEntity);
    public abstract void SetTableName(ModelBuilder modelBuilder);
    public abstract void SetComment(ModelBuilder modelBuilder);
}