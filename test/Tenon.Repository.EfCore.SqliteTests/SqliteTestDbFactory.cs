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
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .Build();

        var serviceProvider = new ServiceCollection()
            .AddLogging(loggingBuilder => loggingBuilder
                .SetMinimumLevel(LogLevel.Debug))
            .AddSingleton<AbstractDbContextConfiguration, BlogDbContextConfiguration>()
            .AddEfCoreSqlite<SqliteTestDbContext>(configuration.GetSection("Sqlite"))
            .BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        return scope.ServiceProvider.GetService<SqliteTestDbContext>();
    }
}