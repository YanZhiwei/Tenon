using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tenon.Repository.EfCore;

namespace Tenon.AspNetCore.Identity.EfCore.Extensions;

public abstract class AbstractIdentityDbContext<TUser, TRole, TKey> : IdentityDbContext<TUser, TRole, TKey,
    IdentityUserClaim<TKey>, IdentityUserRole<TKey>, IdentityUserLogin<TKey>, IdentityRoleClaim<TKey>,
    IdentityUserToken<TKey>>
    where TUser : IdentityUser<TKey>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
{
    protected readonly AbstractDbContextConfiguration? DbContextConfiguration;
    protected readonly IEnumerable<AbstractEntityTypeConfiguration>? EntityTypeConfigurations;

    protected AbstractIdentityDbContext(DbContextOptions options, AbstractDbContextConfiguration? dbContextConfiguration,
        IEnumerable<AbstractEntityTypeConfiguration>? entityTypeConfigurations) : base(options)
    {
        DbContextConfiguration =
            dbContextConfiguration ?? throw new ArgumentNullException(nameof(dbContextConfiguration));
        EntityTypeConfigurations =
            entityTypeConfigurations ?? throw new ArgumentNullException(nameof(entityTypeConfigurations));
        Database.AutoTransactionsEnabled = false;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        if (EntityTypeConfigurations != null)
            foreach (var entityTypeConfiguration in EntityTypeConfigurations)
                entityTypeConfiguration.Configure(modelBuilder);
        DbContextConfiguration?.SetTableName(modelBuilder);
        DbContextConfiguration?.SetComment(modelBuilder);
    }
}