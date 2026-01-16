using FCG.Lib.Shared.Application.Common.Errors;
using FCG.Lib.Shared.Application.Common.Models;
using FCG.Api.Catalog.Application.Contracts.Repositories;
using FCG.Api.Catalog.Application.Contracts.Users;
using FCG.Api.Catalog.Domain.Entities;
using FCG.Lib.Shared.Messaging.Contracts;
using MassTransit;
using MediatR;

namespace FCG.Api.Catalog.Application.Commands.Games.PurchaseGame;

public class PurchaseGameCommandHandler : IRequestHandler<PurchaseGameCommand, Result<Guid>>
{
    private readonly IGameRepository _gameRepository;
    private readonly IUserApiService _userApiService;
    private readonly IPromotionRepository _promotionRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public PurchaseGameCommandHandler(
        IGameRepository gameRepository,
        IUserApiService userApiService,
        IPromotionRepository promotionRepository,
        IPublishEndpoint publishEndpoint)
    {
        _gameRepository = gameRepository;
        _userApiService = userApiService;
        _promotionRepository = promotionRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<Guid>> Handle(PurchaseGameCommand request, CancellationToken cancellationToken)
    {
        var user = await _userApiService.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure<Guid>(ApplicationErrors.User.NotFound(request.UserId));

        var game = await _gameRepository.GetByIdAsync(request.GameId, cancellationToken);
        if (game is null)
            return Result.Failure<Guid>(ApplicationErrors.Game.NotFound(request.GameId));

        if (!game.IsActive)
            return Result.Failure<Guid>(ApplicationErrors.UserGame.GameNotActive);

        var alreadyOwned = await _gameRepository.UserOwnsGameAsync(request.UserId, request.GameId, cancellationToken);
        if (alreadyOwned)
            return Result.Failure<Guid>(ApplicationErrors.UserGame.AlreadyOwned);

        var finalPrice = game.Price;
        var promotion = await _promotionRepository.GetByGameIdAsync(request.GameId, cancellationToken);
        
        if (promotion != null && promotion.IsValid())
        {
            finalPrice = promotion.CalculateDiscountedPrice(game.Price);
        }

        var orderId = Guid.NewGuid();

        // Publicar evento para processamento de pagamento
        await _publishEndpoint.Publish(new OrderPlacedEvent
        {
            OrderId = orderId,
            UserId = request.UserId,
            GameId = request.GameId,
            GameTitle = game.Title,
            UserEmail = user.Email,
            Price = finalPrice,
            PlacedAt = DateTime.UtcNow
        }, cancellationToken);

        return Result.Success(orderId);
    }
}
