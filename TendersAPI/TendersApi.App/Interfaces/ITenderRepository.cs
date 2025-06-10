using TendersApi.App.Common;
using TendersApi.Domain;

namespace TendersApi.App.Interfaces
{
    public interface ITenderRepository 
    {
        Task<Result<PaginatedResult<Tender>>> GetAsync(int page);

        IAsyncEnumerable<Result<PaginatedResult<Tender>>> GetAllAsync();

        Task<Result<Tender>> GetByIdAsync(int id);

    }
}
