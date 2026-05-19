using MediatR;
using FCG.Api.Catalog.Application.Contracts.Repositories;
using FCG.Api.Catalog.Domain.Entities;
using FCG.Lib.Shared.Application.Common.Models;
using FCG.Lib.Shared.Application.Common.Errors;

namespace FCG.Api.Catalog.Application.Commands.Reviews.CreateGameReview;

public class CreateGameReviewCommandHandler(
    IGameReviewRepository reviewRepository,
    IGameRepository gameRepository)
    : IRequestHandler<CreateGameReviewCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateGameReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var game = await gameRepository.GetByIdAsync(request.GameId, cancellationToken);
            if (game is null)
                return Result.Failure<Guid>(Error.NotFound("Review.GameNotFound", $"Game '{request.GameId}' not found."));

            var review = GameReview.Create(request.GameId, request.UserEmail, request.Rating, request.Comment);
            await reviewRepository.AddAsync(review, cancellationToken);

            return Result.Success(review.Id);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<Guid>(Error.Validation("Review.ValidationError", ex.Message));
        }
        catch (Exception)
        {
            return Result.Failure<Guid>(Error.Failure("Review.CreationFailed", "Failed to create the review."));
        }
    }
}
