using BankSystem.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankSystem.Service.Services.TransactionService
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetTransactionHistoryAsync(string accountSecret, DateTime? startDate = null, DateTime? endDate = null);
        Task<Transaction> GetTransactionDetailsAsync(int transactionId);
        Task<bool> CancelTransactionAsync(int transactionId);
        Task<string> GetTransactionStatusAsync(int transactionId);

        Task<string> InitiateWithdrawAsync(string accountSecret, decimal amount);
        Task<(bool success, string message, decimal? newBalance)> ConfirmWithdrawAsync(string accountSecret, decimal amount, string otp, int TransactionId);

        Task<string> InitiateDepositAsync(string accountSecret, decimal amount);
        Task<string> ConfirmDepositAsync(string accountSecret, int transactionId, string otp);
        Task<string> CustomerServiceConfirmDepositAsync(int transactionId, string otp);

        Task<(bool success, string message)> TransferMoneyAsync(string fromAccountSecret, string toAccountNumber, decimal amount);
    }
}