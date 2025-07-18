using BankSystem.Data.Entities;
using System.Threading.Tasks;

namespace BankSystem.Repository.RepositoryInterfaces
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account> GetByAccountNumberAsync(string accountNumber);
        Task<Account> GetByAccountSecretAsync(string accountSecret);
        Task<Account> GetAccountByUserIdAsync(int userId);
        Task<bool> AccountExistsAsync(string accountNumber);
        Task<bool> AccountSecretExistsAsync(string accountSecret);
    }
}