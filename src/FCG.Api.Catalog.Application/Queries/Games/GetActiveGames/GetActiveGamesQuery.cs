using MediatR;
using FCG.Api.Catalog.Domain.Entities;
using FCG.Lib.Shared.Application.Common.Models;

namespace FCG.Api.Catalog.Application.Queries.Games.GetActiveGames;

public record GetActiveGamesQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<PagedResult<Game>>>;
