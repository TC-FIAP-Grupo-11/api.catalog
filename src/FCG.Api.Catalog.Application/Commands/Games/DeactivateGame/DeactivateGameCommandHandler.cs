using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using FCG.Api.Catalog.Application.Cache;
using FCG.Api.Catalog.Application.Contracts.Repositories;
using FCG.Api.Catalog.Application.Contracts.Services;
using FCG.Lib.Shared.Application.Common.Models;
using FCG.Lib.Shared.Application.Common.Errors;

namespace FCG.Api.Catalog.Application.Commands.Games.DeactivateGame;

public class DeactivateGameCommandHandler(IGameRepository gameRepository, IDistributedCache cache, IGameSearchService searchService)
    : IRequestHandler<DeactivateGameCommand, Result>
{
    public async Task<Result> Handle(DeactivateGameCommand request, CancellationToken cancellationToken)
    {
        var game = await gameRepository.GetByIdAsync(request.Id, cancellationToken);

        if (game is null)
            return Result.Failure(ApplicationErrors.Game.NotFound(request.Id));

        game.Deactivate();

        await gameRepository.UpdateAsync(game, cancellationToken);

        foreach (var key in CacheKeys.GamesInvalidationKeys())
            await cache.RemoveAsync(key, cancellationToken);

        await searchService.UpdateIndexAsync(game, cancellationToken);

        return Result.Success();
    }
}
