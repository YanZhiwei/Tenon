using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
namespace Tenon.Repository.EfCore.MySql
{
    public class MysqlDbContext : AuditDbContext
    {
        public MysqlDbContext(DbContextOptions options, ClaimsPrincipal claimsPrincipal) : base(options, claimsPrincipal)
        {

        }

        public MysqlDbContext(DbContextOptions options) : base(options)
        {

        }

       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8mb4 ");
            base.OnModelCreating(modelBuilder);
        }
    }
}
