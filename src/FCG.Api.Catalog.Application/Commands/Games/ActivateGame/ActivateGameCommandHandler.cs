using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using FCG.Api.Catalog.Application.Cache;
using FCG.Api.Catalog.Application.Contracts.Repositories;
using FCG.Api.Catalog.Application.Contracts.Services;
using FCG.Lib.Shared.Application.Common.Models;
using FCG.Lib.Shared.Application.Common.Errors;

namespace FCG.Api.Catalog.Application.Commands.Games.ActivateGame;

public class ActivateGameCommandHandler(IGameRepository gameRepository, IDistributedCache cache, IGameSearchService searchService)
    : IRequestHandler<ActivateGameCommand, Result>
{
    public async Task<Result> Handle(ActivateGameCommand request, CancellationToken cancellationToken)
    {
        var game = await gameRepository.GetByIdAsync(request.Id, cancellationToken);

        if (game is null)
            return Result.Failure(ApplicationErrors.Game.NotFound(request.Id));

        game.Activate();

        await gameRepository.UpdateAsync(game, cancellationToken);

        foreach (var key in CacheKeys.GamesInvalidationKeys())
            await cache.RemoveAsync(key, cancellationToken);

        await searchService.UpdateIndexAsync(game, cancellationToken);

        return Result.Success();
    }
}
