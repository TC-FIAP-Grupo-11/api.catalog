using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using FCG.Api.Catalog.Application.Contracts.Services;
using FCG.Api.Catalog.Domain.Entities;

namespace FCG.Api.Catalog.Infrastructure.Data.Elasticsearch;

public class GameSearchService(ElasticsearchClient client) : IGameSearchService
{
    private const string IndexName = "games";

    public async Task IndexAsync(Game game, CancellationToken cancellationToken = default)
    {
        var document = ToDocument(game);
        await client.IndexAsync(document, i => i.Index(IndexName).Id(document.Id), cancellationToken);
    }

    public async Task UpdateIndexAsync(Game game, CancellationToken cancellationToken = default)
    {
        var document = ToDocument(game);
        await client.IndexAsync(document, i => i.Index(IndexName).Id(document.Id), cancellationToken);
    }

    public async Task<IEnumerable<GameSearchResult>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        var response = await client.SearchAsync<GameIndexDocument>(s => s
            .Index(IndexName)
            .Query(q => q
                .Bool(b => b
                    .Must(m => m
                        .MultiMatch(mm => mm
                            .Query(query)
                            .Fields(new[] { "title^3", "description", "genre^2", "publisher" })
                            .Fuzziness(new Fuzziness("AUTO"))
                            .Type(TextQueryType.BestFields)
                        )
                    )
                    .Filter(f => f
                        .Term(t => t
                            .Field(d => d.IsActive)
                            .Value(true)
                        )
                    )
                )
            )
            .Sort(so => so.Score(sc => sc.Order(SortOrder.Desc)))
            .Size(50),
            cancellationToken);

        if (!response.IsValidResponse)
            return [];

        return response.Hits.Select(h => new GameSearchResult(
            Guid.Parse(h.Source!.Id),
            h.Source.Title,
            h.Source.Description,
            h.Source.Genre,
            h.Source.Publisher,
            h.Source.Price,
            h.Source.IsActive,
            h.Score ?? 0));
    }

    private static GameIndexDocument ToDocument(Game game) => new()
    {
        Id = game.Id.ToString(),
        Title = game.Title,
        Description = game.Description,
        Genre = game.Genre,
        Publisher = game.Publisher,
        Price = game.Price,
        IsActive = game.IsActive
    };
}
