using FCG.Lib.Shared.Application.Common.Models;
using FluentValidation;
using MediatR;

namespace FCG.Api.Catalog.Application.Commands.Promotions.DeactivatePromotion;

public record DeactivatePromotionCommand(Guid PromotionId) : IRequest<Result>;

public class DeactivatePromotionCommandValidator : AbstractValidator<DeactivatePromotionCommand>
{
    public DeactivatePromotionCommandValidator()
    {
        RuleFor(x => x.PromotionId)
            .NotEmpty()
            .WithMessage("Promotion ID is required");
    }
}
