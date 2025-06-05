using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TendersApi.App.Common;
using TendersApi.App.Interfaces;
using TendersApi.Infrastructure.Models;
using TendersAPI.Infrastucture.Mapping;

namespace TendersAPI.Infrastucture
{
    public class TenderRespository : ITenderRepository
    {
        private readonly HttpClient _client;
        private readonly IDistributedCache _cache;
        private ITenderMapper _mapper;

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

        public async Task<Result<PaginatedResult<TendersApi.Domain.Tender>>> GetTendersAsync(int page, int pageSize)
        {

            var response = await _client.GetAsync(_client.BaseAddress + $"?page={page}");

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                return ex.StatusCode switch
                {
                    System.Net.HttpStatusCode.BadRequest => Result<PaginatedResult<TendersApi.Domain.Tender>>.Failure(ResultStatus.InternalError, "External Api bad request"),
                    _ => Result<PaginatedResult<TendersApi.Domain.Tender>>.Failure(ResultStatus.ExternalApiError, "External Api bad request"),
                };
            }

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<PagedResult<Tender>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

            var tenders = result.Results.Select(x => _mapper.MapToDomain(x));

            return Result<PaginatedResult<TendersApi.Domain.Tender>>.Success(
                new PaginatedResult<TendersApi.Domain.Tender> { Items = tenders, Page = page, Size = tenders.Count() });
        }
    }
}
