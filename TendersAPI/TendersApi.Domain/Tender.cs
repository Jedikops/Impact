using System.Text.Json.Serialization;

namespace TendersApi.Domain
{
    public class Tender
    {
        public Tender()
        {
            SupplierIds = new List<int>();
        }

        public int Id { get; init; }
        public DateTime Date { get; init; }
        public required string Title { get; init; }
        public string? Description { get; init; }
        public decimal Value { get; init; }

        public required List<int> SupplierIds { get; init; } //TODO: Expand Suppliers to SupplierType - Sprint's over :/
    }
}
