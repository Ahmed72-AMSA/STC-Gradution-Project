using BankSystem.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Repository.RepositoryInterfaces
{
    public interface IMessageRepository
    {
        Task AddMessageAsync(Messages message);
        Task<List<Messages>> GetConversationAsync(string user1, string user2);
        Task<List<string>> GetUserConversationsAsync(string username);
    }
}
