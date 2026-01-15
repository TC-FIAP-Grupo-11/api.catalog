namespace FCG.Api.Catalog.Contracts.Requests;

public record UpdateGameRequest(
    string Title,
    string Description,
    string Genre,
    decimal Price,
    DateTime ReleaseDate,
    string Publisher
);
