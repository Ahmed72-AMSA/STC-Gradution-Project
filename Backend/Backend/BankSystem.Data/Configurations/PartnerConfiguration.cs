//using BankSystem.Data.Entities;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Microsoft.EntityFrameworkCore;

//namespace BankSystem.Data.Configurations
//{
//    public class PartnerConfiguration : IEntityTypeConfiguration<Partner>
//    {
//        public void Configure(EntityTypeBuilder<Partner> builder)
//        {
//            builder.HasKey(p => p.Id);
//            builder.Property(p => p.CompanyName).IsRequired().HasMaxLength(100);
//            builder.Property(p => p.PhoneNumber).HasMaxLength(20);
//            builder.Property(p => p.Email).HasMaxLength(100);
//            builder.Property(p => p.Location).HasMaxLength(200);
//        }
//    }
//}
