using BankSystem.Data.Entities;
using BankSystem.Data.Entities.Files;
using BankSystem.Data.Entities.Loans;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BankSystem.Data.Contexts
{
    public class BankingContext : DbContext
    {

        public BankingContext(DbContextOptions<BankingContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankingContext).Assembly);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<UploadedFile> UploadedFiles { get; set; }
        public DbSet<BlacklistedFile> BlacklistedFiles { get; set; }

        public DbSet<Complain> Complains { get; set; }

        public DbSet<Messages> Messages { get; set; }

        public DbSet<Cheque> Cheques { get; set; }

        public DbSet<Loan> Loans { get; set; }





    }
}
