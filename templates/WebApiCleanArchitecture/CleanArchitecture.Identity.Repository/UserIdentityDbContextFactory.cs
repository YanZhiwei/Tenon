using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Identity.Repository;

public sealed class UserIdentityDbContextFactory : IDesignTimeDbContextFactory<UserIdentityDbContext>
{
    public UserIdentityDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .Build();

        var serviceProvider = new ServiceCollection()
            .AddLogging(loggingBuilder => loggingBuilder
                .SetMinimumLevel(LogLevel.Debug))
            .AddDbContext<UserIdentityDbContext>(options =>
            {
                options.UseSqlite(configuration.GetSection("Sqlite:ConnectionString").Value,
                    setup => { setup.MigrationsAssembly("CleanArchitecture.Identity.Repository"); });
            })
            .BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        return scope.ServiceProvider.GetService<UserIdentityDbContext>();
    }
}