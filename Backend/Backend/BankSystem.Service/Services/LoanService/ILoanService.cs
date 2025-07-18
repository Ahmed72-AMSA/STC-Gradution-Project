using BankSystem.Data.Entities.Loans;
using BankSystem.Service.Helper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Service.Services.LoanService
{
    public interface ILoanService
    {
        Task<ServiceResult> ApplyForLoanAsync(LoanApplicationDto application, int userId, IFormFile nationalIdFile);
        Task<ServiceResult> ApproveLoanAsync(int loanId);
        Task<ServiceResult> DenyLoanAsync(int loanId);
        Task<ServiceResult> GetLoanByIdAsync(int loanId);
        Task<ServiceResult> GetLoansByUserIdAsync(int userId);
        Task<ServiceResult> MarkLoanAsPaidAsync(int loanId);
        Task<ServiceResult> DeleteLoanAsync(int loanId, int userId);
        Task<ServiceResult> UpdateLoanAsync(int loanId, LoanUpdateDto updateDto, int userId);
        Task<ServiceResult> GetAllLoansAsync();
    }
}
