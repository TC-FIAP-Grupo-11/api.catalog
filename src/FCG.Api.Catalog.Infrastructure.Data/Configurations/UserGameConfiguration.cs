using FCG.Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Api.Catalog.Infrastructure.Data.Configurations;

public class UserGameConfiguration : IEntityTypeConfiguration<UserGame>
{
    public void Configure(EntityTypeBuilder<UserGame> builder)
    {
        builder.ToTable("UserGames");

        builder.HasKey(ug => ug.Id);

        builder.Property(ug => ug.UserId)
            .IsRequired();

        builder.Property(ug => ug.GameId)
            .IsRequired();

        builder.Property(ug => ug.OrderId)
            .IsRequired();

        builder.Property(ug => ug.PurchaseDate)
            .IsRequired();

        builder.Property(ug => ug.PurchasePrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(ug => ug.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(UserGameStatus.Pending);

        builder.HasOne(ug => ug.Game)
            .WithMany(g => g.UserGames)
            .HasForeignKey(ug => ug.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ug => ug.OrderId)
            .IsUnique()
            .HasDatabaseName("IX_UserGames_OrderId");

        builder.HasIndex(ug => new { ug.UserId, ug.GameId })
            .HasDatabaseName("IX_UserGames_UserId_GameId");

        builder.HasIndex(ug => ug.UserId)
            .HasDatabaseName("IX_UserGames_UserId");

        builder.HasIndex(ug => ug.GameId)
            .HasDatabaseName("IX_UserGames_GameId");

        builder.HasIndex(ug => ug.PurchaseDate)
            .HasDatabaseName("IX_UserGames_PurchaseDate");
    }
}
