using TendersApi.App.Common;
using TendersApi.Domain;

namespace TendersApi.App.Interfaces
{
    public interface ITenderRepository
    {
        Task<Result<PaginatedResult<Tender>>> GetTendersAsync(int page, int pageSize);
        Task<Tender> GetTenderByIdAsync(int id);

    }
}
