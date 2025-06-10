using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TendersApi.Infrastucture.Converters
{
    internal class DecimalFromStringConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && decimal.TryParse(reader.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
                return value;
            return reader.GetDecimal();
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options) =>
            writer.WriteNumberValue(value);
    }
}
