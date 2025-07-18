using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using BankSystem.Data.Contexts;

namespace BankSystem.Data.Contexts
{
    public class BankingContextFactory : IDesignTimeDbContextFactory<BankingContext>
    {
        public BankingContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BankingContext>();
            optionsBuilder.UseSqlServer("Server=DESKTOP-1GFUGR7\\SQLEXPRESS02;Database=STC_Final;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True");

            return new BankingContext(optionsBuilder.Options);
        }
    }
}
