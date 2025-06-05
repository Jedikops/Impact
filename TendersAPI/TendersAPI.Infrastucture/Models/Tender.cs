using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TendersAPI.Infrastucture.Converters;

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
        public required string Description { get; init; }
    }
}
