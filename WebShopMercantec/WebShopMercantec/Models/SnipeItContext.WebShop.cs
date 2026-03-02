using Microsoft.EntityFrameworkCore;

namespace WebShopMercantec.Models;

/// <summary>
/// Partial class расширение SnipeItContext для WebShop-specific таблиц.
/// НЕ трогаем SnipeItContext.cs (scaffolded) — расширяем через partial.
/// </summary>
public partial class SnipeItContext
{
    public DbSet<WebShopUserCredits> WebShopUserCredits { get; set; } = null!;
    public DbSet<CreditTransaction> CreditTransactions { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WebShopUserCredits>(entity =>
        {
            entity.ToTable("webshop_user_credits");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.Property(e => e.AvailableCredits).HasColumnType("decimal(10,2)").HasDefaultValue(0m);
            entity.Property(e => e.TotalSpent).HasColumnType("decimal(10,2)").HasDefaultValue(0m);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<CreditTransaction>(entity =>
        {
            entity.ToTable("webshop_credit_transactions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(10,2)");
            entity.Property(e => e.BalanceBefore).HasColumnType("decimal(10,2)");
            entity.Property(e => e.BalanceAfter).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Type).HasMaxLength(20);
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(e => e.UserId);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("webshop_refresh_tokens");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).HasMaxLength(500);
            entity.Property(e => e.ReplacedByToken).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(e => e.Token);
            entity.HasIndex(e => e.UserId);
        });
    }
}

