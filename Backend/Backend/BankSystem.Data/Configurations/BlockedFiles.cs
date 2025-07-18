using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BankSystem.Data.Entities.Files;

namespace BankSystem.Data.Configurations.Files
{
    public class BlacklistedFileConfiguration : IEntityTypeConfiguration<BlacklistedFile>
    {
        public void Configure(EntityTypeBuilder<BlacklistedFile> builder)
        {
            // Set primary key
            builder.HasKey(b => b.Id);

            // Configure properties
            builder.Property(b => b.FileHash)
                .IsRequired()
                .HasMaxLength(64); // SHA-256 hash length

            builder.Property(b => b.BlockedDate)
                .IsRequired(); // Ensure BlockedDate is always provided

            // Optional: Specify table name if needed
            builder.ToTable("BlacklistedFiles");

            // Optionally, you can add indexes if needed for fast lookups
            builder.HasIndex(b => b.FileHash)
                .IsUnique(); // Ensure the hash is unique for blacklisted files
        }
    }
}
