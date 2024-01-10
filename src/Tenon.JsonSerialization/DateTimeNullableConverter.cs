using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tenon.JsonSerialization;

public class DateTimeNullableConverter(string format) : JsonConverter<DateTime?>
{
    public DateTimeNullableConverter() : this("yyyy-MM-dd HH:mm:ss")
    {
    }

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var result = default(DateTime?);
        if (DateTime.TryParse(reader.GetString(), out var datetime)) result = datetime;

        return result;
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToString(format));
    }
}