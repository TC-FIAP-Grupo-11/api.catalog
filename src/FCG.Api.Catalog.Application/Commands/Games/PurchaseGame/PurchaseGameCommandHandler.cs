using FCG.Lib.Shared.Application.Common.Errors;
using FCG.Lib.Shared.Application.Common.Models;
using FCG.Api.Catalog.Application.Contracts.Repositories;
using FCG.Api.Catalog.Application.Contracts.Users;
using FCG.Lib.Shared.Messaging.Contracts;
using MassTransit;
using MediatR;
using FCG.Api.Catalog.Domain.Entities;

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
        var user = await _userApiService.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
            return Result.Failure<Guid>(ApplicationErrors.User.NotFound(request.Email));

        var game = await _gameRepository.GetByIdAsync(request.GameId, cancellationToken);
        if (game is null)
            return Result.Failure<Guid>(ApplicationErrors.Game.NotFound(request.GameId));

        if (!game.IsActive)
            return Result.Failure<Guid>(ApplicationErrors.UserGame.GameNotActive);

        var alreadyOwned = await _gameRepository.UserOwnsGameAsync(user.Id, request.GameId, cancellationToken);
        if (alreadyOwned)
            return Result.Failure<Guid>(ApplicationErrors.UserGame.AlreadyOwned);

        var finalPrice = game.Price;
        var activePromotions = await _promotionRepository.GetActivePromotionsByGameIdAsync(request.GameId, cancellationToken);
        
        if (activePromotions.Any())
        {
            finalPrice = Promotion.CalculateAccumulatedDiscount(activePromotions, game.Price);
        }

        var orderId = Guid.NewGuid();

        // Criar UserGame com status pendente
        var userGame = UserGame.Create(user.Id, request.GameId, orderId, finalPrice);
        await _gameRepository.AddUserGameAsync(userGame, cancellationToken);
        await _gameRepository.SaveChangesAsync(cancellationToken);

        // Publicar evento para processamento de pagamento
        await _publishEndpoint.Publish(new OrderPlacedEvent
        {
            OrderId = orderId,
            UserId = user.Id,
            GameId = request.GameId,
            GameTitle = game.Title,
            UserEmail = user.Email,
            Price = finalPrice,
            PlacedAt = DateTime.UtcNow
        }, cancellationToken);

        return Result.Success(orderId);
    }
}
