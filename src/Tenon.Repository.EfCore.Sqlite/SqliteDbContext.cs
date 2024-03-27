using Microsoft.EntityFrameworkCore;

namespace Tenon.Repository.EfCore.Sqlite
{
    public abstract class SqliteDbContext(DbContextOptions options)
        : AuditDbContext(options)
    {

    }
}
