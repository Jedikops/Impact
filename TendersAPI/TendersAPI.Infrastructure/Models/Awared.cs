using System.Text.Json.Serialization;
using TendersApi.Infrastucture.Converters;
using TendersAPI.Infrastructure.Converters;

namespace TendersApi.Infrastructure.Models
{
    public class Awarded
    {
        [JsonPropertyName("suppliers_id")]
        [JsonConverter(typeof(IntCollectionFromStringConverter))]
        public required IEnumerable<int> SupplierIds { get; set; }
    }
}
