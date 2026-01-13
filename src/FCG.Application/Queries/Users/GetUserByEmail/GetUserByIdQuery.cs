using MediatR;
using FCG.Domain.Entities;
using FCG.Lib.Shared.Application.Common.Models;

namespace FCG.Application.Queries.Users.GetUserByEmail;

public record GetUserByEmailQuery(string Email) : IRequest<Result<User>>;
