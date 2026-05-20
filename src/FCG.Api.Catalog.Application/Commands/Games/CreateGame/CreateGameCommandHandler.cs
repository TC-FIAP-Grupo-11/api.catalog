using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using FCG.Api.Catalog.Application.Cache;
using FCG.Api.Catalog.Application.Contracts.Repositories;
using FCG.Api.Catalog.Application.Contracts.Services;
using FCG.Api.Catalog.Domain.Entities;
using FCG.Lib.Shared.Application.Common.Models;
using FCG.Lib.Shared.Application.Common.Errors;

namespace FCG.Api.Catalog.Application.Commands.Games.CreateGame;

public class CreateGameCommandHandler(
    IGameRepository gameRepository,
    IDistributedCache cache,
    IGameSearchService searchService)
    : IRequestHandler<CreateGameCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var game = Game.Create(
                request.Title,
                request.Description,
                request.Genre,
                request.Price,
                request.ReleaseDate,
                request.Publisher
            );

            await gameRepository.AddAsync(game, cancellationToken);
            await InvalidateCacheAsync(cancellationToken);
            await searchService.IndexAsync(game, cancellationToken);

            return Result.Success(game.Id);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<Guid>(Error.Validation("CreateGame.ValidationError", ex.Message));
        }
        catch (Exception)
        {
            return Result.Failure<Guid>(ApplicationErrors.Game.CreationFailed);
        }
    }

    private async Task InvalidateCacheAsync(CancellationToken cancellationToken)
    {
        foreach (var key in CacheKeys.GamesInvalidationKeys())
            await cache.RemoveAsync(key, cancellationToken);
    }
}
