using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tenon.Repository.EfCore.Sqlite;

namespace Tenon.Repository.EfCore.SqliteTests;

public sealed class SqliteTestDbContext(
    DbContextOptions options,
    AbstractDbContextConfiguration? dbContextConfiguration = null,
    IEnumerable<AbstractEntityTypeConfiguration>? entityTypeConfigurations = null)
    : SqliteDbContext(options, dbContextConfiguration, entityTypeConfigurations)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
    }
}