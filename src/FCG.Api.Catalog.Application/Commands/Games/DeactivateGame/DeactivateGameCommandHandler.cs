using MediatR;
using FCG.Api.Catalog.Application.Contracts.Repositories;
using FCG.Lib.Shared.Application.Common.Models;
using FCG.Lib.Shared.Application.Common.Errors;

namespace FCG.Api.Catalog.Application.Commands.Games.DeactivateGame;

public class DeactivateGameCommandHandler(IGameRepository gameRepository) : IRequestHandler<DeactivateGameCommand, Result>
{
    private readonly IGameRepository _gameRepository = gameRepository;

    public async Task<Result> Handle(DeactivateGameCommand request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.Id, cancellationToken);

        if (game is null)
            return Result.Failure(ApplicationErrors.Game.NotFound(request.Id));

        game.Deactivate();

        await _gameRepository.UpdateAsync(game, cancellationToken);

        return Result.Success();
    }
}
