using CleanArchitecture.Identity.Repository.Entities;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tenon.AspNetCore.Identity.EfCore.Sqlite.Extensions.Extensions;
using Tenon.Repository.EfCore;

namespace CleanArchitecture.Identity.Repository;

public sealed class UserIdentityDbContextFactory : IDesignTimeDbContextFactory<UserIdentityDbContext>
{
    /// <summary>
    ///  add-migration CreateIdentitySchema -Context UserIdentityDbContext
    ///  update-database -Context UserIdentityDbContext
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public UserIdentityDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .Build();

        var serviceProvider = new ServiceCollection()
            .AddLogging(loggingBuilder => loggingBuilder
                .SetMinimumLevel(LogLevel.Debug))
            .AddIdentityEfCoreSqlite<UserIdentityDbContext, User, Role, long>(configuration.GetSection("Sqlite"))
            .BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        return scope.ServiceProvider.GetService<UserIdentityDbContext>();
    }
}