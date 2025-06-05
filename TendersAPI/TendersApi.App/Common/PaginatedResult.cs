namespace TendersApi.App.Common
{
    public class PaginatedResult<T>
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public required IEnumerable<T> Items { get; set; }
    }

}
