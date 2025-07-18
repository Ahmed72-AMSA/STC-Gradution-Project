using BankSystem.Data.Entities;

namespace BankSystem.Repository.RepositoryInterfaces
{
    public interface IPartnerRepository : IRepository<Partner>
    {
        Task<IEnumerable<Partner>> GetPartnersWithActiveSubscriptionsAsync();
    }
}
