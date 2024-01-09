using Microsoft.EntityFrameworkCore;

namespace Tenon.Repository.EfCore.MySql;

public abstract class MySqlDbContext(DbContextOptions options, IAuditContextAccessor auditContext)
    : AuditDbContext(options, auditContext)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasCharSet("utf8mb4");
        base.OnModelCreating(modelBuilder);
    }
}