using FCG.Lib.Shared.Application.Common.Errors;
using FCG.Lib.Shared.Application.Common.Models;
using FCG.Api.Catalog.Application.Contracts.Repositories;
using FCG.Api.Catalog.Application.Contracts.Users;
using FCG.Api.Catalog.Domain.Entities;
using MediatR;

namespace FCG.Api.Catalog.Application.Queries.Games.GetUserGames;

public class GetUserGamesQueryHandler(
    IGameRepository gameRepository,
    IUserApiService userApiService) : IRequestHandler<GetUserGamesQuery, Result<PagedResult<UserGame>>>
{
    private readonly IGameRepository _gameRepository = gameRepository;
    private readonly IUserApiService _userApiService = userApiService;

    public async Task<Result<PagedResult<UserGame>>> Handle(GetUserGamesQuery request, CancellationToken cancellationToken)
    {
        var user = await _userApiService.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
            return Result.Failure<PagedResult<UserGame>>(ApplicationErrors.User.NotFound(request.Email));

        var pagedResult = await _gameRepository.GetUserGamesPagedAsync(
            user.Id,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return Result.Success(pagedResult);
    }
}
