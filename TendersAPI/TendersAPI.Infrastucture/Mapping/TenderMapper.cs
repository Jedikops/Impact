namespace TendersAPI.Infrastucture.Mapping
{
    public class TenderMapper : ITenderMapper
    {
        public TendersApi.Domain.Tender MapToDomain(TendersApi.Infrastructure.Models.Tender dto) => new TendersApi.Domain.Tender
        {
            Id = dto.Id,
            Title = dto.Title,
            Date = dto.Date,
            Description = dto.Description
        };
    }
}
