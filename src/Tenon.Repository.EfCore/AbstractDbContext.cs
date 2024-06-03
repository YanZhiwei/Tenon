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