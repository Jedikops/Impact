using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TendersApi.Infrastucture.Converters;

namespace TendersApi.Infrastructure.Models
{
    public class Tender
    {
        [JsonPropertyName("id")]
        [JsonConverter(typeof(IntFromStringConverter))]
        public int Id { get; init; }

        [JsonPropertyName("date")]
        public DateTime Date { get; init; }

        [JsonPropertyName("title")]
        public required string Title { get; init; }

        [JsonPropertyName("description")] 
        public string? Description { get; init; }

        [JsonPropertyName("awarded_value_eur")]
        [JsonConverter(typeof(DecimalFromStringConverter))]
        public required decimal Value { get; init; }

        [JsonPropertyName("awarded")]
        public List<Awarded>? AwardedData { get; init; }
    }
}
