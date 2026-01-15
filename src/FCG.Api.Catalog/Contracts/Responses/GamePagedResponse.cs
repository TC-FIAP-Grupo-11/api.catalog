using FCG.Api.Catalog.Domain.Entities;

namespace FCG.Api.Catalog.Contracts.Responses;

public record GamePagedResponse(
    IEnumerable<Game> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage
);
