using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tenon.Repository.EfCore.MySqlTests.Entities.Config;

public class BlogConfig : AbstractEntityTypeConfiguration<Blog>
{
    public override void Configure(EntityTypeBuilder<Blog> modelBuilder)
    {
        base.Configure(modelBuilder);
        modelBuilder.HasIndex(x => x.Id);
        modelBuilder.Property(p => p.Url).HasMaxLength(64);
        modelBuilder.Property(p => p.RowVersion).IsConcurrencyToken();
    }
}