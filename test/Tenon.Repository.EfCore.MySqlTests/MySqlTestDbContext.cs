using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tenon.Repository.EfCore.MySql;
using Tenon.Repository.EfCore.MySqlTests.Entities;

namespace Tenon.Repository.EfCore.MySqlTests;

public sealed class MySqlTestDbContext(DbContextOptions options, IAuditContextAccessor auditContext)
    : MySqlDbContext(options, auditContext)
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

    protected override long GetUserId(IAuditContextAccessor context)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "tenon"),
            new(ClaimTypes.NameIdentifier, new Random().NextInt64(0, 1000).ToString())
        };
        var cookieClaimsIdentity = new ClaimsIdentity(claims, "Cookies");
        var efClaimsPrincipal = new ClaimsPrincipal(cookieClaimsIdentity);
        context.Principal = context.Principal ?? efClaimsPrincipal;
        return long.TryParse(context.Principal.Claims?.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
            out var nameIdentifier)
            ? nameIdentifier
            : -1;
    }
}