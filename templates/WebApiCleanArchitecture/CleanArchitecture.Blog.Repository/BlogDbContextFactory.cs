using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Tenon.Repository.EfCore;
using Tenon.Repository.EfCore.Sqlite.Extensions;

namespace CleanArchitecture.Blog.Repository;

public sealed class BlogDbContextFactory : IDesignTimeDbContextFactory<BlogDbContext>
{
    public BlogDbContext CreateDbContext(string[] args)
    {
        //Debugger.Launch();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .Build();

        var serviceProvider = new ServiceCollection()
            .AddLogging(loggingBuilder => loggingBuilder
                .SetMinimumLevel(LogLevel.Debug))
            .AddSingleton<AbstractDbContextConfiguration, BlogDbContextConfiguration>()
            .AddEfCoreSqlite<BlogDbContext>(configuration.GetSection("Sqlite"))
            .BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        return scope.ServiceProvider.GetService<BlogDbContext>();
    }
}