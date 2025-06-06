using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
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
            int concurrencyLimit = 10;
            using var semaphore = new SemaphoreSlim(concurrencyLimit);
            var tasks = new List<Task<Result<PaginatedResult<Domain.Tender>>>>();

            for (int page = 1; page <= _maxPage; page++)
            {
                await semaphore.WaitAsync();
                int currentPage = page;

                var task = Task.Run(async () =>
                {
                    try
                    {
                        string cacheKey = $"{_tenderCacheKey}{currentPage}";
                        var cached = await _cache.GetStringAsync(cacheKey);

                        if (!string.IsNullOrEmpty(cached))
                        {
                            var cachedResult = JsonSerializer.Deserialize<PaginatedResult<Domain.Tender>>(cached)!;
                            return Result<PaginatedResult<Domain.Tender>>.Success(cachedResult);
                        }

                        var result = await _innerRepository.GetAsync(currentPage);

                        if (result.IsSuccess && result.Value != null)
                        {
                            var options = new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                            };

                            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result.Value), options);
                        }

                        return result;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                tasks.Add(task);

                if (tasks.Count >= concurrencyLimit)
                {
                    var finished = await Task.WhenAny(tasks);
                    tasks.Remove(finished);
                    yield return await finished;
                }
            }

            foreach (var task in tasks)
            {
                yield return await task;
            }
        }
    }
}
