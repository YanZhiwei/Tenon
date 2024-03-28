using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tenon.Repository.EfCore;
using Tenon.Repository.EfCore.Sqlite;

namespace CleanArchitecture.Blog.Repository;

public class BlogDbContext(
    DbContextOptions options,
    AbstractDbContextConfiguration? dbContextConfiguration = null,
    IEnumerable<AbstractEntityTypeConfiguration>? entityTypeConfigurations = null)
    : SqliteDbContext(options, dbContextConfiguration, entityTypeConfigurations)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // optionsBuilder.UseLazyLoadingProxies();
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
    }
}