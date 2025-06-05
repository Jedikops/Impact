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
            if (query.Page * query.PageSize > 100000) {
                return Result<PaginatedResult<Tender>>.Failure(ResultStatus.ValidationError, "Out of bounds");
            }

            return await _repository.GetTendersAsync(query.Page, query.PageSize);
        }
    }
}
