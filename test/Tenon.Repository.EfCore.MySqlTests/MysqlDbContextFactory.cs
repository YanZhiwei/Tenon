using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tenon.Repository.EfCore.MySql.Extensions;

namespace Tenon.Repository.EfCore.MySqlTests;

public class MysqlDbContextFactory : IDesignTimeDbContextFactory<MySqlTestDbContext>
{
    public MySqlTestDbContext CreateDbContext(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
        //if (!Debugger.IsAttached) Debugger.Launch();
        var configuration = builder.Build();
        var serviceProvider = new ServiceCollection()
            .AddLogging(loggingBuilder => loggingBuilder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Debug))
            .AddScoped<IAuditContextAccessor, AuditContextAccessor>()
            .AddEfCoreMySql<MySqlTestDbContext>(configuration.GetSection("MySql"))
            .BuildServiceProvider();
        return serviceProvider.GetService<MySqlTestDbContext>();
    }
}