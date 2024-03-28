using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tenon.Repository.EfCore;

namespace CleanArchitecture.Blog.Repository.Entities.Config;

public sealed class PostConfig : AbstractEntityTypeConfiguration<Post>
{
    public override void Configure(EntityTypeBuilder<Post> builder)
    {
        base.Configure(builder);
        builder.HasIndex(x => x.Id);
        builder.Property(p => p.Title).HasMaxLength(MaxLength64);
        builder.Property(p => p.Author).HasMaxLength(NameMaxlength);
    }
}