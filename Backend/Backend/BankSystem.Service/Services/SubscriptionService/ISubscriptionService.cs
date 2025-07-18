using BankSystem.Data.Entities;

namespace BankSystem.Service.Services.SubscriptionService
{
    public interface ISubscriptionService
    {
        Task<string> AddOrUpdateSubscriptionAsync(int accountId, string subscriptionName, decimal amount, decimal discount, DateTime startDate, DateTime? endDate = null, int partnerId = 0);
        Task<Subscription> GetSubscriptionByAccountIdAsync(int accountId);
        Task<string> CancelSubscriptionAsync(int accountId);
        //Task<Subscription> GetSubscriptionByIdAsync(int subscriptionId);
        //Task<string> DeleteSubscriptionAsync(int subscriptionId);
        //Task<IEnumerable<Subscription>> GetSubscriptionsByPartnerIdAsync(int partnerId);
    }
}
