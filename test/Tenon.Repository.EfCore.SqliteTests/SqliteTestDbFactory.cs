using System.Diagnostics;
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
        //var builder = new ConfigurationBuilder()
        //    .SetBasePath(Directory.GetCurrentDirectory())
        //    .AddJsonFile("appsettings.json");
        //Debugger.Launch();
        //var configuration = builder.Build();
        //var sqlLiteSection = configuration.GetSection("Sqlite");
        //Debug.WriteLine("[ConnectionString]" + sqlLiteSection.GetConnectionString("ConnectionString"));
        //var options = new DbContextOptionsBuilder<SqliteTestDbContext>()
        //    //.UseLazyLoadingProxies()
        //    .UseSqlite(configuration.GetSection("Sqlite:ConnectionString").Value)
        //    .Options;

        //List<AbstractEntityTypeConfiguration> entityTypeConfigurations =
        //[
        //    new BlogConfig(),
        //    new PostConfig()
        //];
        //return new SqliteTestDbContext(options, new BlogDbContextConfiguration(), entityTypeConfigurations);

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