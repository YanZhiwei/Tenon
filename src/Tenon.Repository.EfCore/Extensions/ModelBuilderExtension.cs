using Microsoft.EntityFrameworkCore;

namespace Tenon.Repository.EfCore.Extensions;

public static class ModelBuilderExtension
{
    public static ModelBuilder ApplyConfigurations<TDbContext>(this ModelBuilder modelBuilder)
        where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        foreach (var type in typeof(TDbContext).Assembly.DefinedTypes.Where(t =>
                     t is { IsAbstract: false, IsGenericTypeDefinition: false }
                     && typeof(AbstractEntityTypeConfiguration).IsAssignableFrom(t)))
        {
            dynamic entityConfiguration = Activator.CreateInstance(type)!;
            modelBuilder.ApplyConfiguration(entityConfiguration);
        }

        return modelBuilder;
    }
}