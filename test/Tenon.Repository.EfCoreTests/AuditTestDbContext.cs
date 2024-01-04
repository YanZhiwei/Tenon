using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tenon.Repository.EfCore;
using Tenon.Repository.EfCoreTests.Entities;

namespace Tenon.Repository.EfCoreTests;

public sealed class AuditTestDbContext : AuditDbContext<long>
{
    public AuditTestDbContext(DbContextOptions option) : base(option)
    {
      
    }

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

    protected override Operator<long>? GetOperator()
    {
        return new Operator<long>
        {
            Id = 0,
            Account = "Test"
        };
    }
}