using MediatR;
using FCG.Api.Catalog.Application.Contracts.Repositories;
using FCG.Api.Catalog.Domain.Entities;
using FCG.Lib.Shared.Application.Common.Models;

namespace FCG.Api.Catalog.Application.Queries.Reviews.GetGameReviews;

public class GetGameReviewsQueryHandler(IGameReviewRepository reviewRepository)
    : IRequestHandler<GetGameReviewsQuery, Result<IEnumerable<GameReview>>>
{
    public async Task<Result<IEnumerable<GameReview>>> Handle(GetGameReviewsQuery request, CancellationToken cancellationToken)
    {
        var reviews = await reviewRepository.GetByGameIdAsync(request.GameId, cancellationToken);
        return Result.Success(reviews);
    }
}
