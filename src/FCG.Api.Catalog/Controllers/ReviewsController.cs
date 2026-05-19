using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FCG.Api.Catalog.Application.Commands.Reviews.CreateGameReview;
using FCG.Api.Catalog.Application.Queries.Reviews.GetGameReviews;
using FCG.Lib.Shared.Application.Common.Models;
using FCG.Lib.Shared.Application.Extensions;

namespace FCG.Api.Catalog.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateGameReviewCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return result.ToActionResult();

        return CreatedAtAction(
            nameof(GetByGame),
            new { gameId = command.GameId },
            new { reviewId = result.Value, message = "Review created successfully." });
    }

    [HttpGet("game/{gameId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByGame(Guid gameId)
    {
        var result = await _mediator.Send(new GetGameReviewsQuery(gameId));

        if (result.IsFailure)
            return result.ToActionResult();

        var reviews = result.Value.Select(r => new
        {
            r.Id,
            r.GameId,
            r.UserEmail,
            r.Rating,
            r.Comment,
            r.CreatedAt
        });

        return Ok(reviews);
    }
}
