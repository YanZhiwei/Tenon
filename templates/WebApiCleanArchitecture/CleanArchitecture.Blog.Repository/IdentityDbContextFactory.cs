using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CleanArchitecture.Blog.Repository;

public sealed class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        //add-migration init_IdentityDbContext -Context IdentityDbContext
        //update-database -Context IdentityDbContext
        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
        optionsBuilder.UseSqlite("DataSource=identity.db; Cache=Shared",
            b => b.MigrationsAssembly("CleanArchitecture.Blog.Repository"));
        return new IdentityDbContext(optionsBuilder.Options);
    }
}