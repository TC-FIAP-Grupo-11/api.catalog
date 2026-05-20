using MediatR;
using FCG.Api.Catalog.Application.Contracts.Services;
using FCG.Lib.Shared.Application.Common.Models;

namespace FCG.Api.Catalog.Application.Queries.Games.SearchGames;

public class SearchGamesQueryHandler(IGameSearchService searchService)
    : IRequestHandler<SearchGamesQuery, Result<IEnumerable<GameSearchResult>>>
{
    public async Task<Result<IEnumerable<GameSearchResult>>> Handle(SearchGamesQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Q))
            return Result.Failure<IEnumerable<GameSearchResult>>(
                Error.Validation("Search.EmptyQuery", "Search query cannot be empty."));

        var results = await searchService.SearchAsync(request.Q, cancellationToken);
        return Result.Success(results);
    }
}
