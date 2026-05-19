namespace FCG.Api.Catalog.Application.Cache;

public record GameCacheDto(
    Guid Id,
    string Title,
    string Description,
    string Genre,
    decimal Price,
    DateTime ReleaseDate,
    string Publisher,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);
