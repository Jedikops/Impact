using System.Collections.Generic;
using System.Linq;
using System.Text;
using TendersApi.App.Common;
using TendersApi.App.Interfaces;
using TendersApi.App.Queries;
using TendersApi.Domain;
using TendersAPI.App.Interfaces;

namespace TendersApi.App.Handlers
{
    public class GetTendersQueryHandler
    {
        private ITenderRepository _repository;
        private ICacheService _cacheService;
        private readonly ITenderQueryProcessor _tenderQueryProcessor;
        private const string CacheKey = "filtered_tenders_cache_key_";
        public GetTendersQueryHandler(ITenderRepository repository, ICacheService cacheService, ITenderQueryProcessor tenderQueryProcessor)
        {
            _repository = repository;
            _cacheService = cacheService;
            _tenderQueryProcessor = tenderQueryProcessor;
        }

        public async Task<Result<PaginatedResult<Tender>>> Handle(GetTendersQuery query)
        {
            var tenders = new List<Tender>();
            int repoPage = 1;

            var cacheKey = $"filtered_tenders_cache_key_{query.Page}1_{query.After}2_{query.Before}3_{query.LessThan}4_{query.GreaterThan}5_{(int)query.OrderBy}6_{(int)query.OrderByDirection}7";

            var paginatedResult = await _cacheService.GetAsync<PaginatedResult<Tender>>(cacheKey);

            if (paginatedResult != null)
            {
                return Result<PaginatedResult<Tender>>.Success(paginatedResult);
            }

            if (!IsFullListRequired(query)) 
            {
                return await _repository.GetAsync(query.Page);
            }

            //Since user wants to get a page with filtering and/or ordering we've got to pull everything
            await foreach (var result in _repository.GetAllAsync())
            {
                if (!result.IsSuccess || result.Value == null)
                    return Result<PaginatedResult<Tender>>.Failure(ResultStatus.ExternalApiError, "missing chunks of data");

                var filteredTenders = _tenderQueryProcessor.Filter(result.Value.Items, query.After, query.Before, query.GreaterThan, query.LessThan);

                tenders.AddRange(filteredTenders);
            }

            tenders = _tenderQueryProcessor.Order(tenders, query.OrderBy, query.OrderByDirection).ToList();

            var skippedItems = tenders.Skip((query.Page - 1) * query.PageSize).ToList();

            paginatedResult = new PaginatedResult<Tender>()
            {
                Items = skippedItems,
                Page = query.Page,
                Size = query.PageSize
            };

            await _cacheService.SetAsync(cacheKey, paginatedResult, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(3));

            return Result<PaginatedResult<Tender>>.Success(paginatedResult);
        }



        private bool IsFullListRequired(GetTendersQuery query)
        {
            return query.OrderBy != OrderBy.NotSet || query.After != null || query.Before != null || query.GreaterThan > 0 || query.LessThan < int.MaxValue;
        }
    }
}
