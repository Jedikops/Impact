using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading;
using TendersApi.App.Common;
using TendersApi.App.Interfaces;
using TendersApi.Domain;
using TendersAPI.App.Interfaces;

namespace TendersApi.Infrastucture.Repositories
{
    public class CachedTenderRepository : ITenderRepository
    {
        private readonly String _tenderCacheKey = "tender_cache_key_";
        private readonly ICacheService _cacheService;
        private readonly ITenderRepository _innerRepository;
        private readonly int _maxPage = 100;
        private int _concurrencyLimit;
        private SemaphoreSlim _semaphore;

        public CachedTenderRepository(ICacheService cacheService, ITenderRepository innerRepository, int concurrencyLimit)
        {
            _cacheService = cacheService;
            _innerRepository = innerRepository;
            _concurrencyLimit = concurrencyLimit;
            _semaphore = new SemaphoreSlim(_concurrencyLimit);
        }

        public Task<Tender> GetTenderByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<PaginatedResult<Tender>>> GetAsync(int page)
        {
            var paginatedResult = await _cacheService.GetAsync<PaginatedResult<Tender>>($"{_tenderCacheKey}{page}");
            
            if(paginatedResult is not null)
            {
                return Result<PaginatedResult<Tender>>.Success(paginatedResult);
            }

            var result = await _innerRepository.GetAsync(page);

            if (result.IsSuccess && result.Value is not null)
            {
                await _cacheService.SetAsync<PaginatedResult<Tender>>($"{_tenderCacheKey}{page}",
                    result.Value, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(3));
            }

            return result;
        }

        public async IAsyncEnumerable<Result<PaginatedResult<Domain.Tender>>> GetAllAsync()
        {
            var pendingTasks = new List<Task<Result<PaginatedResult<Domain.Tender>>>>();

            for (int page = 1; page <= _maxPage; page++)
            {
                await _semaphore.WaitAsync(); // Use WaitAsync for async/await correctness

                int currentPage = page;

                var task = Task.Run(async () =>
                {
                    try
                    {
                        return await GetAsync(currentPage);
                    }
                    finally
                    {
                        _semaphore.Release();
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
