using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tenon.Repository.EfCore.Sqlite;

namespace Tenon.Repository.EfCore.SqliteTests;

public sealed class SqliteTestDbContext(
    DbContextOptions options,
    AbstractDbContextConfiguration? dbContextConfiguration = null,
    IEnumerable<AbstractEntityTypeConfiguration>? entityTypeConfigurations = null)
    : SqliteDbContext(options, dbContextConfiguration, entityTypeConfigurations)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // optionsBuilder.UseLazyLoadingProxies();
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
    }

    //private long GetUserId(IAuditContextAccessor context)
    //{
    //    var claims = new List<Claim>
    //    {
    //        new(ClaimTypes.Name, "tenon"),
    //        new(ClaimTypes.NameIdentifier, new Random().NextInt64(0, 1000).ToString())
    //    };
    //    var cookieClaimsIdentity = new ClaimsIdentity(claims, "Cookies");
    //    var efClaimsPrincipal = new ClaimsPrincipal(cookieClaimsIdentity);
    //    context.Principal = context.Principal ?? efClaimsPrincipal;
    //    return long.TryParse(context.Principal.Claims?.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
    //        out var nameIdentifier)
    //        ? nameIdentifier
    //        : -1;
    //}
}