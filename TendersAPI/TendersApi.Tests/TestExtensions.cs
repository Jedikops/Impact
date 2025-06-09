using TendersApi.App.Common;
using TendersApi.Domain;

namespace TendersApi.Tests.Extensions
{
    public static class TestExtensions
    {
        public static async IAsyncEnumerable<Result<PaginatedResult<Tender>>> GetFakePaginatedResultAsync(this List<Result<PaginatedResult<Tender>>> resultsAsync)
        {
            foreach (var page in resultsAsync)
            {
                // Optionally simulate async behavior
                await Task.Yield();
                yield return page;
            }
        }
    }
}
