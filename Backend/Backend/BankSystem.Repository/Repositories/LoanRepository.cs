using BankSystem.Data.Contexts;
using BankSystem.Data.Entities.Loans;
using BankSystem.Repository.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Repository.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private readonly BankingContext _context;

        public LoanRepository(BankingContext context)
        {
            _context = context;
        }

        public async Task<Loan> GetByIdAsync(int id)
        {
            return await _context.Loans.FindAsync(id);
        }

        public async Task<IEnumerable<Loan>> GetByUserIdAsync(int userId)
        {
            return await _context.Loans
                .Where(l => l.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> UserHasUnpaidLoansAsync(int userId)
        {
            // Check if user has any pending loans OR any unpaid approved loans
            return await _context.Loans
                .AnyAsync(l => l.UserId == userId && 
                             (l.Status == "Pending" || // Has pending loan
                              (l.Status == "Approved" && !l.IsPaid))); // Has unpaid approved loan
        }

        public async Task<Loan> CreateAsync(Loan loan)
        {
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
            return loan;
        }

        public async Task<bool> UpdateAsync(Loan loan)
        {
            _context.Loans.Update(loan);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null) return false;

            _context.Loans.Remove(loan);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ApproveLoanAsync(int loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null) return false;

            loan.Status = "Approved";
            loan.ApprovalDate = DateTime.UtcNow;
            loan.IsNationalIdVerified = true;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DenyLoanAsync(int loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null) return false;

            loan.Status = "Denied";

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> MarkAsPaidAsync(int loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null) return false;

            loan.IsPaid = true;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Loan>> GetAllAsync()
        {
            return await _context.Loans.ToListAsync();
        }
    }
}
