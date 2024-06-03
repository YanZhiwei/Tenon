using Microsoft.EntityFrameworkCore;

namespace Tenon.Repository.EfCore.Extensions;

public static class ServiceCollectionExtension
{
    public static ModelBuilder ApplyConfigurations<TDbContext>(this ModelBuilder modelBuilder)
        where TDbContext : DbContext
    {
        if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));
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