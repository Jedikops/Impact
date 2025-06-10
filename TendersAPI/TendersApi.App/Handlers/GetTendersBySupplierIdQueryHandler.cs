using TendersApi.App.Common;
using TendersApi.App.Interfaces;
using TendersApi.App.Queries;
using TendersApi.Domain;

namespace TendersApi.App.Handlers
{
    public class GetTendersBySupplierIdQueryHandler
    {
        private ITenderRepository _repository;
        private ITenderQueryProcessor _tenderQueryProcessor;

        public GetTendersBySupplierIdQueryHandler(ITenderRepository repository, ITenderQueryProcessor tenderQueryProcessor)
        {
            _repository = repository;
            _tenderQueryProcessor = tenderQueryProcessor;
        }

        public async Task<Result<IEnumerable<Tender>>> Handle(GetTendersBySupplierIdQuery query)
        {
            List<Tender> tenders = new List<Tender>();

            //can add caching if needed
            await foreach (var tender in _repository.GetAllAsync())
            {
                if (!tender.IsSuccess || tender.Value == null)
                    return Result<IEnumerable<Tender>>.Failure(ResultStatus.ExternalApiError, "missing chunks of data");

                var filteredTenders = _tenderQueryProcessor.FilterBySupplierId(tender.Value.Items, query.Id);


                tenders.AddRange(filteredTenders);
            }

            return Result<IEnumerable<Tender>>.Success(tenders);
            
        }
    }
}
