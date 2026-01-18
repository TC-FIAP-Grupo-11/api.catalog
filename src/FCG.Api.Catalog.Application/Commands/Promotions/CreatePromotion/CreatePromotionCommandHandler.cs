using MediatR;
using FCG.Api.Catalog.Application.Contracts.Repositories;
using FCG.Api.Catalog.Domain.Entities;
using FCG.Lib.Shared.Application.Common.Models;
using FCG.Lib.Shared.Application.Common.Errors;

namespace FCG.Api.Catalog.Application.Commands.Promotions.CreatePromotion;

public class CreatePromotionCommandHandler(
    IPromotionRepository promotionRepository,
    IGameRepository gameRepository) : IRequestHandler<CreatePromotionCommand, Result<Guid>>
{
    private readonly IPromotionRepository _promotionRepository = promotionRepository;
    private readonly IGameRepository _gameRepository = gameRepository;

    public async Task<Result<Guid>> Handle(CreatePromotionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var game = await _gameRepository.GetByIdAsync(request.GameId, cancellationToken);
            if (game is null)
                return Result.Failure<Guid>(ApplicationErrors.Game.NotFound(request.GameId));

            var promotion = Promotion.Create(
                request.GameId,
                request.DiscountPercentage,
                request.StartDate,
                request.EndDate
            );

            await _promotionRepository.AddAsync(promotion, cancellationToken);

            return Result.Success(promotion.Id);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<Guid>(Error.Validation("CreatePromotion.ValidationError", ex.Message));
        }
        catch (Exception)
        {
            return Result.Failure<Guid>(ApplicationErrors.Promotion.CreationFailed);
        }
    }
}
