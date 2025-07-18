
using BankSystem.Data.Entities;

namespace BankSystem.Repository.RepositoryInterfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByGmailAsync(string gmail);
        Task<User?> GetByFacebookIdAsync(string facebookId);
        Task AddAsync(User user);
        Task DeleteAsync(User user);
        Task SaveChangesAsync();

       Task<User> GetUserByUsernameAsync(string username);

        Task<User?> GetByNationalIdAsync(string nationalId);

    }

}
