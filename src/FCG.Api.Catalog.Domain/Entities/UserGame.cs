using FCG.Lib.Shared.Domain.Entities;

namespace FCG.Api.Catalog.Domain.Entities;

public enum UserGameStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2
}

public class UserGame : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid GameId { get; private set; }
    public Guid OrderId { get; private set; }
    public DateTime PurchaseDate { get; private set; }
    public decimal PurchasePrice { get; private set; }
    public UserGameStatus Status { get; private set; }

    public Game Game { get; private set; } = null!;

    private UserGame(Guid userId, Guid gameId, Guid orderId, decimal purchasePrice)
    {
        UserId = userId;
        GameId = gameId;
        OrderId = orderId;
        PurchasePrice = purchasePrice;
        PurchaseDate = DateTime.UtcNow;
        Status = UserGameStatus.Pending;
    }

    public static UserGame Create(Guid userId, Guid gameId, Guid orderId, decimal purchasePrice)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (gameId == Guid.Empty)
            throw new ArgumentException("Game ID cannot be empty", nameof(gameId));

        if (orderId == Guid.Empty)
            throw new ArgumentException("Order ID cannot be empty", nameof(orderId));

        if (purchasePrice < 0)
            throw new ArgumentException("Purchase price cannot be negative", nameof(purchasePrice));

        return new UserGame(userId, gameId, orderId, purchasePrice);
    }

    public void Complete()
    {
        if (Status != UserGameStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be completed");
        
        Status = UserGameStatus.Completed;
    }

    public void Fail()
    {
        if (Status != UserGameStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be marked as failed");
        
        Status = UserGameStatus.Failed;
    }
}
