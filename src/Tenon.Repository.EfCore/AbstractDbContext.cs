using Microsoft.EntityFrameworkCore;

namespace Tenon.Repository.EfCore;

public abstract class AbstractDbContext : DbContext
{
    protected readonly AbstractDbContextConfiguration? DbContextConfiguration;
    protected readonly IEnumerable<AbstractEntityTypeConfiguration>? EntityTypeConfigurations;

    protected AbstractDbContext(DbContextOptions options, AbstractDbContextConfiguration? dbContextConfiguration = null,
        IEnumerable<AbstractEntityTypeConfiguration>? entityTypeConfigurations = null) : base(options)
    {
        DbContextConfiguration = dbContextConfiguration;
        EntityTypeConfigurations = entityTypeConfigurations;
        Database.AutoTransactionsEnabled = false;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (ChangeTracker.Entries<EfEntity>().Any())
        {
            var addEntityEntries =
                ChangeTracker.Entries<EfBasicAuditEntity>().Where(x => x.State == EntityState.Added);
            foreach (var addedEntity in addEntityEntries)
            {
                addedEntity.Entity.CreateTime = DateTime.UtcNow;
                DbContextConfiguration?.OnAddedEntity(addedEntity);
            }

            var modifiedEntities =
                ChangeTracker.Entries<EfBasicAuditEntity>().Where(x => x.State == EntityState.Modified);
            foreach (var modifiedEntity in modifiedEntities)
            {
                modifiedEntity.Entity.ModifyTime = DateTime.UtcNow;
                DbContextConfiguration?.OnModifiedEntity(modifiedEntity);
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //https://github.com/dotnet/efcore/issues/23103
        base.OnModelCreating(modelBuilder);
        if (EntityTypeConfigurations != null)
            foreach (var entityTypeConfiguration in EntityTypeConfigurations)
                entityTypeConfiguration.Configure(modelBuilder);
        DbContextConfiguration?.SetTableName(modelBuilder);
        DbContextConfiguration?.SetComment(modelBuilder);
    }
}