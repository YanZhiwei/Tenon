using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tenon.Repository.EfCore.Sqlite.Extensions;

namespace Tenon.Repository.EfCore.SqliteTests;

public class SqliteTestDbFactory : IDesignTimeDbContextFactory<SqliteTestDbContext>
{
    public SqliteTestDbContext CreateDbContext(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
        var configuration = builder.Build();
        var serviceProvider = new ServiceCollection()
            .AddLogging(loggingBuilder => loggingBuilder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Debug))
            .AddEfCoreSqlite<SqliteTestDbContext>(configuration.GetSection("Sqlite"))
            .BuildServiceProvider();
        return serviceProvider.GetService<SqliteTestDbContext>();
    }
}