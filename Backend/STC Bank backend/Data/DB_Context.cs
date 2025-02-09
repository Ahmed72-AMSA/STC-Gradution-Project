using Microsoft.EntityFrameworkCore;
using STC.Models;

namespace STC.Data.Context
{
    public class STCSystemDbContext : DbContext
    {
        public STCSystemDbContext(DbContextOptions<STCSystemDbContext> options)
            : base(options)
        { }

        public DbSet<SignUp> Users { get; set; }


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
