using MediatR;
using FCG.Api.Catalog.Application.Contracts.Services;
using FCG.Lib.Shared.Application.Common.Models;

namespace FCG.Api.Catalog.Application.Queries.Games.SearchGames;

public record SearchGamesQuery(string Q) : IRequest<Result<IEnumerable<GameSearchResult>>>;
