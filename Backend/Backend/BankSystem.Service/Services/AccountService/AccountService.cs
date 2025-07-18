using BankSystem.Data.Contexts;
using BankSystem.Data.Entities;
using BankSystem.Repository.RepositoryInterfaces;
using BankSystem.Service.Helper;
using BankSystem.Service.Helper.IOTPService;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BankSystem.Service.Services.AccountService
{
    public class AccountService : IAccountService
    {
        private readonly BankingContext _context;
        private readonly TaxService _taxService;
        private readonly InterestService _interestService;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly OTPService _otpService;
        private readonly EmailService _emailService;

        public AccountService(
            BankingContext context,
            TaxService taxService,
            InterestService interestService,
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository,
            OTPService otpService,
            EmailService emailService)
        {
            _context = context;
            _taxService = taxService;
            _interestService = interestService;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _otpService = otpService;
            _emailService = emailService;
        }

        private string GenerateAccountNumber()
        {
            var random = new Random();
            long part1 = random.Next(100000, 999999);
            long part2 = random.Next(100000, 999999);
            return (part1 * 1000000 + part2).ToString();
        }

        private string GenerateAccountSecret()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        public async Task<Account> CreateAccountAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var newAccount = new Account
            {
                UserID = userId,
                AccountNumber = GenerateAccountNumber(),
                AccountSecret = GenerateAccountSecret(),
                Balance = 0,
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            };

            // Ensure AccountNumber uniqueness
            while (await _accountRepository.AccountExistsAsync(newAccount.AccountNumber))
            {
                newAccount.AccountNumber = GenerateAccountNumber();
            }

            await _accountRepository.AddAsync(newAccount);

            // Send account details email
            await SendAccountCreationEmail(user, newAccount);

            return newAccount;
        }

        private async Task SendAccountCreationEmail(User user, Account account)
        {
            string subject = "Your New Bank Account Details";
            string body = $"""
                Hello {user.UserName},

                Your bank account has been successfully created.
                
                Account Details:
                - Account Number: {account.AccountNumber}
                - Account Secret: {account.AccountSecret}
                - Created At: {account.CreatedAt:yyyy-MM-dd HH:mm}

                Please keep your account secret confidential.

                Thank you for choosing our bank.
                """;

            await _emailService.SendEmailAsync(user.Email, subject, body);
        }

        public async Task<string> UpdateAccountSecretAsync(string accountSecret, string newSecret)
        {
            // Validate the new secret
            if (!Regex.IsMatch(newSecret, @"^\d{6}$"))
            {
                throw new ArgumentException("Account secret must be a 6-digit number");
            }

            var account = await _accountRepository.GetByAccountSecretAsync(accountSecret);
            if (account == null)
            {
                throw new Exception("Account not found.");
            }

            account.AccountSecret = newSecret;
            account.LastUpdatedAt = DateTime.UtcNow;

            await _accountRepository.UpdateAsync(account);

            // Send notification email
            await SendSecretUpdateEmail(account.User, account);

            return "Account secret updated successfully.";
        }

        private async Task SendSecretUpdateEmail(User user, Account account)
        {
            string subject = "Your Account Secret Has Been Updated";
            string body = $"""
                Hello {user.UserName},

                Your account secret has been successfully updated.
                
                New Account Secret: {account.AccountSecret}
                Updated At: {DateTime.UtcNow:yyyy-MM-dd HH:mm}

                If you didn't request this change, please contact support immediately.
                """;

            await _emailService.SendEmailAsync(user.Email, subject, body);
        }

        public async Task<decimal> GetAccountBalanceAsync(string accountSecret)
        {
            var account = await _accountRepository.GetByAccountSecretAsync(accountSecret);
            return account?.Balance ?? throw new Exception("Account not found.");
        }

        public async Task<Account> GetAccountDetailsBySecretAsync(string accountSecret)
        {
            return await _accountRepository.GetByAccountSecretAsync(accountSecret)
                ?? throw new Exception("Account not found.");
        }

        public async Task<Account> GetAccountDetailsByUserIdAsync(int userId)
        {
            return await _accountRepository.GetAccountByUserIdAsync(userId)
                ?? throw new Exception("Account not found for the given user.");
        }

        public async Task<string> UpdateAccountDetailsAsync(string accountSecret, decimal newBalance, string accountStatus)
        {
            var account = await _accountRepository.GetByAccountSecretAsync(accountSecret);
            if (account == null)
            {
                throw new Exception("Account not found.");
            }

            account.Balance = newBalance;
            account.Status = accountStatus;
            account.LastUpdatedAt = DateTime.UtcNow;

            await _accountRepository.UpdateAsync(account);
            return "Account updated successfully.";
        }

        public async Task<string> UpdateAccountStatusAsync(string accountSecret, string status)
        {
            if (status != "Active" && status != "Inactive")
            {
                throw new ArgumentException("Invalid status. Status must be either 'Active' or 'Inactive'.");
            }

            var account = await _accountRepository.GetByAccountSecretAsync(accountSecret);
            if (account == null)
            {
                throw new Exception("Account not found.");
            }

            account.Status = status;
            account.LastUpdatedAt = DateTime.UtcNow;

            await _accountRepository.UpdateAsync(account);
            return $"Account has been successfully {status}.";
        }

        public async Task<decimal?> ApplyMonthlyTaxAsync(string accountSecret)
        {
            var account = await _accountRepository.GetByAccountSecretAsync(accountSecret);
            if (account == null) return null;
            if (account.Balance < 10000) return account.Balance;

            account.Balance = _taxService.ApplyMonthlyTax(account.Balance);
            account.LastUpdatedAt = DateTime.UtcNow;
            await _accountRepository.UpdateAsync(account);

            return account.Balance;
        }

        public async Task<decimal?> ApplyAnnualTaxAsync(string accountSecret)
        {
            var account = await _accountRepository.GetByAccountSecretAsync(accountSecret);
            if (account == null) return null;
            if (account.Balance < 10000) return account.Balance;

            account.Balance = _taxService.ApplyAnnualTax(account.Balance);
            account.LastUpdatedAt = DateTime.UtcNow;
            await _accountRepository.UpdateAsync(account);

            return account.Balance;
        }

        public async Task<decimal?> ApplyMonthlyInterestAsync(string accountSecret)
        {
            var account = await _accountRepository.GetByAccountSecretAsync(accountSecret);
            if (account == null) return null;

            account.Balance = _interestService.ApplyMonthlyInterest(account.Balance);
            account.LastUpdatedAt = DateTime.UtcNow;
            await _accountRepository.UpdateAsync(account);

            return account.Balance;
        }

        public async Task<decimal?> ApplyAnnualInterestAsync(string accountSecret)
        {
            var account = await _accountRepository.GetByAccountSecretAsync(accountSecret);
            if (account == null) return null;

            account.Balance = _interestService.ApplyAnnualInterest(account.Balance);
            account.LastUpdatedAt = DateTime.UtcNow;
            await _accountRepository.UpdateAsync(account);

            return account.Balance;
        }
    }
}