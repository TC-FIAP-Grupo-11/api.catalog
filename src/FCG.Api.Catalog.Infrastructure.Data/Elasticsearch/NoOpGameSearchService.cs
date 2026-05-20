using FCG.Api.Catalog.Application.Contracts.Services;
using FCG.Api.Catalog.Domain.Entities;

namespace FCG.Api.Catalog.Infrastructure.Data.Elasticsearch;

// Fallback used when Elasticsearch is not configured (local dev)
public class NoOpGameSearchService : IGameSearchService
{
    public Task IndexAsync(Game game, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task UpdateIndexAsync(Game game, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task<IEnumerable<GameSearchResult>> SearchAsync(string query, CancellationToken cancellationToken = default)
        => Task.FromResult(Enumerable.Empty<GameSearchResult>());
}
