using FCG.Lib.Shared.Application.Common.Models;
using FluentValidation;
using MediatR;

namespace FCG.Api.Catalog.Application.Commands.Games.PurchaseGame;

public record PurchaseGameCommand(string Email, Guid GameId) : IRequest<Result<Guid>>;

public class PurchaseGameCommandValidator : AbstractValidator<PurchaseGameCommand>
{
    public PurchaseGameCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email must be valid");

        RuleFor(x => x.GameId)
            .NotEmpty()
            .WithMessage("Game ID is required");
    }
}
