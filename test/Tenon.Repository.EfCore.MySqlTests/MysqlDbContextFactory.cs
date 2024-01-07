using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Tenon.Repository.EfCore.MySql;
using Tenon.Repository.EfCore.MySql.Configurations;

namespace Tenon.Repository.EfCore.MySqlTests;

public class MysqlDbContextFactory : IDesignTimeDbContextFactory<MySqlTestDbContext>
{
    public MySqlTestDbContext CreateDbContext(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");


        var configuration = builder.Build();
        var mysqlSection = configuration.GetSection("Mysql");
        var mysqlConfig = mysqlSection.Get<MySqlOptions>();
        var serverVersion = new MariaDbServerVersion(new Version(8, 2, 0));
        var options = new DbContextOptionsBuilder<MysqlDbContext>()
            .UseMySql(mysqlConfig.ConnectionString, serverVersion)
            .Options;
        return new MySqlTestDbContext(options);
    }
}