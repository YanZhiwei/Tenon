using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tenon.Repository.EfCore;

namespace CleanArchitecture.Blog.Repository.Entities.Config;

public sealed class FriendLinkConfig : AbstractEntityTypeConfiguration<FriendLink>
{
    public override void Configure(EntityTypeBuilder<FriendLink> builder)
    {
        base.Configure(builder);
        builder.HasIndex(x => x.Id);
        builder.Property(p => p.Name).HasMaxLength(NameMaxlength);
        builder.Property(p => p.Url).HasMaxLength(MaxLength128);
    }
}