using CleanArchitecture.Identity.Repository.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Identity.Repository;

public sealed class UserIdentityDbContext(
    DbContextOptions<UserIdentityDbContext> options)
    : IdentityDbContext<User, Role, long>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}