using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TendersApi.App.Common;
using TendersApi.App.Interfaces;
using TendersApi.Infrastructure.Models;
using TendersApi.Infrastucture.Mapping;

namespace TendersApi.Infrastucture.Repositories
{
    public class TenderRespository : ITenderRepository
    {
        private readonly HttpClient _client;
        private readonly IDistributedCache _cache;
        private ITenderMapper _mapper;

        private readonly int _maxPage = 100;

        public TenderRespository(HttpClient client, IDistributedCache cache, ITenderMapper mapper)
        {
            _client = client;
            _cache = cache;
            _mapper = mapper;
        }

        public Task<TendersApi.Domain.Tender> GetTenderByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<PaginatedResult<Domain.Tender>>> GetAsync(int page)
        {
            if (page > _maxPage)
            {
                return Result<PaginatedResult<Domain.Tender>>.Failure(ResultStatus.ValidationError, "Out of bounds");
            }

            var response = await _client.GetAsync(_client.BaseAddress + $"?page={page}");

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                return ex.StatusCode switch
                {
                    System.Net.HttpStatusCode.BadRequest => Result<PaginatedResult<Domain.Tender>>.Failure(ResultStatus.InternalError, "External Api bad request"),
                    _ => Result<PaginatedResult<Domain.Tender>>.Failure(ResultStatus.ExternalApiError, "External Api bad request"),
                };
            }

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<PagedResult<Tender>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

            var tenders = result.Results.Select(x => _mapper.MapToDomain(x));

            return Result<PaginatedResult<Domain.Tender>>.Success(
                new PaginatedResult<Domain.Tender> { Items = tenders, Page = page, Size = tenders.Count() });
        }

        public async Task<List<Result<PaginatedResult<Domain.Tender>>>> GetAllAsync()
        {
            int concurrencyLimit = 10;
            using var semaphore = new SemaphoreSlim(concurrencyLimit);
            var tasks = new List<Task<Result<PaginatedResult<Domain.Tender>>>>();
            var results = new List<Result<PaginatedResult<Domain.Tender>>>();

            for (int page = 1; page <= _maxPage; page++)
            {
                await semaphore.WaitAsync();
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

                tasks.Add(task);

                if (tasks.Count >= concurrencyLimit)
                {
                    var finishedTask = await Task.WhenAny(tasks);
                    tasks.Remove(finishedTask);
                    results.Add(await finishedTask);
                }
            }

            // Await the remaining tasks
            var remaining = await Task.WhenAll(tasks);
            results.AddRange(remaining);

            return results;
        }
    }
}