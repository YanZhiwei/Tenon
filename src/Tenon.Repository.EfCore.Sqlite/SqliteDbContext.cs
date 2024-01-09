using Microsoft.EntityFrameworkCore;

namespace Tenon.Repository.EfCore.Sqlite
{
    public abstract class SqliteDbContext(DbContextOptions options, IAuditContextAccessor auditContext)
        : AuditDbContext(options, auditContext)
    {

    }
}
