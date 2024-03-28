using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tenon.Repository.EfCore.SqliteTests.Entities.Config;

public sealed class PostConfig : AbstractEntityTypeConfiguration<Post>
{
    public override void Configure(EntityTypeBuilder<Post> builder)
    {
        base.Configure(builder);
        builder.HasIndex(x => x.Id);
        builder.Property(p => p.Title).HasMaxLength(MaxLength64);
        builder.Property(p => p.Content).HasMaxLength(MaxLength256);
    }
}