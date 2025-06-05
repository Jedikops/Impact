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

        public CachedTenderRepository(IDistributedCache cache, ITenderRepository innerRepository)
        {
            _cache = cache;
            _innerRepository = innerRepository;
        }
        public Task<Tender> GetTenderByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<PaginatedResult<Tender>>> GetTendersAsync(int page)
        {
            var cached = await _cache.GetStringAsync($"{_tenderCacheKey}{page}");

            if (!string.IsNullOrEmpty(cached))
            {
                var paginatedResult = JsonSerializer.Deserialize<PaginatedResult<Tender>>(cached)!;
                return Result<PaginatedResult<Tender>>.Success(paginatedResult);

            }

            var result = await _innerRepository.GetTendersAsync(page);

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
    }
}
