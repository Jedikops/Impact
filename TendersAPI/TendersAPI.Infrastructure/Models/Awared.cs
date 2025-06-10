using System.Text.Json.Serialization;

namespace TendersApi.Infrastructure.Models
{
    public class Awarded
    {
        [JsonPropertyName("suppliers_id")]
        public int SupplierId { get; set; }
    }
}
