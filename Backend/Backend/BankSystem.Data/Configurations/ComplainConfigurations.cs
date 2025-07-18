using BankSystem.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankSystem.Data.Configurations
{
    public class ComplainConfiguration : IEntityTypeConfiguration<Complain>
    {
        public void Configure(EntityTypeBuilder<Complain> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Describtion)
                .HasMaxLength(500);

            builder.Property(c => c.Recipient)
                .HasMaxLength(100);

            builder.Property(c => c.Timestamp)
                .IsRequired();

            builder.Property(c => c.Solved)
                .IsRequired();

            // One-to-Many: User -> Complains
            builder.HasOne(c => c.User)
                .WithMany(u => u.Complains)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
