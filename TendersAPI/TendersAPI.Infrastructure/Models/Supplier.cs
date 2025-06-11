using System.Text.Json.Serialization;

namespace TendersApi.Infrastructure.Models
{
    public class Supplier
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

    }
}
