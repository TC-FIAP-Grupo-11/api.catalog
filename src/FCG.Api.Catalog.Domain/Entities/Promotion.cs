using FCG.Lib.Shared.Domain.Entities;

namespace FCG.Api.Catalog.Domain.Entities;

public class Promotion : BaseEntity
{
    public Guid GameId { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsActive { get; private set; }

    public Game Game { get; private set; } = null!;

    private Promotion(
        Guid gameId,
        decimal discountPercentage,
        DateTime startDate,
        DateTime endDate)
    {
        GameId = gameId;
        DiscountPercentage = discountPercentage;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = true;
    }

    public static Promotion Create(
        Guid gameId,
        decimal discountPercentage,
        DateTime startDate,
        DateTime endDate)
    {
        if (gameId == Guid.Empty)
            throw new ArgumentException("Game ID cannot be empty", nameof(gameId));

        if (discountPercentage <= 0 || discountPercentage > 100)
            throw new ArgumentException("Discount percentage must be between 0 and 100", nameof(discountPercentage));

        if (startDate >= endDate)
            throw new ArgumentException("Start date must be before end date", nameof(startDate));

        if (endDate <= DateTime.UtcNow)
            throw new ArgumentException("End date must be in the future", nameof(endDate));

        return new Promotion(gameId, discountPercentage, startDate, endDate);
    }

    public void Deactivate() => IsActive = false;

    public bool IsValid()
    {
        var now = DateTime.UtcNow;
        return IsActive && StartDate <= now && EndDate >= now;
    }

    public decimal CalculateDiscountedPrice(decimal originalPrice)
    {
        if (!IsValid())
            return originalPrice;

        var discount = originalPrice * (DiscountPercentage / 100);
        return originalPrice - discount;
    }

    public static decimal CalculateAccumulatedDiscount(IEnumerable<Promotion> promotions, decimal originalPrice)
    {
        var validPromotions = promotions.Where(p => p.IsValid()).ToList();
        
        if (validPromotions.Count == 0)
            return originalPrice;

        decimal totalDiscountPercentage = validPromotions.Sum(p => p.DiscountPercentage);
        
        // Limitar desconto a 100%
        if (totalDiscountPercentage > 100)
            totalDiscountPercentage = 100;

        var discount = originalPrice * (totalDiscountPercentage / 100);
        return Math.Max(0, originalPrice - discount);
    }
}
