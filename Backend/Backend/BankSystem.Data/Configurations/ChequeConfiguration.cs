using BankSystem.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankSystem.Data.Configurations
{
    public class ChequeConfiguration : IEntityTypeConfiguration<Cheque>
    {
        public void Configure(EntityTypeBuilder<Cheque> builder)
        {
            // Primary Key
            builder.HasKey(c => c.Id);

            // Properties Configuration
            builder.Property(c => c.ChequeNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.SenderUserName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.ReceiverName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.ReceiverBankName)
                .HasMaxLength(100);

            builder.Property(c => c.ReceiverAccountNumber)
                .HasMaxLength(50);

            builder.Property(c => c.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(c => c.IssueDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(c => c.ClearanceDate)
                .IsRequired(false);

            builder.Property(c => c.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(15)
                .HasDefaultValue(ChequeStatus.Pending);

            // Indexes
            builder.HasIndex(c => c.ChequeNumber)
                .IsUnique();

            builder.HasIndex(c => c.SenderUserName);

            builder.HasIndex(c => c.Status);

            builder.HasIndex(c => c.IssueDate);

            // Relationships
            builder.HasOne(c => c.Sender)
                .WithMany(u => u.Cheques)
                .HasForeignKey(c => c.SenderUserName)
                .HasPrincipalKey(u => u.UserName)
                .OnDelete(DeleteBehavior.Restrict);

            // Table Configuration
            builder.ToTable("Cheques");
        }
    }
}