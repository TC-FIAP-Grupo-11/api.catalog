using MediatR;
using FCG.Domain.Entities;
using FCG.Lib.Shared.Application.Common.Models;

namespace FCG.Application.Queries.Games.GetActiveGames;

public record GetActiveGamesQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<PagedResult<Game>>>;
