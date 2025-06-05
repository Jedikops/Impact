namespace TendersApi.Infrastucture.Mapping
{
    public interface ITenderMapper
    {
        TendersApi.Domain.Tender MapToDomain(TendersApi.Infrastructure.Models.Tender dto);
    }
}