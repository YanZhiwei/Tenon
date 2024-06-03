using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tenon.Repository.EfCore.Extensions;
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
        //modelBuilder.ApplyConfigurations<MySqlTestDbContext>();
    }
}