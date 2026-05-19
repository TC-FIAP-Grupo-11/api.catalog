using FCG.Api.Catalog.Domain.Entities;

namespace FCG.Api.Catalog.Application.Contracts.Repositories;

public interface IGameReviewRepository
{
    Task AddAsync(GameReview review, CancellationToken cancellationToken = default);
    Task<IEnumerable<GameReview>> GetByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);
}
