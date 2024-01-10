using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tenon.JsonSerialization;

public class DateTimeConverter(string format) : JsonConverter<DateTime>
{
    public DateTimeConverter() : this("yyyy-MM-dd HH:mm:ss")
    {
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();
        if (dateString is null)
            throw new ArgumentException(nameof(reader));
        return DateTime.Parse(dateString);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(format));
    }
}