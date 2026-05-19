using MediatR;
using FCG.Api.Catalog.Domain.Entities;
using FCG.Lib.Shared.Application.Common.Models;

namespace FCG.Api.Catalog.Application.Queries.Reviews.GetGameReviews;

public record GetGameReviewsQuery(Guid GameId) : IRequest<Result<IEnumerable<GameReview>>>;
