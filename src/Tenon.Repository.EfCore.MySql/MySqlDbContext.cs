using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Tenon.Repository.EfCore.MySql;

public class MySqlDbContext : AuditDbContext
{
    public MySqlDbContext(DbContextOptions options, ClaimsPrincipal claimsPrincipal) : base(options, claimsPrincipal)
    {
    }

    public MySqlDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasCharSet("utf8mb4");
        base.OnModelCreating(modelBuilder);
    }
}