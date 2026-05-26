using Microsoft.EntityFrameworkCore;

namespace WebShopMercantec.Models;

public partial class SnipeItContext
{
    public DbSet<WebShopUserCredits> WebShopUserCredits { get; set; } = null!;
    public DbSet<CreditTransaction> CreditTransactions { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        // Environment-specific Snipe-IT schema uses singular table name.
        modelBuilder.Entity<Asset>().ToTable("asset");

        modelBuilder.Entity<WebShopUserCredits>(entity =>
        {
            entity.ToTable("webshop_user_credits");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.AvailableCredits)
                .HasColumnName("available_credits")
                .HasColumnType("decimal(10,2)")
                .HasDefaultValue(0m);
            entity.Property(e => e.TotalSpent)
                .HasColumnName("total_spent")
                .HasColumnType("decimal(10,2)")
                .HasDefaultValue(0m);
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<CreditTransaction>(entity =>
        {
            entity.ToTable("webshop_credit_transactions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Amount)
                .HasColumnName("amount")
                .HasColumnType("decimal(10,2)");
            entity.Property(e => e.Type)
                .HasColumnName("type")
                .HasMaxLength(20);
            entity.Property(e => e.Reason)
                .HasColumnName("reason")
                .HasMaxLength(500);
            entity.Property(e => e.BalanceBefore)
                .HasColumnName("balance_before")
                .HasColumnType("decimal(10,2)");
            entity.Property(e => e.BalanceAfter)
                .HasColumnName("balance_after")
                .HasColumnType("decimal(10,2)");
            entity.Property(e => e.RelatedOrderId).HasColumnName("related_order_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(e => e.UserId);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("webshop_refresh_tokens");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Token)
                .HasColumnName("token")
                .HasMaxLength(500);
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.RevokedAt).HasColumnName("revoked_at");
            entity.Property(e => e.ReplacedByToken)
                .HasColumnName("replaced_by_token")
                .HasMaxLength(500);
            entity.HasIndex(e => e.Token);
            entity.HasIndex(e => e.UserId);
        });
    }
}

