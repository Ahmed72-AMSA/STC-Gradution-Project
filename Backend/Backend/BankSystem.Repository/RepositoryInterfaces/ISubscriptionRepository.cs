using BankSystem.Data.Entities;

namespace BankSystem.Repository.RepositoryInterfaces
{
    public interface ISubscriptionRepository : IRepository<Subscription>
    {
        Task<Subscription> GetActiveSubscriptionByAccountIdAsync(int accountId);
    }
}
