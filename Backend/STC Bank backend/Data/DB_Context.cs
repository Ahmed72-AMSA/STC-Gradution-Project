using Microsoft.EntityFrameworkCore;
using STC.Models;
using STC.Models.Chat_Service;

namespace STC.Data.Context
{
    public class STCSystemDbContext : DbContext
    {
        public STCSystemDbContext(DbContextOptions<STCSystemDbContext> options)
            : base(options)
        { }


        public DbSet<SignUp> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UploadedFile> UploadedFiles { get; set; }
        public DbSet<BlacklistedFile> BlacklistedFiles { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SignUp>()
                .HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("Email");

            modelBuilder.Entity<SignUp>()
                .HasIndex(u => u.UserName)
                .IsUnique()
                .HasDatabaseName("UserName");

 
        }
    }
}
