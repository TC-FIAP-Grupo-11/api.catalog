namespace FCG.Api.Catalog.Domain.Entities;

public class GameReview
{
    public Guid Id { get; private set; }
    public Guid GameId { get; private set; }
    public string UserEmail { get; private set; }
    public int Rating { get; private set; }
    public string Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private GameReview()
    {
        UserEmail = string.Empty;
        Comment = string.Empty;
    }

    private GameReview(Guid gameId, string userEmail, int rating, string comment)
    {
        Id = Guid.NewGuid();
        GameId = gameId;
        UserEmail = userEmail;
        Rating = rating;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;
    }

    public static GameReview Create(Guid gameId, string userEmail, int rating, string comment)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.", nameof(rating));

        if (string.IsNullOrWhiteSpace(comment))
            throw new ArgumentException("Comment cannot be empty.", nameof(comment));

        return new GameReview(gameId, userEmail, rating, comment);
    }

    public static GameReview Reconstitute(Guid id, Guid gameId, string userEmail, int rating, string comment, DateTime createdAt)
    {
        var review = new GameReview();
        review.Id = id;
        review.GameId = gameId;
        review.UserEmail = userEmail;
        review.Rating = rating;
        review.Comment = comment;
        review.CreatedAt = createdAt;
        return review;
    }
}
