using System.Text.Json;
using System.Text.Json.Serialization;

namespace PixivSync.Pixiv.ApiResponse;

public class IdConverter : JsonConverter<long>
{
    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        return reader.TokenType switch
        {
            JsonTokenType.Null => throw new InvalidDataException(),
            JsonTokenType.Number => reader.GetInt64(),
            JsonTokenType.String => long.Parse(reader.GetString()!),
            _ => throw new InvalidDataException()
        };
    }

    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}