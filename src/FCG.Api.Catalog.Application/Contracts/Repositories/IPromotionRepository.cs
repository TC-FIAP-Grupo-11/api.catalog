using FCG.Api.Catalog.Domain.Entities;
using FCG.Lib.Shared.Application.Common.Models;
using FCG.Lib.Shared.Application.Contracts.Repositories;

namespace FCG.Api.Catalog.Application.Contracts.Repositories;

public interface IPromotionRepository : IBaseRepository<Promotion>
{
    Task<List<Promotion>> GetActivePromotionsByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<PagedResult<Promotion>> GetAllPagedAsync(int pageNumber, int pageSize, bool includeInactive, CancellationToken cancellationToken = default);
    Task<PagedResult<Promotion>> GetActivePromotionsPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<bool> GameHasActivePromotionAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}
