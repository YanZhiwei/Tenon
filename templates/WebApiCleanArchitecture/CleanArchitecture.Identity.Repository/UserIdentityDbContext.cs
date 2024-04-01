using CleanArchitecture.Identity.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Tenon.AspNetCore.Identity.EfCore.Extensions;
using Tenon.Repository.EfCore;

namespace CleanArchitecture.Identity.Repository;

public sealed class UserIdentityDbContext(
    DbContextOptions<UserIdentityDbContext> options,
    AbstractDbContextConfiguration? dbContextConfiguration,
    IEnumerable<AbstractEntityTypeConfiguration>? entityTypeConfigurations)
    : AbstractIdentityDbContext<User, Role, long>(options, dbContextConfiguration,
        entityTypeConfigurations)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}