using MediatR;
using FCG.Api.Catalog.Domain.Entities;
using FCG.Lib.Shared.Application.Common.Models;

namespace FCG.Api.Catalog.Application.Queries.Games.GetGameById;

public record GetGameByIdQuery(Guid Id) : IRequest<Result<Game>>;
