namespace TendersAPI.App.Interfaces
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);

        Task SetAsync<T>(string key,T obj, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null);
    }
}
