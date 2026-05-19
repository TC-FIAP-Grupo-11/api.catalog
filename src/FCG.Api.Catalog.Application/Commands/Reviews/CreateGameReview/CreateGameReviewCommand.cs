using MediatR;
using FluentValidation;
using FCG.Lib.Shared.Application.Common.Models;

namespace FCG.Api.Catalog.Application.Commands.Reviews.CreateGameReview;

public record CreateGameReviewCommand(
    Guid GameId,
    string UserEmail,
    int Rating,
    string Comment
) : IRequest<Result<Guid>>;

public class CreateGameReviewCommandValidator : AbstractValidator<CreateGameReviewCommand>
{
    public CreateGameReviewCommandValidator()
    {
        RuleFor(x => x.GameId)
            .NotEmpty().WithMessage("GameId is required.");

        RuleFor(x => x.UserEmail)
            .NotEmpty().WithMessage("UserEmail is required.")
            .EmailAddress().WithMessage("UserEmail must be a valid email address.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Comment is required.")
            .MaximumLength(2000).WithMessage("Comment must have at most 2000 characters.");
    }
}
