using System.Text.Json.Serialization;
using TendersApi.Infrastucture.Converters;
using TendersAPI.Infrastructure.Converters;

namespace TendersApi.Infrastructure.Models
{
    public class Awarded
    {
        [JsonPropertyName("suppliers")]
        public List<Supplier> Suppliers { get; set; }

    }
}
