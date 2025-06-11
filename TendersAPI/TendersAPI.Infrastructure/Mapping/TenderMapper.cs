using TendersApi.Infrastructure.Models;

namespace TendersApi.Infrastucture.Mapping
{
    public class TenderMapper : ITenderMapper
    {
        public TendersApi.Domain.Tender MapToDomain(TendersApi.Infrastructure.Models.Tender dto) => new TendersApi.Domain.Tender
        {
            Id = dto.Id,
            Title = dto.Title,
            Date = dto.Date,
            Description = dto.Description!, 
            Value = dto.Value,
            Suppliers = dto.AwardedData?.SelectMany(a => a.Suppliers ).Select(x => new Domain.Supplier(x.Id, x.Name)).ToList()!
        };
    }
}
