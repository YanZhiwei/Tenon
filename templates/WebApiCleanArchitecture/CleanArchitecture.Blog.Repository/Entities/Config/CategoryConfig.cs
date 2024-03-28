using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tenon.Repository.EfCore;

namespace CleanArchitecture.Blog.Repository.Entities.Config;

public sealed class CategoryConfig : AbstractEntityTypeConfiguration<Category>
{
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        base.Configure(builder);
        builder.HasIndex(x => x.Id);
        builder.Property(p => p.Name).HasMaxLength(NameMaxlength);
        builder.Property(p => p.Alias).HasMaxLength(MaxLength32);
    }
}