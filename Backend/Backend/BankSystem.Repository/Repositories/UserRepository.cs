using BankSystem.Data.Contexts;
using BankSystem.Data.Entities;
using BankSystem.Repository.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly BankingContext _context;

        public UserRepository(BankingContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync() => await _context.Users.ToListAsync();
        public async Task<User?> GetByIdAsync(int id) => await _context.Users.FindAsync(id);
        public async Task<User?> GetByEmailAsync(string email) => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        public async Task<User?> GetByGmailAsync(string gmail) => await _context.Users.FirstOrDefaultAsync(u => u.Gmail == gmail);
        public async Task<User?> GetByFacebookIdAsync(string facebookId) => await _context.Users.FirstOrDefaultAsync(u => u.FacebookId == facebookId);
        public async Task AddAsync(User user) => await _context.Users.AddAsync(user);
        public async Task DeleteAsync(User user) => _context.Users.Remove(user);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();


        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);
        }


        public async Task<User?> GetByNationalIdAsync(string nationalId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.NationalID == nationalId);
        }


    }
}
