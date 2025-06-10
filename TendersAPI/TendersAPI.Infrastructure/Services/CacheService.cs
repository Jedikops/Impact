using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TendersAPI.App.Interfaces;

namespace TendersAPI.Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetAsync<T>(string key )
        {
            var cached = await _cache.GetStringAsync(key);

            if (!string.IsNullOrEmpty(cached))
            {
                return JsonSerializer.Deserialize<T>(cached)!;
            }

            return default;
        }

        public async Task SetAsync<T>(string key, T obj, TimeSpan? absoluteExpirationInMinutes = null, TimeSpan? slidingExpiration = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpirationInMinutes,
                SlidingExpiration = slidingExpiration
            };

            await _cache.SetStringAsync(key, JsonSerializer.Serialize(obj), options);
        }
    }
}
