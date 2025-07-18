using BankSystem.Data.Contexts;
using BankSystem.Data.Entities;
using BankSystem.Repository.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Repository.Repositories
{
    public class SubscriptionRepository : GenericRepository<Subscription>, ISubscriptionRepository
    {
        private readonly BankingContext _context;

        public SubscriptionRepository(BankingContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Subscription> GetActiveSubscriptionByAccountIdAsync(int accountId)
        {
            var subscription = await _context.Subscriptions
                .Where(s => s.AccountId == accountId && (s.EndDate == null || s.EndDate > DateTime.Now))
                .FirstOrDefaultAsync();

            return subscription;
        }
    }

}
