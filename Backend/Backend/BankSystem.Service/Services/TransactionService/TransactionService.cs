using BankSystem.Data.Contexts;
using BankSystem.Data.Entities;
using BankSystem.Service.Helper;
using BankSystem.Service.Helper.IOTPService;
using BankSystem.Service.Services.ReportService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankSystem.Service.Services.TransactionService
{
    public class TransactionService : ITransactionService
    {
        private readonly BankingContext _context;
        private readonly OTPService _otpService;
        private readonly EmailService _emailService;
        private readonly IReportService _reportService;

        public TransactionService(
            BankingContext context,
            OTPService otpService,
            EmailService emailService,
            IReportService reportService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _otpService = otpService ?? throw new ArgumentNullException(nameof(otpService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
        }

        public async Task<List<Transaction>> GetTransactionHistoryAsync(string accountSecret, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (string.IsNullOrEmpty(accountSecret))
                throw new ArgumentException("Account secret cannot be empty.");

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountSecret == accountSecret);

            if (account == null)
                throw new KeyNotFoundException("Account not found.");

            var query = _context.Transactions
                .Where(t => t.AccountID == account.Id);

            if (startDate.HasValue)
                query = query.Where(t => t.UpdatedAt >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(t => t.UpdatedAt <= endDate.Value);

            return await query
                .OrderByDescending(t => t.UpdatedAt)
                .ToListAsync();
        }

        public async Task<Transaction> GetTransactionDetailsAsync(int transactionId)
        {
            if (transactionId <= 0)
                throw new ArgumentException("Invalid transaction ID.");

            return await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == transactionId);
        }

        public async Task<bool> CancelTransactionAsync(int transactionId)
        {
            if (transactionId <= 0)
                throw new ArgumentException("Invalid transaction ID.");

            var transaction = await _context.Transactions
                .FindAsync(transactionId);

            if (transaction == null || transaction.Status == "Success")
                return false;

            transaction.Status = "Canceled";
            transaction.OTP = null;
            transaction.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GetTransactionStatusAsync(int transactionId)
        {
            if (transactionId <= 0)
                throw new ArgumentException("Invalid transaction ID.");

            var transaction = await _context.Transactions
                .FindAsync(transactionId);

            return transaction?.Status ?? "Not Found";
        }

        public async Task<string> InitiateWithdrawAsync(string accountSecret, decimal amount)
        {
            if (string.IsNullOrEmpty(accountSecret))
                throw new ArgumentException("Account secret cannot be empty.");
            if (amount <= 0)
                throw new ArgumentException("Withdrawal amount must be positive.");

            var account = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AccountSecret == accountSecret);

            if (account == null)
                return "Account not found.";
            if (account.Balance < amount)
                return "Insufficient balance.";
            if (account.User == null)
                return "User information not found.";

            var otp = _otpService.GenerateOtp();

            var transaction = new Transaction
            {
                AccountID = account.Id,
                Amount = amount,
                Status = "Pending",
                TransactionType = "Withdraw",
                OTP = otp,
                OTPGeneratedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(account.User.Email, otp, transaction.Id);
            return $"OTP sent for withdraw. Transaction ID: {transaction.Id}";
        }

        public async Task<(bool success, string message, decimal? newBalance)> ConfirmWithdrawAsync(
            string accountSecret,
            decimal amount,
            string otp,
            int transactionId)
        {
            if (string.IsNullOrEmpty(accountSecret))
                return (false, "Account secret cannot be empty.", null);
            if (amount <= 0)
                return (false, "Withdrawal amount must be positive.", null);
            if (string.IsNullOrEmpty(otp))
                return (false, "OTP cannot be empty.", null);
            if (transactionId <= 0)
                return (false, "Invalid transaction ID.", null);

            var account = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AccountSecret == accountSecret);

            if (account == null)
                return (false, "Account not found.", null);
            if (account.User == null)
                return (false, "User information not found.", null);

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t =>
                    t.Id == transactionId &&
                    t.AccountID == account.Id &&
                    t.Status == "Pending" &&
                    t.TransactionType == "Withdraw");

            if (transaction == null)
                return (false, "No pending transaction found.", null);
            if (transaction.OTP != otp || transaction.OTPGeneratedAt == null ||
                (DateTime.UtcNow - transaction.OTPGeneratedAt.Value).TotalMinutes > 5)
            {
                return (false, "Invalid or expired OTP.", null);
            }

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                account.Balance -= amount;
                account.LastUpdatedAt = DateTime.UtcNow;

                transaction.Status = "Success";
                transaction.OTP = null;
                transaction.UpdatedAt = DateTime.UtcNow;

                var reference = Guid.NewGuid().ToString()[..8];
                var reportDto = new TransactionReport
                {
                    UserFullName = account.User.UserName,
                    AccountNumber = account.AccountNumber,
                    TransactionType = "Withdraw",
                    Amount = amount,
                    Date = DateTime.UtcNow,
                    ReferenceNumber = reference,
                    Status = "Success"
                };

                var pdfBytes = _reportService.GenerateTransactionReceiptPdf(reportDto);
                if (pdfBytes == null)
                    throw new Exception("Failed to generate receipt PDF.");

                var reportHistory = new ReportHistory
                {
                    AccountId = account.Id,
                    TransactionType = "Withdraw",
                    Amount = amount,
                    Date = DateTime.UtcNow,
                    ReferenceNumber = reference,
                    Status = "Success",
                    PdfBytes = pdfBytes
                };

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                var subject = "Withdrawal Receipt - STC Bank";
                var body = $"Dear {account.User.UserName},\n\nYour withdrawal of ${amount} was successful. Please find your receipt attached.\n\nThank you for banking with us.";
                var attachmentName = $"WithdrawReceipt{reference}.pdf";

                await _emailService.SendEmailWithAttachmentAsync(
                    account.User.Email,
                    subject,
                    body,
                    pdfBytes,
                    attachmentName);

                return (true, "Withdrawal successful.", account.Balance);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                return (false, $"Transaction failed: {ex.Message}", null);
            }
        }

        public async Task<string> InitiateDepositAsync(string accountSecret, decimal amount)
        {
            if (string.IsNullOrEmpty(accountSecret))
                throw new ArgumentException("Account secret cannot be empty.");
            if (amount < 5)
                throw new ArgumentException("Minimum deposit is $5.");

            var account = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AccountSecret == accountSecret);

            if (account == null)
                return "Account not found.";
            if (account.User == null)
                return "User information not found.";

            var otp = _otpService.GenerateOtp();

            var transaction = new Transaction
            {
                AccountID = account.Id,
                Amount = amount,
                Status = "Pending",
                TransactionType = "Deposit",
                OTP = otp,
                OTPGeneratedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(account.User.Email, otp, transaction.Id);
            return $"OTP sent. Transaction ID: {transaction.Id}";
        }

        public async Task<string> ConfirmDepositAsync(string accountSecret, int transactionId, string otp)
        {
            if (string.IsNullOrEmpty(accountSecret))
                return "Account secret cannot be empty.";
            if (transactionId <= 0)
                return "Invalid transaction ID.";
            if (string.IsNullOrEmpty(otp))
                return "OTP cannot be empty.";

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountSecret == accountSecret);

            if (account == null)
                return "Invalid account.";

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t =>
                    t.Id == transactionId &&
                    t.AccountID == account.Id &&
                    t.Status == "Pending");

            if (transaction == null)
                return "Transaction not found.";
            if (transaction.OTP != otp || transaction.OTPGeneratedAt == null ||
                (DateTime.UtcNow - transaction.OTPGeneratedAt.Value).TotalMinutes > 5)
            {
                return "OTP invalid or expired.";
            }

            return "Awaiting customer service confirmation.";
        }

        public async Task<string> CustomerServiceConfirmDepositAsync(int transactionId, string otp)
        {
            if (transactionId <= 0)
                throw new ArgumentException("Invalid transaction ID.");
            if (string.IsNullOrEmpty(otp))
                throw new ArgumentException("OTP cannot be empty.");

            var transaction = await _context.Transactions
                .Include(t => t.Account)
                .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction == null)
                return "Transaction not found.";
            if (transaction.Status == "Success")
                return "Transaction already confirmed.";
            if (transaction.Account?.User == null)
                return "User information not found.";

            // Enhanced OTP validation with expiration check
            if (transaction.OTPGeneratedAt == null)
                return "OTP was never generated for this transaction.";

            var otpAge = DateTime.UtcNow - transaction.OTPGeneratedAt.Value;
            var otpExpirationMinutes = 5; // OTP expires after 5 minutes

            if (otpAge.TotalMinutes > otpExpirationMinutes)
            {
                // Mark transaction as failed due to expired OTP
                transaction.Status = "Failed";
                transaction.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return $"OTP expired. The OTP was valid until {transaction.OTPGeneratedAt.Value.AddMinutes(otpExpirationMinutes):g}";
            }

            if (transaction.OTP != otp)
                return "Invalid OTP provided.";

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var account = transaction.Account;
                account.Balance += transaction.Amount;
                account.LastUpdatedAt = DateTime.UtcNow;

                transaction.Status = "Success";
                transaction.OTP = null;
                transaction.UpdatedAt = DateTime.UtcNow;

                var reference = Guid.NewGuid().ToString()[..8];
                var reportDto = new TransactionReport
                {
                    UserFullName = account.User.UserName,
                    AccountNumber = account.AccountNumber,
                    TransactionType = "Deposit",
                    Amount = transaction.Amount,
                    Date = DateTime.UtcNow,
                    ReferenceNumber = reference,
                    Status = "Success"
                };

                var pdfBytes = _reportService.GenerateTransactionReceiptPdf(reportDto);
                if (pdfBytes == null)
                    throw new Exception("Failed to generate receipt PDF.");

                var reportHistory = new ReportHistory
                {
                    AccountId = account.Id,
                    TransactionType = "Deposit",
                    Amount = transaction.Amount,
                    Date = DateTime.UtcNow,
                    ReferenceNumber = reference,
                    Status = "Success",
                    PdfBytes = pdfBytes
                };

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                var subject = "Deposit Receipt - STC Bank";
                var body = $"Dear {account.User.UserName},\n\nYour deposit of ${transaction.Amount} was successful. Please find your receipt attached.\n\nThank you for banking with us.";
                var attachmentName = $"DepositReceipt{reference}.pdf";

                await _emailService.SendEmailWithAttachmentAsync(
                    account.User.Email,
                    subject,
                    body,
                    pdfBytes,
                    attachmentName);

                return $"Deposit successful. New Balance: {account.Balance:C}";
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                return $"Deposit confirmation failed: {ex.Message}";
            }
        }
        public async Task<(bool success, string message)> TransferMoneyAsync(
            string fromAccountSecret,
            string toAccountNumber,
            decimal amount)
        {
            if (string.IsNullOrEmpty(fromAccountSecret))
                return (false, "Source account secret cannot be empty.");
            if (string.IsNullOrEmpty(toAccountNumber))
                return (false, "Destination account number cannot be empty.");
            if (amount <= 0)
                return (false, "Transfer amount must be positive.");

            var fromAccount = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AccountSecret == fromAccountSecret);

            var toAccount = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AccountNumber == toAccountNumber);

            if (fromAccount == null)
                return (false, "Source account not found.");
            if (toAccount == null)
                return (false, "Destination account not found.");
            if (fromAccount.AccountNumber == toAccountNumber)
                return (false, "Cannot transfer to the same account.");
            if (fromAccount.Balance < amount)
                return (false, "Insufficient balance.");
            if (fromAccount.User == null || toAccount.User == null)
                return (false, "User information not found.");

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                fromAccount.Balance -= amount;
                toAccount.Balance += amount;
                fromAccount.LastUpdatedAt = DateTime.UtcNow;
                toAccount.LastUpdatedAt = DateTime.UtcNow;

                var fromReference = Guid.NewGuid().ToString("N")[..8];
                var toReference = Guid.NewGuid().ToString("N")[..8];

                var fromTransaction = new Transaction
                {
                    AccountID = fromAccount.Id,
                    Amount = amount,
                    Status = "Success",
                    TransactionType = "Transfer Sent",
                    UpdatedAt = DateTime.UtcNow
                };

                var toTransaction = new Transaction
                {
                    AccountID = toAccount.Id,
                    Amount = amount,
                    Status = "Success",
                    TransactionType = "Transfer Received",
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Transactions.AddRange(fromTransaction, toTransaction);
                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return (true, "Transfer successful.");
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                return (false, $"Transfer failed: {ex.Message}");
            }
        }
    }
}