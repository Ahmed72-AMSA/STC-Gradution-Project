using BankSystem.Data.Entities.Loans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Repository.RepositoryInterfaces
{
    public interface ILoanRepository
    {
        Task<Loan> GetByIdAsync(int id);
        Task<IEnumerable<Loan>> GetByUserIdAsync(int userId);
        Task<bool> UserHasUnpaidLoansAsync(int userId);
        Task<Loan> CreateAsync(Loan loan);
        Task<bool> UpdateAsync(Loan loan);
        Task<bool> DeleteAsync(int id);
        Task<bool> ApproveLoanAsync(int loanId);
        Task<bool> DenyLoanAsync(int loanId);
        Task<bool> MarkAsPaidAsync(int loanId);
        Task<IEnumerable<Loan>> GetAllAsync();

    }
}
