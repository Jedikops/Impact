using TendersApi.App.Common;
using TendersApi.Domain;

namespace TendersApi.App.Interfaces
{
    public interface ITenderRepository 
    {
        Task<Result<PaginatedResult<Tender>>> GetAsync(int page);

        IAsyncEnumerable<Result<PaginatedResult<Domain.Tender>>> GetAllAsync();

        Task<Tender> GetTenderByIdAsync(int id);

    }
}
