using BankSystem.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Repository.RepositoryInterfaces
{
    public interface IComplainRepository
    {
        Task<List<Complain>> GetAllAsync();
        Task<Complain?> GetByIdAsync(int id);
        Task AddAsync(Complain complain);
        Task UpdateAsync(Complain complain);
        Task DeleteAsync(Complain complain);
    }
}
