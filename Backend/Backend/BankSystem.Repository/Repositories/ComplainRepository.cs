using global::BankSystem.Data.Contexts;
using global::BankSystem.Data.Entities;
using global::BankSystem.Repository.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;






namespace BankSystem.Repository.Repositories
{


    public class ComplainRepository : IComplainRepository
    {
        private readonly BankingContext _context;

        public ComplainRepository(BankingContext context)
        {
            _context = context;
        }

        public async Task<List<Complain>> GetAllAsync() =>
            await _context.Complains.ToListAsync();

        public async Task<Complain?> GetByIdAsync(int id) =>
            await _context.Complains.FindAsync(id);

        public async Task AddAsync(Complain complain)
        {
            await _context.Complains.AddAsync(complain);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Complain complain)
        {
            _context.Complains.Update(complain);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Complain complain)
        {
            _context.Complains.Remove(complain);
            await _context.SaveChangesAsync();
        }
    }

}
