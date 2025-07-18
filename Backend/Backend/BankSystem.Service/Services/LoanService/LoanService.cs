using BankSystem.Data.Contexts;
using BankSystem.Data.Entities.Loans;
using BankSystem.Repository.RepositoryInterfaces;
using BankSystem.Service.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BankSystem.Service.Services.LoanService
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IFileScanService _fileScanService;
        private readonly ILoginService _loginService;
        private readonly BankingContext _context;
        private readonly string _uploadPath;

        public LoanService(
            ILoanRepository loanRepository,
            IFileScanService fileScanService,
            ILoginService loginService,
            BankingContext context)
        {
            _loanRepository = loanRepository;
            _fileScanService = fileScanService;
            _loginService = loginService;
            _context = context;
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(_uploadPath);
        }

        public async Task<ServiceResult> ApplyForLoanAsync(LoanApplicationDto application, int userId, IFormFile nationalIdFile)
        {
            try
            {
                // Check for existing unpaid loans
                if (await _loanRepository.UserHasUnpaidLoansAsync(userId))
                {
                    return ServiceResult.Fail("You have an existing loan that hasn't been paid yet.");
                }

                // Validate and scan file
                var fileScanResult = await ScanAndValidateFile(nationalIdFile, userId);
                if (!fileScanResult.Success)
                {
                    return fileScanResult;
                }

                // Create loan
                var loan = new Loan
                {
                    IncomeAnnum = application.IncomeAnnum,
                    LoanAmount = application.LoanAmount,
                    LoanTerm = application.LoanTerm,
                    Education = application.Education,
                    SelfEmployed = application.SelfEmployed,
                    UserId = userId,
                    NationalIdDocumentHash = fileScanResult.Data.ToString(),
                    NationalIdDocumentPath = await SaveNationalIdFile(nationalIdFile),
                    Status = "Pending"
                };

                var createdLoan = await _loanRepository.CreateAsync(loan);
                return ServiceResult.Ok("Loan application submitted successfully.", createdLoan);
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"Error applying for loan: {ex.Message}");
            }
        }

        private async Task<ServiceResult> ScanAndValidateFile(IFormFile file, int userId)
        {
            if (file == null || file.Length == 0)
            {
                return ServiceResult.Fail("National ID document is required.");
            }

            // Scan file for malware or issues
            var scanResult = await _fileScanService.UploadAndScanFileAsync(file, null);
            if (scanResult is BadRequestObjectResult badRequest)
            {
                // If malicious, suspend user
                await _loginService.SuspendUserAsync(userId);
                return ServiceResult.Fail(badRequest.Value?.ToString() ?? "File scanning failed. Your account has been suspended.");
            }

            // Compute hash for storage
            using var stream = file.OpenReadStream();
            var hashService = new BankSystem.Service.Services.FileHashService.FileHashService();
            var fileHash = await hashService.ComputeSHA256Async(stream);

            return ServiceResult.Ok("File validated successfully.", fileHash);
        }

        private async Task<string> SaveNationalIdFile(IFormFile file)
        {
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(_uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"uploads/{fileName}";
        }

        public async Task<ServiceResult> ApproveLoanAsync(int loanId)
        {
            try
            {
                var loan = await _context.Loans
                    .Include(l => l.User)
                    .ThenInclude(u => u.Account)
                    .FirstOrDefaultAsync(l => l.Id == loanId);

                if (loan == null)
                {
                    return ServiceResult.Fail("Loan not found.");
                }

                if (loan.Status != "Pending")
                {
                    return ServiceResult.Fail("Loan is not in pending status.");
                }

                // Approve loan
                var success = await _loanRepository.ApproveLoanAsync(loanId);
                if (!success)
                {
                    return ServiceResult.Fail("Failed to approve loan.");
                }

                // Add funds to user account balance
                if (loan.User.Account != null)
                {
                    loan.User.Account.Balance += loan.LoanAmount;
                    await _context.SaveChangesAsync();
                }

                return ServiceResult.Ok("Loan approved successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"Error approving loan: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DenyLoanAsync(int loanId)
        {
            try
            {
                var success = await _loanRepository.DenyLoanAsync(loanId);
                return success
                    ? ServiceResult.Ok("Loan denied successfully.")
                    : ServiceResult.Fail("Failed to deny loan.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"Error denying loan: {ex.Message}");
            }
        }

        public async Task<ServiceResult> GetLoanByIdAsync(int loanId)
        {
            try
            {
                var loan = await _loanRepository.GetByIdAsync(loanId);
                return loan != null
                    ? ServiceResult.Ok("Loan retrieved successfully.", loan)
                    : ServiceResult.Fail("Loan not found.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"Error retrieving loan: {ex.Message}");
            }
        }

        public async Task<ServiceResult> GetLoansByUserIdAsync(int userId)
        {
            try
            {
                var loans = await _loanRepository.GetByUserIdAsync(userId);
                return ServiceResult.Ok("Loans retrieved successfully.", loans);
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"Error retrieving loans: {ex.Message}");
            }
        }

        public async Task<ServiceResult> MarkLoanAsPaidAsync(int loanId)
        {
            try
            {
                var success = await _loanRepository.MarkAsPaidAsync(loanId);
                return success
                    ? ServiceResult.Ok("Loan marked as paid successfully.")
                    : ServiceResult.Fail("Failed to mark loan as paid.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"Error marking loan as paid: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteLoanAsync(int loanId, int userId)
        {
            try
            {
                var loan = await _loanRepository.GetByIdAsync(loanId);
                if (loan == null)
                {
                    return ServiceResult.Fail("Loan not found.");
                }

                if (loan.UserId != userId)
                {
                    return ServiceResult.Fail("You are not authorized to delete this loan.");
                }

                if (loan.Status == "Approved" && !loan.IsPaid)
                {
                    return ServiceResult.Fail("Cannot delete an approved and unpaid loan.");
                }

                var success = await _loanRepository.DeleteAsync(loanId);
                return success
                    ? ServiceResult.Ok("Loan deleted successfully.")
                    : ServiceResult.Fail("Failed to delete loan.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"Error deleting loan: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UpdateLoanAsync(int loanId, LoanUpdateDto updateDto, int userId)
        {
            try
            {
                var loan = await _loanRepository.GetByIdAsync(loanId);
                if (loan == null)
                {
                    return ServiceResult.Fail("Loan not found.");
                }

                if (loan.UserId != userId)
                {
                    return ServiceResult.Fail("You are not authorized to update this loan.");
                }

                if (loan.Status != "Pending")
                {
                    return ServiceResult.Fail("Only pending loans can be updated.");
                }

                loan.IncomeAnnum = updateDto.IncomeAnnum;
                loan.LoanTerm = updateDto.LoanTerm;
                loan.Education = updateDto.Education;
                loan.SelfEmployed = updateDto.SelfEmployed;

                var success = await _loanRepository.UpdateAsync(loan);
                return success
                    ? ServiceResult.Ok("Loan updated successfully.")
                    : ServiceResult.Fail("Failed to update loan.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"Error updating loan: {ex.Message}");
            }
        }



        public async Task<ServiceResult> GetAllLoansAsync()
        {
            try
            {
                var loans = await _loanRepository.GetAllAsync();
                return ServiceResult.Ok("All loans retrieved successfully.", loans);
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"Error retrieving all loans: {ex.Message}");
            }
        }

    }
}
