using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using FCG.Api.Catalog.Application.Cache;
using FCG.Api.Catalog.Application.Contracts.Repositories;
using FCG.Api.Catalog.Domain.Entities;
using FCG.Lib.Shared.Application.Common.Models;

namespace FCG.Api.Catalog.Application.Queries.Games.GetActiveGames;

public class GetActiveGamesQueryHandler(IGameRepository gameRepository, IDistributedCache cache)
    : IRequestHandler<GetActiveGamesQuery, Result<PagedResult<Game>>>
{
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

    public async Task<Result<PagedResult<Game>>> Handle(GetActiveGamesQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.GamesActive(request.PageNumber, request.PageSize);
        var cached = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (cached is not null)
        {
            var dto = JsonSerializer.Deserialize<PagedResult<GameCacheDto>>(cached)!;
            var games = dto.Items.Select(g => Game.Reconstitute(
                g.Id, g.Title, g.Description, g.Genre, g.Price,
                g.ReleaseDate, g.Publisher, g.IsActive, g.CreatedAt, g.UpdatedAt));
            return Result.Success(new PagedResult<Game>(games, dto.TotalCount, dto.PageNumber, dto.PageSize));
        }

        var pagedGames = await gameRepository.GetActiveGamesPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

        var cacheValue = new PagedResult<GameCacheDto>(
            pagedGames.Items.Select(g => new GameCacheDto(
                g.Id, g.Title, g.Description, g.Genre, g.Price,
                g.ReleaseDate, g.Publisher, g.IsActive, g.CreatedAt, g.UpdatedAt)),
            pagedGames.TotalCount, pagedGames.PageNumber, pagedGames.PageSize);

        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(cacheValue), CacheOptions, cancellationToken);

        return Result.Success(pagedGames);
    }
}
