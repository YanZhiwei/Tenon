using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tenon.Repository.EfCore.Extensions;
using Tenon.Repository.EfCore.SqliteTests.Entities;

namespace Tenon.Repository.EfCore.SqliteTests;

public sealed class SqliteTestDbContext(
    DbContextOptions options)
    : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>().ToTable("blogs");
        modelBuilder.Entity<Post>().ToTable("posts");
        modelBuilder.ApplyConfigurations<SqliteTestDbContext>();
        base.OnModelCreating(modelBuilder);
    }
}