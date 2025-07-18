using BankSystem.Data.Entities.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankSystem.Data.Configurations
{
    public class UploadedFileConfiguration : IEntityTypeConfiguration<UploadedFile>
    {
        public void Configure(EntityTypeBuilder<UploadedFile> builder)
        {
            builder.ToTable("UploadedFiles");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(f => f.FileHash)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(f => f.FilePath)
                .IsRequired();

            builder.Property(f => f.Status)
                .IsRequired()
                .HasMaxLength(100)
                .HasDefaultValue("No Status");

            builder.Property(f => f.FileType)
                .HasMaxLength(100);

            builder.Property(f => f.FileSize);

            builder.Property(f => f.TotalEngines);
            builder.Property(f => f.MaliciousCount);
            builder.Property(f => f.HarmlessCount);
            builder.Property(f => f.SuspiciousCount);
            builder.Property(f => f.UndetectedCount);

            builder.Property(f => f.ScanDate);

            builder.Property(f => f.ScanDetailsJson);
        }
    }
}
