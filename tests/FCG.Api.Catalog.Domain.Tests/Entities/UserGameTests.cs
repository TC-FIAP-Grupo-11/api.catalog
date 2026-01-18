using FCG.Api.Catalog.Domain.Entities;
using FluentAssertions;

namespace FCG.Api.Catalog.Domain.Tests.Entities;

public class UserGameTests
{
    [Fact]
    public void GivenEmptyUserId_WhenCreating_ThenShouldThrowArgumentException()
    {
        // Given (Dado)
        var act = () => UserGame.Create(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), 59.99m);

        // When & Then (Quando & Então)
        act.Should().Throw<ArgumentException>()
            .WithMessage("User ID cannot be empty*");
    }

    [Fact]
    public void GivenEmptyGameId_WhenCreating_ThenShouldThrowArgumentException()
    {
        // Given (Dado)
        var act = () => UserGame.Create(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), 59.99m);

        // When & Then (Quando & Então)
        act.Should().Throw<ArgumentException>()
            .WithMessage("Game ID cannot be empty*");
    }

    [Fact]
    public void GivenEmptyOrderId_WhenCreating_ThenShouldThrowArgumentException()
    {
        // Given (Dado)
        var act = () => UserGame.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, 59.99m);

        // When & Then (Quando & Então)
        act.Should().Throw<ArgumentException>()
            .WithMessage("Order ID cannot be empty*");
    }

    [Fact]
    public void GivenNegativePurchasePrice_WhenCreating_ThenShouldThrowArgumentException()
    {
        // Given (Dado)
        var act = () => UserGame.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), -10m);

        // When & Then (Quando & Então)
        act.Should().Throw<ArgumentException>()
            .WithMessage("Purchase price cannot be negative*");
    }

    [Fact]
    public void GivenValidData_WhenCreating_ThenShouldCreateWithPendingStatus()
    {
        // Given (Dado)
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var price = 59.99m;

        // When (Quando)
        var userGame = UserGame.Create(userId, gameId, orderId, price);

        // Then (Então)
        userGame.UserId.Should().Be(userId);
        userGame.GameId.Should().Be(gameId);
        userGame.OrderId.Should().Be(orderId);
        userGame.PurchasePrice.Should().Be(price);
        userGame.Status.Should().Be(UserGameStatus.Pending);
    }

    [Fact]
    public void GivenPendingUserGame_WhenCompleting_ThenShouldChangeStatusToCompleted()
    {
        // Given (Dado)
        var userGame = UserGame.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 59.99m);

        // When (Quando)
        userGame.Complete();

        // Then (Então)
        userGame.Status.Should().Be(UserGameStatus.Completed);
    }

    [Fact]
    public void GivenCompletedUserGame_WhenTryingToComplete_ThenShouldThrowInvalidOperationException()
    {
        // Given (Dado)
        var userGame = UserGame.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 59.99m);
        userGame.Complete();

        // When & Then (Quando & Então)
        var act = () => userGame.Complete();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only pending orders can be completed");
    }

    [Fact]
    public void GivenPendingUserGame_WhenFailing_ThenShouldChangeStatusToFailed()
    {
        // Given (Dado)
        var userGame = UserGame.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 59.99m);

        // When (Quando)
        userGame.Fail();

        // Then (Então)
        userGame.Status.Should().Be(UserGameStatus.Failed);
    }

    [Fact]
    public void GivenCompletedUserGame_WhenTryingToFail_ThenShouldThrowInvalidOperationException()
    {
        // Given (Dado)
        var userGame = UserGame.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 59.99m);
        userGame.Complete();

        // When & Then (Quando & Então)
        var act = () => userGame.Fail();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only pending orders can be marked as failed");
    }
}
