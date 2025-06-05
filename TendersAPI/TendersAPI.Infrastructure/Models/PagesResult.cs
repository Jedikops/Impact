using System.Text.Json.Serialization;

namespace TendersApi.Infrastructure.Models
{
    public class PagedResult<T>
    {
        [JsonPropertyName("page_count")]
        public int PagesCount { get; set; }

        [JsonPropertyName("page_size")]
        public int AvaialblePages { get; set; }

        [JsonPropertyName("page_number")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("total")]
        public int TotalTendersCount { get; set; }

        [JsonPropertyName("data")]
        public required IEnumerable<T> Results { get; set; }

    }
}

