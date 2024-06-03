using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tenon.Repository.EfCore;

public abstract class AbstractEntityTypeConfiguration
{
    public abstract void Configure(ModelBuilder modelBuilder);
}

public abstract class AbstractEntityTypeConfiguration<TEntity> : AbstractEntityTypeConfiguration,
    IEntityTypeConfiguration<TEntity>
    where TEntity : EfEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        var entityType = typeof(TEntity);
        ConfigureKey(builder, entityType);
        ConfigureConcurrency(builder, entityType);
        ConfigureQueryFilter(builder, entityType);
    }
    //public abstract void Configure(EntityTypeBuilder<TEntity> builder);

    public override void Configure(ModelBuilder modelBuilder)
    {
        Configure(modelBuilder.Entity<TEntity>());
    }

    protected void ConfigureQueryFilter(EntityTypeBuilder<TEntity> builder, Type entityType)
    {
        builder.HasKey(x => x.Id);
        //ValueGeneratedNever:指定所选属性的值不应该由数据库自动生成。
        builder.Property(x => x.Id).HasColumnOrder(1).ValueGeneratedNever();
    }

    protected void ConfigureConcurrency(EntityTypeBuilder<TEntity> builder, Type entityType)
    {
        //ValueGeneratedOnAddOrUpdate:方法允许我们指定属性的值在插入或更新记录时自动生成。
        if (typeof(IConcurrency).IsAssignableFrom(entityType))
            builder.Property("RowVersion").IsRequired().IsRowVersion().ValueGeneratedOnAddOrUpdate();
    }

    protected void ConfigureKey(EntityTypeBuilder<TEntity> builder, Type entityType)
    {
        if (typeof(ISoftDelete).IsAssignableFrom(entityType))
        {
            builder.Property("IsDeleted")
                .HasDefaultValue(false)
                .HasColumnOrder(2);
            builder.HasQueryFilter(d => !EF.Property<bool>(d, "IsDeleted"));
        }
    }
}