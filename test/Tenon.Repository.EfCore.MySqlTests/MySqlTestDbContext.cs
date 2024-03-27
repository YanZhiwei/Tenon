using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Tenon.Repository.EfCore.MySql;
using Tenon.Repository.EfCore.MySqlTests.Entities;

namespace Tenon.Repository.EfCore.MySqlTests;

public sealed class MySqlTestDbContext(DbContextOptions options)
    : MySqlDbContext(options)
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // optionsBuilder.UseLazyLoadingProxies();
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>().ToTable("blogs");
        modelBuilder.Entity<Post>().ToTable("posts");
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnModifiedEntity(EntityEntry<EfBasicAuditEntity> modifiedEntity)
    {
        throw new NotImplementedException();
    }

    protected override void OnAddedEntity(EntityEntry<EfBasicAuditEntity> addedEntity)
    {
        throw new NotImplementedException();
    }

    //protected override long GetUserId(IAuditContextAccessor context)
    //{
    //    var claims = new List<Claim>
    //    {
    //        new(ClaimTypes.Name, "tenon"),
    //        new(ClaimTypes.NameIdentifier, new Random().NextInt64(0, 1000).ToString())
    //    };
    //    var cookieClaimsIdentity = new ClaimsIdentity(claims, "Cookies");
    //    var efClaimsPrincipal = new ClaimsPrincipal(cookieClaimsIdentity);
    //    context.Principal = context.Principal ?? efClaimsPrincipal;
    //    return long.TryParse(context.Principal.Claims?.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
    //        out var nameIdentifier)
    //        ? nameIdentifier
    //        : -1;
    //}
}