using FCG.Api.Catalog.Domain.Entities;

namespace FCG.Api.Catalog.Application.Contracts.Services;

public interface IGameSearchService
{
    Task IndexAsync(Game game, CancellationToken cancellationToken = default);
    Task UpdateIndexAsync(Game game, CancellationToken cancellationToken = default);
    Task<IEnumerable<GameSearchResult>> SearchAsync(string query, CancellationToken cancellationToken = default);
}

public record GameSearchResult(
    Guid Id,
    string Title,
    string Description,
    string Genre,
    string Publisher,
    decimal Price,
    bool IsActive,
    double Score);
