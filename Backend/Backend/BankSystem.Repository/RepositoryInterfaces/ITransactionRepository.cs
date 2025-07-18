using BankSystem.Data.Entities;

namespace BankSystem.Repository.RepositoryInterfaces
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetTransactionsByAccountIdAsync(int accountId);
    }
}
