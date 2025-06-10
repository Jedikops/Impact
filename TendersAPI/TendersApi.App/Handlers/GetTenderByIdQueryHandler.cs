using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TendersApi.App.Common;
using TendersApi.App.Interfaces;
using TendersApi.App.Queries;
using TendersApi.Domain;

namespace TendersApi.App.Handlers
{
    public class GetTenderByIdQueryHandler
    {
        private readonly ITenderRepository _repository;

        public GetTenderByIdQueryHandler(ITenderRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<Tender>> Handle(GetTenderByIdQuery query)
        {
            //can add caching here if needed
            return await _repository.GetByIdAsync(query.Id);

        }
    }
}
