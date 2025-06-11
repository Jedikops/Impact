using System.Text.Json;
using System.Text.Json.Serialization;

namespace TendersAPI.Infrastructure.Converters
{
    internal class IntCollectionFromStringConverter : JsonConverter<IEnumerable<int>>
    {
        public override IEnumerable<int> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string stringValue = reader.GetString();
                if (string.IsNullOrEmpty(stringValue))
                {
                    return Array.Empty<int>();
                }

                try
                {
                    // Split the string (e.g., "1,2,3") and parse each part to an int
                    return stringValue.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => int.Parse(s.Trim()))
                        .ToList();
                }
                catch (FormatException ex)
                {
                    throw new JsonException($"Failed to parse '{stringValue}' as a collection of integers.", ex);
                }
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                // Handle JSON array of integers, e.g., [1, 2, 3]
                List<int> result = new List<int>();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    if (reader.TokenType == JsonTokenType.Number)
                    {
                        result.Add(reader.GetInt32());
                    }
                    else
                    {
                        throw new JsonException($"Expected a number in the array, but got {reader.TokenType}.");
                    }
                }
                return result;
            }

            throw new JsonException($"Expected a string or array, but got {reader.TokenType}.");
        }

        public override void Write(Utf8JsonWriter writer, IEnumerable<int> value, JsonSerializerOptions options)
        {
            // Serialize as a JSON array, e.g., [1, 2, 3]
            writer.WriteStartArray();
            foreach (int item in value)
            {
                writer.WriteNumberValue(item);
            }
            writer.WriteEndArray();
        }
    }
}
