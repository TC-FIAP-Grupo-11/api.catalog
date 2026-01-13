using MediatR;
using FCG.Domain.Entities;
using FCG.Lib.Shared.Application.Common.Models;

namespace FCG.Application.Queries.Games.GetGameById;

public record GetGameByIdQuery(Guid Id) : IRequest<Result<Game>>;
