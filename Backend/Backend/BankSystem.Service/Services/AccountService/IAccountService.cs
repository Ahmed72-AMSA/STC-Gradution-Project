using BankSystem.Data.Entities;
using System.Threading.Tasks;

namespace BankSystem.Service.Services.AccountService
{
    public interface IAccountService
    {
        Task<Account> CreateAccountAsync(int userId);
        Task<decimal> GetAccountBalanceAsync(string accountSecret);
        Task<Account> GetAccountDetailsBySecretAsync(string accountSecret);
        Task<Account> GetAccountDetailsByUserIdAsync(int userId);
        Task<string> UpdateAccountDetailsAsync(string accountSecret, decimal newBalance, string accountStatus);
        Task<string> UpdateAccountStatusAsync(string accountSecret, string status);

        Task<decimal?> ApplyMonthlyTaxAsync(string accountSecret);
        Task<decimal?> ApplyAnnualTaxAsync(string accountSecret);
        Task<decimal?> ApplyMonthlyInterestAsync(string accountSecret);
        Task<decimal?> ApplyAnnualInterestAsync(string accountSecret);

        Task<string> UpdateAccountSecretAsync(string accountSecret, string newSecret);
    }
}