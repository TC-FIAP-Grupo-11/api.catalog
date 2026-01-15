using MediatR;
using FCG.Api.Catalog.Application.Contracts.Repositories;
using FCG.Api.Catalog.Domain.Entities;
using FCG.Lib.Shared.Application.Common.Models;

namespace FCG.Api.Catalog.Application.Queries.Games.GetActiveGames;

public class GetActiveGamesQueryHandler(IGameRepository gameRepository) : IRequestHandler<GetActiveGamesQuery, Result<PagedResult<Game>>>
{
    private readonly IGameRepository _gameRepository = gameRepository;

    public async Task<Result<PagedResult<Game>>> Handle(GetActiveGamesQuery request, CancellationToken cancellationToken)
    {
        var pagedGames = await _gameRepository.GetActiveGamesPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        return Result.Success(pagedGames);
    }
}
