using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tenon.Repository.EfCore;

namespace CleanArchitecture.Blog.Repository.Entities.Config;

public sealed class TagConfig : AbstractEntityTypeConfiguration<Tag>
{
    public override void Configure(EntityTypeBuilder<Tag> builder)
    {
        base.Configure(builder);
        builder.Property(p => p.Name).HasMaxLength(NameMaxlength);
        builder.Property(p => p.Alias).HasMaxLength(MaxLength32);
    }

    public override void Configure(ModelBuilder modelBuilder)
    {
        throw new NotImplementedException();
    }
}