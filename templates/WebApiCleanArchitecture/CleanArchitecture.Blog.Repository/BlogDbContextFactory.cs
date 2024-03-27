using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CleanArchitecture.Blog.Repository;

public sealed class BlogDbContextFactory : IDesignTimeDbContextFactory<BlogDbContext>
{
    public BlogDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BlogDbContext>();
        optionsBuilder.UseSqlite("DataSource=blog.db; Cache=Shared",
            b => b.MigrationsAssembly("CleanArchitecture.Blog.Repository"));
        return new BlogDbContext(optionsBuilder.Options);
    }
}