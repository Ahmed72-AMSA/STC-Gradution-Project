using BankSystem.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Data.Configurations
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Amount).HasColumnType("decimal(18,2)");
            builder.Property(s => s.Discount).HasColumnType("decimal(18,2)");

            builder.HasOne(s => s.Account)
                .WithMany(a => a.Subscriptions)
                .HasForeignKey(s => s.AccountId);

            builder.HasOne(s => s.Partner)
                .WithMany(p => p.Subscriptions)
                .HasForeignKey(s => s.PartnerId);
        }
    }
}
