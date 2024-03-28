using Microsoft.EntityFrameworkCore;

namespace Tenon.Repository.EfCore.Sqlite;

public abstract class SqliteDbContext(
    DbContextOptions options,
    AbstractDbContextConfiguration? dbContextConfiguration = null,
    IEnumerable<AbstractEntityTypeConfiguration>? entityTypeConfigurations = null)
    : AbstractDbContext(options, dbContextConfiguration, entityTypeConfigurations)
{

}