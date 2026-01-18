using FCG.Api.Catalog.Domain.Entities;
using FluentAssertions;

namespace FCG.Api.Catalog.Domain.Tests.Entities;

public class PromotionTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(101)]
    public void GivenInvalidDiscountPercentage_WhenCreating_ThenShouldThrowArgumentException(decimal invalidDiscount)
    {
        // Given (Dado)
        var act = () => Promotion.Create(Guid.NewGuid(), invalidDiscount, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));

        // When & Then (Quando & Então)
        act.Should().Throw<ArgumentException>()
            .WithMessage("Discount percentage must be between 0 and 100*");
    }

    [Fact]
    public void GivenStartDateAfterEndDate_WhenCreating_ThenShouldThrowArgumentException()
    {
        // Given (Dado)
        var act = () => Promotion.Create(Guid.NewGuid(), 25m, DateTime.UtcNow.AddDays(7), DateTime.UtcNow);

        // When & Then (Quando & Então)
        act.Should().Throw<ArgumentException>()
            .WithMessage("Start date must be before end date*");
    }

    [Fact]
    public void GivenEndDateInThePast_WhenCreating_ThenShouldThrowArgumentException()
    {
        // Given (Dado)
        var act = () => Promotion.Create(Guid.NewGuid(), 25m, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(-1));

        // When & Then (Quando & Então)
        act.Should().Throw<ArgumentException>()
            .WithMessage("End date must be in the future*");
    }

    [Fact]
    public void GivenValidPromotion_WhenCalculatingDiscount_ThenShouldReturnCorrectPrice()
    {
        // Given (Dado)
        var promotion = Promotion.Create(Guid.NewGuid(), 25m, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddDays(7));

        // When (Quando)
        var discountedPrice = promotion.CalculateDiscountedPrice(100m);

        // Then (Então)
        discountedPrice.Should().Be(75m);
    }

    [Fact]
    public void GivenInactivePromotion_WhenCalculatingDiscount_ThenShouldReturnOriginalPrice()
    {
        // Given (Dado)
        var promotion = Promotion.Create(Guid.NewGuid(), 50m, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddDays(7));
        promotion.Deactivate();

        // When (Quando)
        var discountedPrice = promotion.CalculateDiscountedPrice(100m);

        // Then (Então)
        discountedPrice.Should().Be(100m);
    }

    [Fact]
    public void GivenMultipleActivePromotions_WhenCalculatingAccumulatedDiscount_ThenShouldSumDiscounts()
    {
        // Given (Dado)
        var gameId = Guid.NewGuid();
        var promotions = new List<Promotion>
        {
            Promotion.Create(gameId, 20m, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddDays(7)),
            Promotion.Create(gameId, 15m, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddDays(7)),
            Promotion.Create(gameId, 10m, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddDays(7))
        };

        // When (Quando)
        var discountedPrice = Promotion.CalculateAccumulatedDiscount(promotions, 100m);

        // Then (Então) - 20% + 15% + 10% = 45% de desconto
        discountedPrice.Should().Be(55m);
    }

    [Fact]
    public void GivenPromotionsExceeding100Percent_WhenCalculatingAccumulatedDiscount_ThenShouldCapAt100Percent()
    {
        // Given (Dado)
        var gameId = Guid.NewGuid();
        var promotions = new List<Promotion>
        {
            Promotion.Create(gameId, 50m, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddDays(7)),
            Promotion.Create(gameId, 40m, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddDays(7)),
            Promotion.Create(gameId, 30m, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddDays(7))
        };

        // When (Quando)
        var discountedPrice = Promotion.CalculateAccumulatedDiscount(promotions, 100m);

        // Then (Então) - Limitado a 100% = preço 0
        discountedPrice.Should().Be(0m);
    }

    [Fact]
    public void GivenMixOfActiveAndInactivePromotions_WhenCalculatingAccumulatedDiscount_ThenShouldOnlyUseActive()
    {
        // Given (Dado)
        var gameId = Guid.NewGuid();
        var activePromotion1 = Promotion.Create(gameId, 20m, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddDays(7));
        var inactivePromotion = Promotion.Create(gameId, 30m, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddDays(7));
        inactivePromotion.Deactivate();
        var activePromotion2 = Promotion.Create(gameId, 15m, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddDays(7));

        var promotions = new List<Promotion> { activePromotion1, inactivePromotion, activePromotion2 };

        // When (Quando)
        var discountedPrice = Promotion.CalculateAccumulatedDiscount(promotions, 100m);

        // Then (Então) - Apenas 20% + 15% = 35% de desconto
        discountedPrice.Should().Be(65m);
    }

    [Fact]
    public void GivenNoPromotions_WhenCalculatingAccumulatedDiscount_ThenShouldReturnOriginalPrice()
    {
        // Given (Dado)
        var promotions = new List<Promotion>();

        // When (Quando)
        var discountedPrice = Promotion.CalculateAccumulatedDiscount(promotions, 100m);

        // Then (Então)
        discountedPrice.Should().Be(100m);
    }
}
