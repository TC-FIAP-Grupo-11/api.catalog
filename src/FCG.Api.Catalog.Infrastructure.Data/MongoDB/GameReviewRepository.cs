using MongoDB.Driver;
using FCG.Api.Catalog.Application.Contracts.Repositories;
using FCG.Api.Catalog.Domain.Entities;

namespace FCG.Api.Catalog.Infrastructure.Data.MongoDB;

public class GameReviewRepository(MongoDbContext context) : IGameReviewRepository
{
    private readonly IMongoCollection<GameReviewDocument> _collection =
        context.GetCollection<GameReviewDocument>("game_reviews");

    public async Task AddAsync(GameReview review, CancellationToken cancellationToken = default)
    {
        var document = new GameReviewDocument
        {
            Id = review.Id,
            GameId = review.GameId,
            UserEmail = review.UserEmail,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };

        await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<GameReview>> GetByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<GameReviewDocument>.Filter.Eq(d => d.GameId, gameId);
        var sort = Builders<GameReviewDocument>.Sort.Descending(d => d.CreatedAt);

        var documents = await _collection
            .Find(filter)
            .Sort(sort)
            .ToListAsync(cancellationToken);

        return documents.Select(d => GameReview.Reconstitute(d.Id, d.GameId, d.UserEmail, d.Rating, d.Comment, d.CreatedAt));
    }
}
