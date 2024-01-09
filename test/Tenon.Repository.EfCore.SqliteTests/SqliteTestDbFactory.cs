using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Tenon.Repository.EfCore.SqliteTests;

public class SqliteTestDbFactory : IDesignTimeDbContextFactory<SqliteTestDbContext>
{
    public SqliteTestDbContext CreateDbContext(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

        var configuration = builder.Build();
        var connectString = configuration.GetConnectionString("DefaultConnection");
        var options = new DbContextOptionsBuilder<SqliteTestDbContext>()
            //.UseLazyLoadingProxies()
            .UseSqlite(connectString)
            .Options;
        return new SqliteTestDbContext(options, null);
    }
}