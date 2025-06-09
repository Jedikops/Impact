using System.Collections.Generic;
using System.Linq;
using System.Text;
using TendersApi.App.Common;
using TendersApi.App.Interfaces;
using TendersApi.App.Queries;
using TendersApi.Domain;

namespace TendersApi.App.Handlers
{
    public class GetTendersQueryHandler
    {
        private ITenderRepository _repository;

        public GetTendersQueryHandler(ITenderRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<PaginatedResult<Tender>>> Handle(GetTendersQuery query)
        {
            var tenders = new List<Tender>();
            int repoPage = 1;

            if (!IsFullListRequired(query)) //Since user wants to get a page with filtering and/or ordering we've got to pull everything
            {
                return await _repository.GetAsync(query.Page);
            }


            await foreach(var result in _repository.GetAllAsync()) {
                if (!result.IsSuccess || result.Value == null)
                    return Result<PaginatedResult<Tender>>.Failure(ResultStatus.ExternalApiError, "missing chunks of data");

                tenders.AddRange(result.Value.Items);
            }

            if (query.OrderBy != OrderBy.NotSet)
            {
                tenders = query switch
                {
                    { OrderBy: OrderBy.Date, OrderByDirection: OrderByDirection.Descending } => tenders.OrderByDescending(t => t.Date).ToList(),
                    { OrderBy: OrderBy.Date } => tenders.OrderBy(t => t.Date).ToList(),
                    { OrderBy: OrderBy.Value, OrderByDirection: OrderByDirection.Descending } => tenders.OrderByDescending(t => t.Value).ToList(),
                    { OrderBy: OrderBy.Value } => tenders.OrderBy(t => t.Value).ToList(),
                    _ => tenders
                };
            }

            if (query.After != null || query.Before != null)
            {
                tenders = tenders
                        .Where(x =>
                            (!query.After.HasValue || x.Date >= query.After.Value) &&
                            (!query.Before.HasValue || x.Date < query.Before.Value) &&
                            (query.GreaterThan >= 0 && x.Value > query.GreaterThan) && 
                            (query.LessThan >= 0 && x.Value < query.LessThan))
                        .ToList();
            }

            var skippedItems = tenders.Skip((query.Page - 1) * query.PageSize).ToList();

            return Result<PaginatedResult<Tender>>.Success(new PaginatedResult<Tender>()
            {
                Items = skippedItems,
                Page = query.Page,
                Size = query.PageSize
            });
        }

        private bool IsFullListRequired(GetTendersQuery query)
        {
            return query.OrderBy != OrderBy.NotSet || query.After != null || query.Before != null || query.GreaterThan >= 0 || query.LessThan >= 0;
        }
    }
}
