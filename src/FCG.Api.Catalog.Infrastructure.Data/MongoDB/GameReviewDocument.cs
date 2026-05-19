using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FCG.Api.Catalog.Infrastructure.Data.MongoDB;

public class GameReviewDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid GameId { get; set; }

    public string UserEmail { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
