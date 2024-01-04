using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Tenon.Repository.EfCoreTests;

public class AuditDbContextFactory : IDesignTimeDbContextFactory<AuditTestDbContext>
{
    public AuditTestDbContext CreateDbContext(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

        var configuration = builder.Build();
        var connectString = configuration.GetConnectionString("DefaultConnection");
        var options = new DbContextOptionsBuilder<AuditTestDbContext>()
            //.UseLazyLoadingProxies()
            .UseSqlite(connectString)
            .Options;
        return new AuditTestDbContext(options);
    }
}