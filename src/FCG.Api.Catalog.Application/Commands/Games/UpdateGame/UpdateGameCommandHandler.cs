using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using FCG.Api.Catalog.Application.Cache;
using FCG.Api.Catalog.Application.Contracts.Repositories;
using FCG.Api.Catalog.Application.Contracts.Services;
using FCG.Lib.Shared.Application.Common.Models;
using FCG.Lib.Shared.Application.Common.Errors;

namespace FCG.Api.Catalog.Application.Commands.Games.UpdateGame;

public class UpdateGameCommandHandler(
    IGameRepository gameRepository,
    IDistributedCache cache,
    IGameSearchService searchService)
    : IRequestHandler<UpdateGameCommand, Result>
{
    public async Task<Result> Handle(UpdateGameCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var game = await gameRepository.GetByIdAsync(request.Id, cancellationToken);

            if (game is null)
                return Result.Failure(ApplicationErrors.Game.NotFound(request.Id));

            game.Update(
                request.Title,
                request.Description,
                request.Genre,
                request.Price,
                request.ReleaseDate,
                request.Publisher
            );

            await gameRepository.UpdateAsync(game, cancellationToken);
            await InvalidateCacheAsync(cancellationToken);
            await searchService.UpdateIndexAsync(game, cancellationToken);

            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(Error.Validation("UpdateGame.ValidationError", ex.Message));
        }
        catch (Exception)
        {
            return Result.Failure(ApplicationErrors.Game.UpdateFailed);
        }
    }

    private async Task InvalidateCacheAsync(CancellationToken cancellationToken)
    {
        foreach (var key in CacheKeys.GamesInvalidationKeys())
            await cache.RemoveAsync(key, cancellationToken);
    }
}
