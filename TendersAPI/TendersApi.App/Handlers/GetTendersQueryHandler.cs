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
            var mathingTenders = new List<Tender>();
            int repoPage = 1;



            while (mathingTenders.Count < query.Page * query.PageSize)
            {
                var tendersResult = await _repository.GetTendersAsync(repoPage);

                if (!tendersResult.IsSuccess)
                {
                    return tendersResult;
                }

                if (tendersResult.Value != null)
                {
                    var filtered = tendersResult.Value.Items
                        .Where(x =>
                            (!query.After.HasValue || x.Date >= query.After.Value) &&
                            (!query.Before.HasValue || x.Date < query.Before.Value))
                        .ToList();

                    mathingTenders.AddRange(filtered);

                    if (tendersResult.Value.Items.Count() < query.PageSize) break; //Not enough elements in repo matching condition

                    repoPage++;
                    continue;
                }

                return Result<PaginatedResult<Tender>>.Failure(ResultStatus.ExternalApiError, "Unknown error");

            }

            var skippedItems = mathingTenders.Skip((query.Page - 1) * query.PageSize).ToList();
            return Result<PaginatedResult<Tender>>.Success(new PaginatedResult<Tender>()
            {
                Items = skippedItems,
                Page = query.Page,
                Size = query.PageSize
            });
        }
    }
}
