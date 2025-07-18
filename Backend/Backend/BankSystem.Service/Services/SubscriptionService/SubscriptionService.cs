using BankSystem.Data.Contexts;
using BankSystem.Data.Entities;
using BankSystem.Repository.RepositoryInterfaces;
using BankSystem.Service.Helper;

namespace BankSystem.Service.Services.SubscriptionService
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly BankingContext _context;
        private readonly EmailService _emailService;
        private readonly IAccountRepository _accountRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IPartnerRepository _partnerRepository;

        public SubscriptionService(BankingContext context, EmailService emailService, IAccountRepository accountRepository, ISubscriptionRepository subscriptionRepository, IPartnerRepository partnerRepository)
        {
            _context = context;
            _emailService = emailService;
            _accountRepository = accountRepository;
            _subscriptionRepository = subscriptionRepository;
            _partnerRepository = partnerRepository;
        }

        public async Task<string> AddOrUpdateSubscriptionAsync(int accountId, string subscriptionName, decimal amount, decimal discount, DateTime startDate, DateTime? endDate = null, int partnerId = 0)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
                return "Account not found.";

            // Validate partner only if it's not 0
            if (partnerId != 0)
            {
                var partner = await _partnerRepository.GetByIdAsync(partnerId);
                if (partner == null)
                    return "Partner not found.";
            }

            // Apply discount directly
            var finalAmount = amount - discount;
            if (finalAmount < 0) finalAmount = 0;

            var existingSubscription = await _subscriptionRepository.GetActiveSubscriptionByAccountIdAsync(accountId);

            if (existingSubscription != null)
            {
                existingSubscription.SubscriptionName = subscriptionName;
                existingSubscription.Amount = finalAmount;
                existingSubscription.Discount = discount;
                existingSubscription.StartDate = startDate;
                existingSubscription.EndDate = endDate ?? existingSubscription.EndDate;
                existingSubscription.PartnerId = partnerId;

                await _subscriptionRepository.UpdateAsync(existingSubscription);
                return $"Subscription updated successfully. Final amount: {finalAmount:C}";
            }
            else
            {
                var newSubscription = new Subscription
                {
                    AccountId = accountId,
                    SubscriptionName = subscriptionName,
                    Amount = finalAmount,
                    Discount = discount,
                    StartDate = startDate,
                    EndDate = endDate,
                    IsActive = true,
                    PartnerId = partnerId
                };

                await _subscriptionRepository.AddAsync(newSubscription);
                return $"Subscription added successfully. Final amount: {finalAmount:C}";
            }
        }

        public async Task<string> CancelSubscriptionAsync(int accountId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                return "Account not found.";
            }

            var activeSubscription = await _subscriptionRepository.GetActiveSubscriptionByAccountIdAsync(accountId);
            if (activeSubscription == null)
            {
                return "No active subscription found for this account.";
            }

            activeSubscription.EndDate = DateTime.Now;

            await _subscriptionRepository.UpdateAsync(activeSubscription);
            return "Subscription cancelled successfully.";
        }
        public async Task<Subscription> GetSubscriptionByAccountIdAsync(int accountId)
        {
            return await _subscriptionRepository.GetActiveSubscriptionByAccountIdAsync(accountId);
        }
    }
}
//using BankSystem.Data.Contexts;
//using BankSystem.Data.Contexts;
//using BankSystem.Data.Entities;
//using BankSystem.Service.Helper;
//using Microsoft.EntityFrameworkCore;

//namespace BankSystem.Service.Services.SubscriptionService
//{
//    public class SubscriptionService : ISubscriptionService
//    {
//        private readonly BankingContext _context;
//        private readonly EmailService _emailService;

//        public SubscriptionService(BankingContext context, EmailService emailService)
//        {
//            _context = context;
//            _emailService = emailService;
//        }

//        public async Task<bool> CreateSubscriptionAsync(int accountId, int partnerId, decimal amount, decimal discount = 0)
//        {
//            var account = await _context.Accounts.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == accountId);
//            var partner = await _context.Partners.FindAsync(partnerId);

//            if (account == null || partner == null)
//                return false;

//            var subscription = new Subscription
//            {
//                AccountId = accountId,
//                PartnerId = partnerId,
//                Amount = amount - discount,
//                Discount = discount,
//                StartDate = DateTime.UtcNow,
//                EndDate = DateTime.UtcNow.AddMonths(1),
//                IsActive = true
//            };

//            _context.Subscriptions.Add(subscription);
//            await _context.SaveChangesAsync();
//            return true;
//        }
//        public async Task ProcessSubscriptionsAsync()
//        {
//            var now = DateTime.UtcNow;
//            var subscriptions = await _context.Subscriptions
//                .Where(s => s.IsActive && s.EndDate <= now)
//                .Include(s => s.Account)
//                .ThenInclude(a => a.User)
//                .ToListAsync();

//            foreach (var subscription in subscriptions)
//            {
//                var account = subscription.Account;
//                var user = account.User;

//                if (account.Balance >= subscription.Amount)
//                {
//                    // ✅ Renew subscription
//                    account.Balance -= subscription.Amount;
//                    subscription.StartDate = now;
//                    subscription.EndDate = now.AddMonths(1);
//                    subscription.IsActive = true;

//                    Console.WriteLine($"Subscription {subscription.Id} renewed for User {user.Name}.");
//                }
//                else
//                {
//                    // ❌ Send warning email and set cancellation deadline (2 days later)
//                    subscription.IsActive = false;
//                    subscription.EndDate = now.AddDays(2);

//                    string subject = "⚠️ Subscription Renewal Failed!";
//                    string body = $@"
//                    <p>Dear {user.Name},</p>
//                    <p>Your subscription renewal failed due to insufficient balance.</p>
//                    <p>You have until {subscription.EndDate:yyyy-MM-dd} to top up before cancellation.</p>
//                    <p>Thank you!</p>";

//                    await _emailService.SendEmailAsync(user.Email, subject, body);
//                    Console.WriteLine($"Warning email sent to {user.Email}");
//                }
//            }

//            await _context.SaveChangesAsync();
//        }
//        public async Task CancelExpiredSubscriptionsAsync()
//        {
//            var now = DateTime.UtcNow;
//            var expiredSubscriptions = await _context.Subscriptions
//                .Where(s => !s.IsActive && s.EndDate <= now)
//                .Include(s => s.Account)
//                .ThenInclude(a => a.User)
//                .ToListAsync();

//            foreach (var subscription in expiredSubscriptions)
//            {
//                var user = subscription.Account.User;
//                if (user == null) continue;

//                _context.Subscriptions.Remove(subscription);

//                string subject = "❌ Subscription Canceled";
//                string body = $@"
//                <p>Dear {user.Name},</p>
//                <p>Your subscription has been canceled due to insufficient funds.</p>
//                <p>You can subscribe again anytime.</p>";

//                await _emailService.SendEmailAsync(user.Email, body);
//                Console.WriteLine($"Cancellation email sent to {user.Email}");
//            }

//            await _context.SaveChangesAsync();
//        }
//    }

//}
