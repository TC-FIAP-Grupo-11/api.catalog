using MediatR;
using FluentValidation;
using FCG.Lib.Shared.Application.Common.Models;

namespace FCG.Api.Catalog.Application.Commands.Games.DeactivateGame;

public record DeactivateGameCommand(Guid Id) : IRequest<Result>;

public class DeactivateGameCommandValidator : AbstractValidator<DeactivateGameCommand>
{
    public DeactivateGameCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Game ID is required");
    }
}
