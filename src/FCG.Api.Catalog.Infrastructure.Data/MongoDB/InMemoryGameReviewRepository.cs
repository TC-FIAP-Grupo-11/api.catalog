using FCG.Api.Catalog.Application.Contracts.Repositories;
using FCG.Api.Catalog.Domain.Entities;

namespace FCG.Api.Catalog.Infrastructure.Data.MongoDB;

// Fallback used when MongoDB is not configured (local dev without MongoDB)
public class InMemoryGameReviewRepository : IGameReviewRepository
{
    private readonly List<GameReview> _reviews = [];

    public Task AddAsync(GameReview review, CancellationToken cancellationToken = default)
    {
        _reviews.Add(review);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<GameReview>> GetByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var result = _reviews.Where(r => r.GameId == gameId).AsEnumerable();
        return Task.FromResult(result);
    }
}
