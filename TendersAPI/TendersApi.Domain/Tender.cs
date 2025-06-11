using System.Text.Json.Serialization;

namespace TendersApi.Domain
{
    public class Tender
    {
        public int Id { get; init; }
        public DateTime Date { get; init; }
        public required string Title { get; init; }
        public string? Description { get; init; }
        public decimal Value { get; init; }

        [JsonIgnore]
        public IEnumerable<int>? SupplierIds { get; set; }
    }
}
