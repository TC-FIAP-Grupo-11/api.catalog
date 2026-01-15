namespace FCG.Api.Catalog.Contracts.Responses;

public record PromotionPagedResponse(
    IEnumerable<PromotionItemResponse> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage
);
