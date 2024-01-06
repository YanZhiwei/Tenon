using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tenon.Repository.EfCore;

public class EfAuditEntryConfiguration : IEntityTypeConfiguration<EfAuditEntry>
{
    public void Configure(EntityTypeBuilder<EfAuditEntry> builder)
    {
        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();
        builder.Ignore(c => c.TempProperties);
        builder.Property(ae => ae.Changes).HasConversion(
            value => JsonSerializer.Serialize(value, options),
            serializedValue => JsonSerializer.Deserialize<Dictionary<string, object?>>(serializedValue, options) ?? new Dictionary<string, object?>());
    }
}