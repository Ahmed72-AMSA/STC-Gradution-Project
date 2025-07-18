using BankSystem.Data.Entities;
using BankSystem.Data.Entities.Loans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankSystem.Data.Configurations
{
    public class LoanConfiguration : IEntityTypeConfiguration<Loan>
    {
        public void Configure(EntityTypeBuilder<Loan> builder)
        {
            builder.ToTable("Loans");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.IncomeAnnum)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(l => l.LoanAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(l => l.Status)
                .HasMaxLength(50);

            builder.Property(l => l.ApplicationDate)
                .HasDefaultValueSql("GETDATE()");

            // Configure foreign key relationship with User entity
            builder.HasOne(l => l.User)
                .WithMany(u => u.Loans)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict); // or Cascade depending on your design
        }

    }
}
