using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading;
using TendersApi.App.Common;
using TendersApi.App.Interfaces;
using TendersApi.Domain;

namespace TendersApi.Infrastucture.Repositories
{
    public class CachedTenderRepository : ITenderRepository
    {
        private readonly String _tenderCacheKey = "tender_cache_key_to_page_";
        private readonly IDistributedCache _cache;
        private readonly ITenderRepository _innerRepository;
        private readonly int _maxPage = 100;
        private const int _concurrencyLimit = 10;
        private SemaphoreSlim semaphore = new SemaphoreSlim(_concurrencyLimit);

        public CachedTenderRepository(IDistributedCache cache, ITenderRepository innerRepository)
        {
            _cache = cache;
            _innerRepository = innerRepository;
        }
        public Task<Tender> GetTenderByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<PaginatedResult<Tender>>> GetAsync(int page)
        {
            var cached = await _cache.GetStringAsync($"{_tenderCacheKey}{page}");

            if (!string.IsNullOrEmpty(cached))
            {
                var paginatedResult = JsonSerializer.Deserialize<PaginatedResult<Tender>>(cached)!;
                return Result<PaginatedResult<Tender>>.Success(paginatedResult);

            }

            var result = await _innerRepository.GetAsync(page);

            if (result.IsSuccess)
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };

                await _cache.SetStringAsync($"{_tenderCacheKey}{page}", JsonSerializer.Serialize(result.Value), options);
            }

            return result;
        }

        public async IAsyncEnumerable<Result<PaginatedResult<Domain.Tender>>> GetAllAsync()
        {
            var pendingTasks = new List<Task<Result<PaginatedResult<Domain.Tender>>>>();

            for (int page = 1; page <= _maxPage; page++)
            {
                await semaphore.WaitAsync(); // Use WaitAsync for async/await correctness

                int currentPage = page;

                var task = Task.Run(async () =>
                {
                    try
                    {
                        return await GetAsync(currentPage);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                pendingTasks.Add(task);

                if (pendingTasks.Count >= _concurrencyLimit)
                {
                    var completedTask = await Task.WhenAny(pendingTasks);
                    pendingTasks.Remove(completedTask);
                    yield return await completedTask;
                }
            }

            foreach (var remainingTask in pendingTasks)
            {
                yield return await remainingTask;
            }
        }

    }
}
