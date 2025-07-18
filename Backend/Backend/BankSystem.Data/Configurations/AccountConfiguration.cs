using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BankSystem.Data.Entities;

namespace BankSystem.Data.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            // Primary Key
            builder.HasKey(a => a.Id);

            // Properties Configuration
            builder.Property(a => a.AccountNumber)
                .IsRequired()
                .HasMaxLength(12); // Assuming 12-digit account numbers

            builder.Property(a => a.AccountSecret)
                .IsRequired()
                .HasMaxLength(6) // 6-digit secret
                .IsFixedLength(); // Ensure exactly 6 characters

            builder.Property(a => a.Balance)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0m);

            builder.Property(a => a.Status)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("Active");

            builder.Property(a => a.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(a => a.LastUpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Indexes
            builder.HasIndex(a => a.AccountNumber)
                .IsUnique();

            builder.HasIndex(a => a.AccountSecret)
                .IsUnique();

            builder.HasIndex(a => a.UserID);

            // Relationships
            builder.HasMany(a => a.Transactions)
                .WithOne(t => t.Account)
                .HasForeignKey(t => t.AccountID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(a => a.Subscriptions)
                .WithOne(s => s.Account)
                .HasForeignKey(s => s.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Table Configuration
            builder.ToTable("Accounts");
        }
    }
}