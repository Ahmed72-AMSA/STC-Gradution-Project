using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Service.Helper
{
    public class InterestService
    {
        private const decimal MonthlyInterestRate = 0.02m; // 2%
        private const decimal AnnualInterestRate = 0.15m;  // 15%

        public decimal ApplyMonthlyInterest(decimal balance)
        {
            if (balance <= 0) return balance;
            var interest = balance * MonthlyInterestRate;
            return balance + interest;
        }

        public decimal ApplyAnnualInterest(decimal balance)
        {
            if (balance <= 0) return balance;
            var interest = balance * AnnualInterestRate;
            return balance + interest;
        }
    }
}
